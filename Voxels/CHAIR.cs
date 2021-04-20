using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class CHAIR : BehaviorBlock
    {
		public CHAIR()
		{
			blockData = new Rotatable();
		}

		public override Voxel getType()
		{
			return Voxel.CHAIR;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override ModelType getModelType()
		{
			return ModelType.CUSTOM;
		}

		public override CachedModel getModelData()
		{
			return GameMaster.Instance.getCachedModel("chair");
		}

		public override Position getOffset()
		{
			return new Position(0.5f, 0.5f, 0.5f);
		}
	}
}