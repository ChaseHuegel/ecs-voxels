using System;

namespace Swordfish
{
namespace items
{
    public class Tool : Item
    {
        public override Position getViewOffset()
		{
			return new Position(0, 0, 0);
		}

		public override Position getViewRotation()
		{
			return new Position(6.0f, 85.0f, -0.5f);
		}

		public override Position getViewScale()
		{
			return new Position(2, 2, 2);
		}

		public override Position getDisplayScale()
		{
			return new Position(1.5f, 1.5f, 1.5f);
		}
	}
}
}