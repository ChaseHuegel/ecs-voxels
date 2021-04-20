using System;

namespace Swordfish
{
namespace items
{
    public class WELDER : Tool
    {
		public override ItemType getType()
		{
			return ItemType.WELDER;
		}

		public override string getName()
		{
			return "Welder";
		}

		public override CachedImage getImageData()
		{
			return ResourceManager.GetImage("item.welder").image;
		}
	}
}
}