using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class STANDING_CONSOLE : CONSOLE
    {
		public override Voxel getType()
		{
			return Voxel.STANDING_CONSOLE;
		}

		public override CachedModel getModelData()
		{
			return GameMaster.Instance.getCachedModel("console2");
		}
	}
}