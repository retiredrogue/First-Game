using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData {
	private int x;
	private int z;

	public Chunk chunk;

	public ChunkCoord coord;

	private readonly VoxelData[,,] voxelMap = new VoxelData[ WorldData.chunkWidth, WorldData.chunkHeight, WorldData.chunkWidth ];

	public VoxelData GetVoxelData( int x, int y, int z ) => voxelMap[ x, y, z ];

	public VoxelData GetVoxelData( Vector3Int pos ) => voxelMap[ pos.x, pos.y, pos.z ];

	public ChunkData( Vector2Int _worldPosition ) {
		WorldPosition = _worldPosition;
	}

	public Vector2Int WorldPosition {//X Z PLAIN
		get { return new Vector2Int( x, z ); }
		set {
			x = value.x;
			z = value.y;
		}
	}

	public Chunk GetChunk() => chunk;

	public void PopulateVoxelMapData() {
		for ( int x = 0; x < WorldData.chunkWidth; x++ ) {
			for ( int y = 0; y < WorldData.chunkHeight; y++ ) {
				for ( int z = 0; z < WorldData.chunkWidth; z++ ) {
					Vector3 voxelPosition = new Vector3( x + WorldPosition.x, y, z + WorldPosition.y );
					VoxelData voxel = voxelMap[ x, y, z ] = new VoxelData( World.Instance.GenBlockData( voxelPosition ), this, new Vector3Int( x, y, z ) );

					for ( int i = 0; i < 6; i++ ) {
						Vector3Int neighbourPosition = voxel.GPosition + voxel.faceCheck[ i ];
						if ( IsVoxelInChunk( neighbourPosition ) )
							voxel.neighbours[ i ] = VoxelFromV3Int( neighbourPosition );
						else
							voxel.neighbours[ i ] = World.Instance.worldData.GetVoxelData( neighbourPosition );
					}
				}
			}
		}
		Lighting.RecalculateNaturaLight( this );

		World.Instance.worldData.AddToModifiedChunkList( this );
	}

	public void UpdateSurroundingVoxels( Vector3Int voxelMapPosition ) {
		VoxelData ChangedVoxel = voxelMap[ voxelMapPosition.x, voxelMapPosition.y, voxelMapPosition.z ];

		for ( int i = 0; i < 6; i++ ) {
			Vector3Int currentVoxel = voxelMapPosition + ChangedVoxel.faceCheck[ i ];

			if ( !IsVoxelInChunk( currentVoxel ) )
				World.Instance.AddChunkToUpdate( chunk, true );
		}
	}

	public void ModifyVoxel( Vector3Int voxelChunkPosition, byte _id, Vector3 rotation ) {
		if ( voxelMap[ voxelChunkPosition.x, voxelChunkPosition.y, voxelChunkPosition.z ].id == _id )
			return;

		VoxelData voxel = voxelMap[ voxelChunkPosition.x, voxelChunkPosition.y, voxelChunkPosition.z ];
		//BlockData newVoxel = World.Instance.worldData.blocks[ _id ];

		byte oldOpacity = voxel.Properties.GetOpacity();

		voxel.id = _id;
		if ( voxel.Properties.GetRotatable() )
			voxel.Properties.RotateFaces( rotation );

		if ( voxel.Properties.GetOpacity() != oldOpacity &&
			( voxelChunkPosition.y == WorldData.chunkHeight - 1 || voxelMap[ voxelChunkPosition.x, voxelChunkPosition.y + 1, voxelChunkPosition.z ].Light == 15 ) ) {
			Lighting.CastNaturalLight( this, voxelChunkPosition.x, voxelChunkPosition.z, voxelChunkPosition.y + 1 );
		}

		World.Instance.worldData.AddToModifiedChunkList( this );

		if ( chunk != null )
			World.Instance.AddChunkToUpdate( chunk );
	}

	public bool IsVoxelInChunk( int x, int y, int z ) => x >= 0 && x <= WorldData.chunkWidth - 1 && y >= 0 && y <= WorldData.chunkHeight - 1 && z >= 0 && z <= WorldData.chunkWidth - 1;

	public bool IsVoxelInChunk( Vector3Int pos ) => IsVoxelInChunk( pos.x, pos.y, pos.z );

	public VoxelData VoxelFromV3Int( Vector3Int pos ) => voxelMap[ pos.x, pos.y, pos.z ];
}