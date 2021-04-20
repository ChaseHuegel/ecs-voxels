using System;

namespace Swordfish
{
    public enum Voxel
    {
        VOID,
		SHIP_CORE,
		FRAME,
        METAL_PANEL,
		METAL_PORTHOLE,
		METAL_GRATING,
		SHIP_MONITOR,
		CONTROL_PANEL,
		THRUSTER_ROCKET,
		THRUSTER_ROCKET_INTERNAL,
		METAL_BARS,
		CAUTION_PANEL,
		CONSOLE,
		STANDING_CONSOLE,
		METAL_WINDOW,
		LIGHT_BLOCK,
		LIGHT_PANEL,
		LIGHT_BULB,
		METAL_PANEL_SLOPE,
		CAUTION_PANEL_SLOPE,
		METAL_PORTHOLE_SLOPE,
		METAL_WINDOW_SLOPE,
		METAL_GRATING_SLOPE,
		METAL_BARS_SLOPE,
		ASTEROID_ROCK,
		ASTEROID_ICE,
		SWITCH_PANEL,
		INTERFACE_PANEL,
		DISPLAY_PANEL,
		DIAGNOSTIC_PANEL,
		METAL_SLAB,
		METAL_CRATE,
		CHAIR,
		VENT_PANEL,
		DECORATED_PANEL,
		GRASS_BLOCK,
		DIRT
    }

	public static class VoxelExtensions
	{
		public static items.Item toItem(this Voxel _voxel)
		{
			return new items.BLOCKITEM(_voxel);
		}

		public static Block toBlock(this Voxel _voxel)
		{
			switch (_voxel)
			{
				case Voxel.VOID:
					return new VOID();

				case Voxel.FRAME:
					return new FRAME();

				case Voxel.METAL_PANEL:
					return new METAL_PANEL();

				case Voxel.SHIP_CORE:
					return new SHIP_CORE();

				case Voxel.METAL_PORTHOLE:
					return new METAL_PORTHOLE();

				case Voxel.METAL_GRATING:
					return new METAL_GRATING();

				case Voxel.SHIP_MONITOR:
					return new SHIP_MONITOR();

				case Voxel.CONTROL_PANEL:
					return new CONTROL_PANEL();

				case Voxel.THRUSTER_ROCKET:
					return new THRUSTER_ROCKET();

				case Voxel.THRUSTER_ROCKET_INTERNAL:
					return new THRUSTER_ROCKET_INTERNAL();

				case Voxel.METAL_BARS:
					return new METAL_BARS();

				case Voxel.CAUTION_PANEL:
					return new CAUTION_PANEL();

				case Voxel.CONSOLE:
					return new CONSOLE();

				case Voxel.STANDING_CONSOLE:
					return new STANDING_CONSOLE();

				case Voxel.METAL_WINDOW:
					return new METAL_WINDOW();

				case Voxel.LIGHT_BLOCK:
					return new LIGHT_BLOCK();

				case Voxel.LIGHT_PANEL:
					return new LIGHT_PANEL();

				case Voxel.LIGHT_BULB:
					return new LIGHT_BULB();

				case Voxel.METAL_PANEL_SLOPE:
					return new METAL_PANEL_SLOPE();

				case Voxel.CAUTION_PANEL_SLOPE:
					return new CAUTION_PANEL_SLOPE();

				case Voxel.METAL_PORTHOLE_SLOPE:
					return new METAL_PORTHOLE_SLOPE();

				case Voxel.METAL_WINDOW_SLOPE:
					return new METAL_WINDOW_SLOPE();

				case Voxel.METAL_GRATING_SLOPE:
					return new METAL_GRATING_SLOPE();

				case Voxel.METAL_BARS_SLOPE:
					return new METAL_BARS_SLOPE();

				case Voxel.ASTEROID_ROCK:
					return new ASTEROID_ROCK();

				case Voxel.ASTEROID_ICE:
					return new ASTEROID_ICE();

				case Voxel.SWITCH_PANEL:
					return new SWITCH_PANEL();

				case Voxel.INTERFACE_PANEL:
					return new INTERFACE_PANEL();

				case Voxel.DISPLAY_PANEL:
					return new DISPLAY_PANEL();

				case Voxel.DIAGNOSTIC_PANEL:
					return new DIAGNOSTIC_PANEL();

				case Voxel.METAL_SLAB:
					return new METAL_SLAB();

				case Voxel.METAL_CRATE:
					return new METAL_CRATE();

				case Voxel.CHAIR:
					return new CHAIR();

				case Voxel.VENT_PANEL:
					return new VENT_PANEL();

				case Voxel.DECORATED_PANEL:
					return new DECORATED_PANEL();

				case Voxel.GRASS_BLOCK:
					return new GRASS_BLOCK();

				case Voxel.DIRT:
					return new DIRT();

				default:
					return new VOID();
			}
		}
	}
}
