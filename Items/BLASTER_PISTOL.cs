using System;

namespace Swordfish
{
namespace items
{
    public class BLASTER_PISTOL : Tool
    {
		public override ItemType getType()
		{
			return ItemType.BLASTER_PISTOL;
		}

		public override string getName()
		{
			return "Blaster Pistol";
		}

		public override CachedImage getImageData()
		{
			return ResourceManager.GetImage("item.blaster_pistol").image;
		}
	}
}
}