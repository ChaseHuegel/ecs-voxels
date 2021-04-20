using System;

namespace Swordfish
{
namespace items
{
	public enum ItemType
	{
		VOID,
		DRILL,
		CUTTER,
		WELDER,
		BLASTER_PISTOL,
		BLOCKITEM
	}

	public static class ItemTypeExtensions
	{
		public static Item toItem(this ItemType _type)
		{
			switch (_type)
			{
				case ItemType.VOID:
					return new VOID();

				case ItemType.DRILL:
					return new DRILL();

				case ItemType.CUTTER:
					return new CUTTER();

				case ItemType.WELDER:
					return new WELDER();

				case ItemType.BLASTER_PISTOL:
					return new BLASTER_PISTOL();

				case ItemType.BLOCKITEM:
					return new BLOCKITEM();

				default:
					return new VOID();
			}
		}
	}
}
}
