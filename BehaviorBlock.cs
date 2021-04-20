using System;

namespace Swordfish
{
	public class BehaviorBlock : Block
	{
		public BehaviorBlock() {}

		public virtual bool isTickable()	//	Whether this block can be ticked
		{
			return true;
		}

		public virtual bool isInteractable()	// Whether this block can be interacted with by a player
		{
			return false;
		}

		public virtual void Initialize()
		{

		}

		public virtual void Tick()
		{

		}

		public virtual void Interact()
		{

		}
	}
}