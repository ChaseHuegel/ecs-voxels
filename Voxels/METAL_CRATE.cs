using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class METAL_CRATE : ContainerBlock
    {
		public METAL_CRATE()
		{
			blockData = new Rotatable();
		}

		public override Voxel getType()
		{
			return Voxel.METAL_CRATE;
		}

		public override bool isTransparent()
		{
			return true;
		}

		public override ModelType getModelType()
		{
			return ModelType.CUSTOM_CUBE;
		}

		public override Coord2D getTexture(Direction _face)
		{
			switch (_face)
			{
				case Direction.ABOVE:
					return new Coord2D(11, 2);

				case Direction.BELOW:
					return new Coord2D(12, 2);

				case Direction.NORTH:
					return new Coord2D(11, 3);

				case Direction.SOUTH:
					return new Coord2D(12, 3);

				default:
					return new Coord2D(13, 3);
			}
		}

		public override void Update()
		{

		}

		public override void Initialize()
		{

		}

		public override void OnRemove()
		{

		}

		public override void Interact()
		{

		}
	}
}