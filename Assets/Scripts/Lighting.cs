using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Lighting {

	public static void RecalculateNaturaLight( ChunkData chunkData ) {
		for ( int x = 0; x < WorldData.chunkWidth; x++ ) {
			for ( int z = 0; z < WorldData.chunkWidth; z++ ) {
				CastNaturalLight( chunkData, x, z, WorldData.chunkHeight - 1 );
			}
		}
	}

	public static void CastNaturalLight( ChunkData chunkData, int x, int z, int startY ) {
		if ( startY > WorldData.chunkHeight - 1 ) {
			startY = WorldData.chunkHeight - 1;
			Debug.LogWarning( "Attempted to cast natural light out of world." );
		}

		bool obstructed = false;

		for ( int y = startY; y > -1; y-- ) {
			VoxelData voxel = chunkData.voxelMap[ x, y, z ];

			if ( obstructed ) {
				voxel.Light = 0;
			} else if ( voxel.Properties.opacityValue > 0 ) {
				voxel.Light = 0;
				obstructed = true;
			} else
				voxel.Light = 15;
		}
	}
}

public static class GameTime {
	private static readonly float tickInterval = 1 / 60f * Time.deltaTime;
	private static readonly int maxTicksPerDay = 86400;
	private static float timer = 0;
	private static int tickCount = 0; // 8:15 AM 29700
	public static int Minutes { get { return tickCount % 3600 / 60; } }

	public static int Hours {
		get {
			return ( tickCount / 3600 >= 12 ) ? tickCount / 3600 - 12 : tickCount / 3600;
		}
	}

	public static int TicksPassed { get { return tickCount; } }

	public static void Tick() {
		timer += Time.deltaTime;

		if ( timer >= tickInterval )
			tickCount++;

		if ( tickCount >= maxTicksPerDay )
			tickCount = 0;
	}
}