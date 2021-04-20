using System;

namespace Swordfish
{
namespace items
{
	[Serializable]
    public class Item
    {
		[UnityEngine.SerializeField]
		protected int amount 		= 1;
		protected int durability 	= 1;

		[UnityEngine.SerializeField]
		protected string name;

		protected ItemData itemData;

		public override bool Equals(object value)
		{
			if(value == null)
			{
				return false;
			}

			Item item = value as Item;

			return ( item.getType() == this.getType() );
		}

		public Item()
		{
			itemData = new ItemData();
			name = getName();
		}

		public virtual ItemType getType()	//	This item's type
		{
			return ItemType.VOID;
		}

		public virtual string getName()	//	This item's name
		{
			return "default";
		}

		public virtual CachedImage getImageData()	//	This item's texture
		{
			return null;
		}

		public virtual Position getViewOffset()	//	This item's view model offset
		{
			return new Position(0, 0, 0);
		}

		public virtual Position getViewRotation()	//	This item's view model rotation
		{
			return new Position(0, 0, 0);
		}

		public virtual Position getViewScale()	//	This item's view model scale
		{
			return new Position(1, 1, 1);
		}

		public virtual Position getDisplayOffset()	//	This item's UI display offset
		{
			return new Position(0, 0, 0);
		}

		public virtual Position getDisplayRotation()	//	This item's UI display rotation
		{
			return new Position(0, 0, 0);
		}

		public virtual Position getDisplayScale()	//	This item's UI display scale
		{
			return new Position(1, 1, 1);
		}

		public virtual bool hasDurability()	//	Whether this item has durability
		{
			return false;
		}

		public virtual int getMaxDurability()	//	The max durability of this item
		{
			return 1;
		}

		public virtual int getMaxStackSize()	//	The maximum amount this item can stack
		{
			return 1;
		}

		public virtual bool isValid()
		{
			return (amount > 0 && durability > 0 && getType() != ItemType.VOID);
		}

		public virtual Item copyOf()
		{
			Item item = getType().toItem();
			return item;
		}

		public virtual Item copy()
		{
			Item item = getType().toItem();
			item.setAmount(getAmount());
			item.setDurability(getDurability());
			item.setItemData(getItemData());
			return item;
		}

	//	Immutable methods

		public void setItemData(ItemData _itemData)
		{
			itemData = _itemData;
		}

		public ItemData getItemData()
		{
			return itemData;
		}

		public void setDurability(int _durability)
		{
			if (hasDurability() == true) { durability = _durability; }
		}

		public int getDurability()
		{
			return durability;
		}

		public void damage(int _amount)
		{
			if (hasDurability() == true) { durability -= _amount; }
		}

		public void repair(int _amount)
		{
			if (hasDurability() == true)
			{
				durability += _amount;
				if (durability > getMaxDurability()) { durability = getMaxDurability(); }
				else if (durability < 0) { durability = 0; }
			}
		}

		public int setAmount(int _amount)
		{
			amount = _amount;
			if (amount >= getMaxStackSize()) { int overflow = amount - getMaxStackSize(); amount = getMaxStackSize(); return overflow; }

			return 0;
		}

		public int getAmount()
		{
			return amount;
		}

		public int modifyAmount(int _modifier)
		{
			amount += _modifier;
			if (amount >= getMaxStackSize()) { int overflow = amount - getMaxStackSize(); amount = getMaxStackSize(); return overflow; }
			else if (amount < 0) { return Math.Abs(amount); }

			return 0;
		}
	}
}
}