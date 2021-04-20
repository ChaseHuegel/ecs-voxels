using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class THRUSTER_ROCKET_INTERNAL : THRUSTER_ROCKET
    {
		public override Voxel getType()
		{
			return Voxel.THRUSTER_ROCKET_INTERNAL;
		}

		public override bool isTransparent()
		{
			return false;
		}

		public override CachedModel getModelData()
		{
			return GameMaster.Instance.getCachedModel("thruster_rocket_internal");
		}
	}
}