using System;

namespace Swordfish
{
namespace items
{
    public class CUTTER : Tool
    {
		public override ItemType getType()
		{
			return ItemType.CUTTER;
		}

		public override string getName()
		{
			return "Cutter";
		}

		public override CachedImage getImageData()
		{
			return ResourceManager.GetImage("item.cutter").image;
		}
	}
}
}