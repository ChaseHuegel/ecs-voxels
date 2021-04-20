
using System;

namespace Swordfish
{
	public enum CollisionState
	{
		Default,
		Building,
		Built,
		Waiting,
		Stopping,
		Stopped,
		Ready,
		Failed,
		Queued
	}
}