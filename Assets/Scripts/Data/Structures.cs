using System.Collections.Generic;
using UnityEngine;

public static class Structures {

	public static Queue<VoxelMod> GenerateMajorFlora( int index, Vector3 gPosition, int minStemHeight, int maxStemHeight ) {
		switch ( index ) {
			case 0:
				return MakeTree( gPosition, minStemHeight, maxStemHeight );

			case 1:
				return MakeCacti( gPosition, minStemHeight, maxStemHeight );

			default:
				break;
		}

		return new Queue<VoxelMod>();
	}

	public static Queue<VoxelMod> MakeTree( Vector3 gPosition, int minStemHeight, int maxStemHeight ) {
		Queue<VoxelMod> queue = new Queue<VoxelMod>();
		int height = ( int )( maxStemHeight * noise2.Get2DPerlin( new Vector2( gPosition.x, gPosition.z ), 250f, 3f ) );

		if ( height < minStemHeight )
			height = minStemHeight;

		// generate trunk
		for ( int i = 1; i < height; i++ )
			queue.Enqueue( new VoxelMod( new Vector3( gPosition.x, gPosition.y + i, gPosition.z ), 7 ) );

		//generate tree top
		for ( int y = -2; y < 4; y++ ) {
			for ( int x = -2; x < 3; x++ ) {
				for ( int z = -2; z < 3; z++ ) {
					Vector3 pos = new Vector3( gPosition.x + x, gPosition.y + height + y, gPosition.z + z );
					byte voxelID = World.Instance.GenBlockData( pos );
					if ( voxelID == 0 )
						queue.Enqueue( new VoxelMod( pos, 8 ) );
				}
			}
		}

		return queue;
	}

	public static Queue<VoxelMod> MakeCacti( Vector3 gPosition, int minStemHeight, int maxStemHeight ) {
		Queue<VoxelMod> queue = new Queue<VoxelMod>();

		int height = ( int )( maxStemHeight * noise2.Get2DPerlin( new Vector2( gPosition.x, gPosition.z ), 23456f, 2f ) );

		if ( height < minStemHeight )
			height = minStemHeight;

		for ( int i = 1; i <= height; i++ ) {
			queue.Enqueue( new VoxelMod( new Vector3( gPosition.x, gPosition.y + i, gPosition.z ), 9 ) );
		}
		return queue;
	}
}