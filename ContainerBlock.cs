using System;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class ContainerBlock : BehaviorBlock
    {
		protected Inventory inventory = new Inventory();

		public Inventory getInventory()
		{
			return inventory;
		}

		public void setInventory(Inventory _inventory)
		{
			inventory = _inventory;
		}

		public void setSize(int _size)
		{
			inventory.setSize(_size);
		}

		public int getSize()
		{
			return inventory.getSize();
		}
	}
}