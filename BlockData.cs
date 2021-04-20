using System;
using UnityEngine;

namespace Swordfish
{
	public class BlockData
	{
		public Color color = Color.white;

		public BlockData() {}
	}

	public class Flippable : BlockData
	{
		private bool flipped = false;

		public Flippable() {}

		public virtual bool canFlip()
		{
			return true;
		}

		public bool isFlipped()
		{
			if (canFlip() == false) { return false; }
			return flipped;
		}

		public void setFlipped(bool _flipped)
		{
			if (canFlip() == false && _flipped) { return; }
			flipped = _flipped;
		}

		public void Flip()
		{
			if (canFlip()) { flipped = !flipped; }
		}
	}

	public class Rotatable : Flippable
	{
		private Direction facing = Direction.NORTH;

		public Rotatable() {}

		public override bool canFlip()
		{
			return true;
		}

		public Direction getDirection()
		{
			return facing;
		}

		public virtual Direction getFacing()
		{
			Quaternion rotation = new Quaternion();
			rotation.eulerAngles = getRotation();
			Vector3 vector = rotation * Vector3.forward;
			return Util.DirectionFromVector3( vector.normalized );
		}

		public void setDirection(Direction _facing)
		{
			facing = _facing;
		}

		public virtual Vector3 getRotation()
		{
			Vector3 rotation = new Vector3(0, 0, 0);

			switch (facing)
			{
			case Direction.NORTH:
				break;
			case Direction.EAST:
				rotation = new Vector3(0, 90, 0);
				break;
			case Direction.SOUTH:
				rotation = new Vector3(0, 180, 0);
				break;
			case Direction.WEST:
				rotation = new Vector3(0, 270, 0);
				break;
			case Direction.ABOVE:
				rotation = new Vector3(-90, 0, 0);
				break;
			case Direction.BELOW:
				rotation = new Vector3(90, 0, 0);
				break;
			}

			if ( isFlipped() )
			{
				rotation = new Vector3(rotation.x + 180, rotation.y + 180, rotation.z);
			}

			return rotation;
		}
	}

	public class Orientated : Rotatable
	{
		private Direction orientation = Direction.NORTH;

		public Orientated() {}

		public override Vector3 getRotation()
		{
			Vector3 rotation = base.getRotation();

			switch (orientation)
			{
			case Direction.NORTH:
				rotation = new Vector3(rotation.x, rotation.y, rotation.z + 90);
				break;
			case Direction.EAST:
				rotation = new Vector3(rotation.x, rotation.y + 90, rotation.z + 90);
				break;
			case Direction.SOUTH:
				rotation = new Vector3(rotation.x, rotation.y + 180, rotation.z - 90);
				break;
			case Direction.WEST:
				rotation = new Vector3(rotation.x, rotation.y - 90, rotation.z - 90);
				break;
			case Direction.ABOVE:
				rotation = new Vector3(rotation.x - 90, rotation.y, rotation.z + 90);
				break;
			case Direction.BELOW:
				rotation = new Vector3(rotation.x + 90, rotation.y + 180, rotation.z - 90);
				break;
			}

			return rotation;
		}

		public override bool canFlip()
		{
			return false;
		}

		public Direction getOrientation()
		{
			return orientation;
		}

		public void setOrientation(Direction _orientation)
		{
			orientation = _orientation;
		}
	}

	public class Thruster : Orientated
	{
		public bool active = false;

		public Thruster() {}

		public void setActive(bool _active)
		{
			active = _active;
		}

		public virtual float getPower()
		{
			return 10.0f;
		}
	}

	public class RocketThruster : Thruster
	{
		public RocketThruster() {}

		public override float getPower()
		{
			return 100.0f;
		}
	}
}