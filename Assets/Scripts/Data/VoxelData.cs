using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData {
	public byte id;
	[System.NonSerialized] private byte _light;

	public ChunkData chunkData;
	public VoxelNeighbours neighbours;
	public Vector3Int lPosition;

	public Vector3Int[] faceCheck ={
		Vector3Int.forward,	// 0 Front
		Vector3Int.up,		// 1 Top
		Vector3Int.right,	// 2 Right
		Vector3Int.left,	// 3 Left
		Vector3Int.down,	// 4 Bottom
		Vector3Int.back		// 5 Back}
		};

	public int[] inverseFaceCheck = { 5, 4, 3, 2, 1, 0 };

	public Vector3Int gPosition {
		get {
			return new Vector3Int( lPosition.x + chunkData.gPosition.x, lPosition.y, lPosition.z + chunkData.gPosition.y );
		}
	}

	public VoxelData( byte _id, ChunkData _chunkData, Vector3Int _localPosition ) {
		id = _id;
		lPosition = _localPosition;
		chunkData = _chunkData;
		neighbours = new VoxelNeighbours( this );
	}

	public BlockData properties {
		get { return World.Instance.worldData.blocks[ id ]; }
	}

	public byte light {
		get { return _light; }
		set {
			if ( value != _light ) {
				byte oldLightValue = _light;
				byte oldCastValue = castLight;

				_light = value;

				if ( _light < oldLightValue ) {
					List<int> neigboursToDarken = new List<int>();

					for ( int p = 0; p < 6; p++ ) {
						if ( neighbours[ p ] != null ) {
							if ( neighbours[ p ].light <= oldCastValue )
								neigboursToDarken.Add( p );
							else
								neighbours[ p ].PropogateLight();
						}
					}

					foreach ( int i in neigboursToDarken )
						neighbours[ i ].light = 0;

					if ( chunkData.chunk != null )
						World.Instance.AddChunkToUpdate( chunkData.chunk );
				} else if ( _light > 1 )
					PropogateLight();
			}
		}
	}

	public float lightAsFloat {
		get { return ( float )light * WorldData.unitOfLight; }
	}

	public byte castLight {
		get {
			int lightLevel = _light - properties.opacityValue - 1;
			if ( lightLevel < 0 )
				lightLevel = 0;
			return ( byte )lightLevel;
		}
	}

	public void PropogateLight() {
		if ( light < 2 )
			return;

		for ( int i = 0; i < 6; i++ ) {
			if ( neighbours[ i ] != null ) {
				if ( neighbours[ i ].light < castLight )
					neighbours[ i ].light = castLight;
			}

			if ( chunkData.chunk != null )
				World.Instance.AddChunkToUpdate( chunkData.chunk );
		}
	}
}

public class VoxelNeighbours {
	public VoxelData voxel;

	public VoxelNeighbours( VoxelData _voxel ) {
		voxel = _voxel;
	}

	private VoxelData[] _neighbours = new VoxelData[ 6 ];
	public int Length { get { return _neighbours.Length; } }

	public VoxelData this[ int index ] {
		get {
			if ( _neighbours[ index ] == null ) {
				_neighbours[ index ] = World.Instance.worldData.GetVoxelData( voxel.gPosition + voxel.faceCheck[ index ] );
				ReturnNeighbour( index );
			}
			return _neighbours[ index ];
		}
		set {
			_neighbours[ index ] = value;
			ReturnNeighbour( index );
		}
	}

	private void ReturnNeighbour( int index ) {
		if ( _neighbours[ index ] == null )
			return;

		if ( _neighbours[ index ].neighbours[ voxel.inverseFaceCheck[ index ] ] != voxel )
			_neighbours[ index ].neighbours[ voxel.inverseFaceCheck[ index ] ] = voxel;
	}
}