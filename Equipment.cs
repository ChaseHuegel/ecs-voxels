using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish;
using Swordfish.items;

[Serializable]
public class Equipment
{
	//	Variables
	[SerializeField]
	protected Item[] contents = null;

	[SerializeField]
	protected int mainHandSlot = 0;

	[SerializeField]
	protected Inventory inventory = null;

	public Equipment()
	{
		contents = new Item[Enum.GetNames(typeof(EquipmentSlot)).Length];
	}

	//	Getters
	public Inventory 	getInventory() 		{ return inventory; }
	public Item[] 		getContents() 		{ return contents; }
	public int			getMainHandSlot() 	{ return mainHandSlot; }
	public Item 		getSlot(EquipmentSlot _slot)
						{
							Item item = contents[(int)_slot];
							if (_slot == EquipmentSlot.MAINHAND)
							{
								if (inventory != null) { contents[mainHandSlot] = inventory.at(mainHandSlot); }
								item = contents[mainHandSlot];
							}

							return ( item == null ? ItemType.VOID.toItem() : item  );
						}

	//	Setters
	public void 	setInventory(Inventory c) 	{ inventory = c; }
	public void 	setMainHandSlot(int c) 		{ mainHandSlot = c; OnEquipmentChange(new EquipmentChangeEvent( (EquipmentSlot)c )); }
	public void 	setContents(Item[] c)
					{
						contents = c;
						foreach(Item _item in contents) { OnEquipmentChange(new EquipmentChangeEvent(_item)); }
					}
	public void 	setSlot(EquipmentSlot _slot, Item _item)
					{
						if (_slot == EquipmentSlot.MAINHAND)
							{ contents[mainHandSlot] = ( _item.getType() == ItemType.VOID ? null : _item  ); }
						else
							{ contents[(int)_slot] = ( _item.getType() == ItemType.VOID ? null : _item  ); }

						OnEquipmentChange(new EquipmentChangeEvent(_item, _slot));
					}

	//	Events

	protected virtual void OnEquipmentChange(EquipmentChangeEvent e)
	{
		EventHandler<EquipmentChangeEvent> handler = EquipmentChange;
		if (handler != null)
		{
			handler(this, e);
		}
	}

    public event EventHandler<EquipmentChangeEvent> EquipmentChange;
}

public class EquipmentChangeEvent : EventArgs
{
	public EquipmentSlot slot = EquipmentSlot.GENERIC;
	public Item item = null;

	public EquipmentChangeEvent(EquipmentSlot _slot = EquipmentSlot.GENERIC)
	{
		slot = _slot;
	}

	public EquipmentChangeEvent(Item _item = null, EquipmentSlot _slot = EquipmentSlot.GENERIC)
	{
		item = _item;
		slot = _slot;
	}
}