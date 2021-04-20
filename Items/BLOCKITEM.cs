using System;

namespace Swordfish
{
namespace items
{
    public class BLOCKITEM : Item
    {
		private Voxel voxel;

		public override bool Equals(object value)
		{
			if(value == null || (value is BLOCKITEM) == false )
			{
				return false;
			}

			BLOCKITEM item = value as BLOCKITEM;

			return ( item.getVoxel() == this.getVoxel() );
		}

		public BLOCKITEM()
		{
			voxel = Voxel.VOID;
			itemData = new ItemData();
		}

		public BLOCKITEM(Voxel _voxel)
		{
			voxel = _voxel;
			itemData = new ItemData();
		}

		public override ItemType getType()
		{
			return ItemType.BLOCKITEM;
		}

		public override string getName()
		{
			name = getVoxel().ToString();
			return name;
		}

		public override CachedImage getImageData()
		{
			return GameMaster.Instance.getCachedImage(getVoxel().ToString());
		}

		public override int getMaxStackSize()
		{
			return 100;
		}

		public override Position getViewRotation()
		{
			return new Position(15.0f, 45.0f, 0.0f);
		}

		public override bool isValid()
		{
			return (amount > 0 && durability > 0 && getVoxel() != Voxel.VOID);
		}

		public override Item copyOf()
		{
			BLOCKITEM item = (BLOCKITEM)base.copyOf();
			item.setVoxel(getVoxel());
			return item;
		}

		public override Item copy()
		{
			BLOCKITEM item = (BLOCKITEM)base.copy();
			item.setVoxel(getVoxel());
			return item;
		}

		public Voxel getVoxel()
		{
			return voxel;
		}

		public BLOCKITEM setVoxel(Voxel _voxel)
		{
			voxel = _voxel;
			return this;
		}
	}
}
}