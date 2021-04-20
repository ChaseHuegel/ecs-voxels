using System;

namespace Swordfish
{
	public enum ChunkState
	{
		Default,
		Unregistered,
		Loaded,
		Unloaded,
		Loading,
		Unloading,
		Ready,
		Waiting,
		Stopped,
		Stopping
	}
}