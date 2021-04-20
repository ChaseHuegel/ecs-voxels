using System;

namespace Swordfish
{
	public enum RenderState
	{
		Default,
		Rendering,
		Rendered,
		Waiting,
		Stopping,
		Stopped,
		Ready,
		Failed,
		Queued
	}
}