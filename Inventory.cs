using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish;
using Swordfish.items;

[Serializable]
public class Inventory
{
	//	Variables
	[SerializeField]
	protected Item[] contents = new Item[40];

	//	Getters
	public Item[] 	getContents() 	{ return contents; }
	public int 		getSize() 		{ return contents.Length; }

	//	Setters
	public void 	setContents(Item[] c) 	{ contents = c; }
	public void 	setSize(int c) 			{ Array.Resize(ref contents, c); }

	//	Constructor
	public Inventory(int _slots = 40)
	{
		contents = new Item[_slots];
	}

//---------------------------------------------------------
	//	Public methods

	//	Clear all slots
	public void Clear()
	{
		contents = new Item[contents.Length];
		OnInventoryChange(new InventoryChangeEvent());
	}

	//	Clear a slot
	public void Clear(int _slot)
	{
		contents[_slot] = null;
		OnInventoryChange(new InventoryChangeEvent(_slot));
	}

	//	Get a slot
	public Item at(int _slot)
	{
		if (_slot >= contents.Length) { return null; }

		if (contents[_slot] != null && contents[_slot].isValid() == false) { contents[_slot] = null; }

		return contents[_slot];
	}

	//	Set a slot by type
	public void setAt(int _slot, ItemType _type, int _amount = -1)
	{
		setAt(_slot, _type.toItem(), _amount);
	}

	//	Set a slot by item
	public void setAt(int _slot, Item _item, int _amount = -1)
	{
		if (_item != null && _item.isValid() == false) { _item = null; }

		if (_amount > 0) { _item.setAmount(_amount); }

		contents[_slot] = _item;
		OnInventoryChange(new InventoryChangeEvent(_item, _slot));
	}

	//	Get the first empty slot
	public int FirstEmpty()
	{
		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] == null || contents[i].isValid() == false) { return i; }
		}

		return -1;
	}

	//	Get the first slot matching item
	public int First(Item _item, int _minAmount = 1)
	{
		return First(_item.getType(), _minAmount);
	}

	//	Get the first slot matching type
	public int First(ItemType _type, int _minAmount = 1)
	{
		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] != null && contents[i].isValid() == false) { contents[i] = null; return -1; }

			if (contents[i] != null &&
				_type == contents[i].getType() &&
				contents[i].getAmount() >= _minAmount)
				{
					return i;
				}

			if (contents[i] == null &&
					_type == ItemType.VOID)
				{
					return i;
				}
		}

		return -1;
	}

	//	Get all slots matching by type
	public int[] Find(ItemType _type)
	{
		return Find(_type.toItem());
	}

	//	Get all slots matching by item
	public int[] Find(Item _item)
	{
		List<int> matchingSlots = new List<int>();

		for (int i = 0; i < contents.Length; i++)
		{
			if (contents[i] != null && contents[i].isValid() == false) { contents[i] = null; continue; }

			if (contents[i] != null &&
				_item.Equals(contents[i]) == true)
				{
					matchingSlots.Add(i);
				}

			if (contents[i] == null &&
					_item.getType() == ItemType.VOID)
				{
					matchingSlots.Add(i);
				}
		}

		return matchingSlots.ToArray();
	}

	//	Add items by type
	public Item[] Add(ItemType[] _types)
	{
		List<Item> items = new List<Item>();

		for (int i = 0; i < _types.Length; i++)
		{
			items.Add( this.Add( _types[i] ) );
		}

		return items.ToArray();
	}

	//	Add items by item
	public Item[] Add(Item[] _items)
	{
		List<Item> items = new List<Item>();

		for (int i = 0; i < _items.Length; i++)
		{
			items.Add( this.Add( _items[i] ) );
		}

		return items.ToArray();
	}

	//	Add item by type
	public Item Add(ItemType _type, int _amount = 1)
	{
		return this.Add( _type.toItem(), _amount );
	}

	//	Add item by item
	public Item Add(Item _item, int _amount = -1)
	{
		int amountToAdd = _amount;
		if (_amount < 1) { amountToAdd = _item.getAmount(); }

		int[] slots = Find( _item );

		// Add to each matching item until there is no more to add or we have no slots to add to
		for (int i = 0; i < slots.Length; i++)
		{
			amountToAdd = contents[ slots[i] ].modifyAmount(amountToAdd);

			OnInventoryChange(new InventoryChangeEvent(contents[ slots[i] ], slots[i]));

			if (amountToAdd <= 0) { return null; }	//	Finished adding items
		}

		//	Items not found or ran out of slots to add to. Attempt to fill empty slots.
		while (amountToAdd > 0)
		{
			int slot = FirstEmpty();

			if (slot > -1)	//	Found an empty slot
			{
				Item thisItem = _item.copy();
				amountToAdd = thisItem.setAmount(amountToAdd);
				contents[slot] = thisItem;

				OnInventoryChange(new InventoryChangeEvent(contents[slot], slot));

				if (amountToAdd <= 0) { return null; }	//	Finished adding items
			}
			else	//	No empty slot found
			{
				break;
			}
		}

		_item.setAmount(amountToAdd);
		return _item;	//	Failed finish adding item to inventory
	}

	//	Remove item by type, default to removing all
	public Item Remove(ItemType _type, int _amount = 99999)
	{
		return Remove(_type.toItem(), _amount);
	}

	//	Remove item by item
	public Item Remove(Item _item, int _amount = -1)
	{
		int amountToRemove = _amount;
		if (_amount < 1) { amountToRemove = _item.getAmount(); }

		int[] slots = Find( _item );

		if (slots.Length > 0)	//	Items of this kind are in the inventory
		{
			// Remove from each slot until there is no more to remove or we run out of slots
			for (int i = 0; i < slots.Length; i++)
			{
				amountToRemove = contents[ slots[i] ].modifyAmount(-amountToRemove);

				OnInventoryChange(new InventoryChangeEvent(contents[ slots[i] ], slots[i]));

				if (amountToRemove <= 0 || contents[ slots[i] ] == null) { return null; }	//	Finished removing items
			}
		}

		_item.setAmount(amountToRemove);
		return _item;	//	Failed finish removing item from inventory
	}

	//	Check if this contains some item with a minimum amount
	public bool Contains(Item _item, int _amount = -1)
	{
		if (_amount < 1) { _amount = _item.getAmount(); }
		return Contains( _item.getType(), _amount );
	}

	//	Check if this contains some item type with a minimum amount
	public bool Contains(ItemType _type, int _amount = 1)
	{
		int amountToFind = _amount;

		int[] slots = Find( _type );

		if (slots.Length > 0)	//	Items of this kind are in the inventory
		{
			// Find amount of each slot until we run out of slots or found the amount
			for (int i = 0; i < slots.Length; i++)
			{
				amountToFind -= contents[ slots[i] ].getAmount();

				if (amountToFind <= 0) { return true; }	//	Found amount of some item
			}
		}

		return false;
	}

//---------------------------------------------------------
	//	Events

	protected virtual void OnInventoryChange(InventoryChangeEvent e)
	{
		EventHandler<InventoryChangeEvent> handler = InventoryChange;
		if (handler != null)
		{
			handler(this, e);
		}
	}

    public event EventHandler<InventoryChangeEvent> InventoryChange;
}

public class InventoryChangeEvent : EventArgs
{
	public int slot = -1;
	public Item item = null;

	public InventoryChangeEvent() {}

	public InventoryChangeEvent(int _slot = -1)
	{
		slot = _slot;
	}

	public InventoryChangeEvent(Item _item = null, int _slot = -1)
	{
		item = _item;
		slot = _slot;
	}
}