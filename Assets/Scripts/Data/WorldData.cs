using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldData {
	public string worldName;
	public int seed;

	public static int chunkWidth = 16;
	public static int chunkHeight = 128;
	public static int worldSizeInChunks = 10;
	public static int terrainHeight = chunkHeight / 2;

	public ItemData[] items;
	public Biome[] biomes;
	public Material[] materials;

	public static int textureAtlasSizeInBlocks = 16;

	public static int WorldSizeInVoxels {
		get { return worldSizeInChunks * chunkWidth; }
	}

	public static float NormalizedBlockTextureSize {
		get { return 1f / ( float )textureAtlasSizeInBlocks; }
	}

	public static Vector2 lightLevel = new Vector2( 0.1f, 0.9f );

	public static float UnitOfLight {
		get { return .0625f; }
	}

	public Chunk[,] chunkMap = new Chunk[ worldSizeInChunks, worldSizeInChunks ];
	public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();

	public List<ChunkData> modifiedChunks = new List<ChunkData>();

	public Vector3 worldCenter = new Vector3( ( worldSizeInChunks % 2f == 0 ) ? worldSizeInChunks / 2f + .5f : worldSizeInChunks / 2f, 0, ( worldSizeInChunks % 2f == 0 ) ? worldSizeInChunks / 2f + .5f : worldSizeInChunks / 2f ) * chunkWidth;

	public WorldData( string _worldName, int _seed ) {
		if ( _worldName == null || _worldName == "" )
			worldName = "New World";
		else
			worldName = _worldName;

		if ( _seed == 0 )
			seed = 1;// Random.Range( 1, 10000 );
		else
			seed = _seed;

		GetAssets();
	}

	public WorldData( WorldData wD ) {
		if ( wD.worldName == null || wD.worldName == "" )
			worldName = "New World";
		else
			worldName = wD.worldName;

		if ( wD.seed == 0 )
			seed = Random.Range( 1, 10000 );
		else
			seed = wD.seed;

		GetAssets();
	}

	public void GetAssets() {
		items = World.Instance.gameAssetsData.items;
		biomes = World.Instance.gameAssetsData.biomes;
		materials = World.Instance.gameAssetsData.materials;
	}

	public void AddToModifiedChunkList( ChunkData chunk ) {
		if ( !modifiedChunks.Contains( chunk ) )
			modifiedChunks.Add( chunk );
	}

	public void LoadChunk( Vector2Int coord ) {
		if ( chunks.ContainsKey( coord ) )
			return;

		chunks.Add( coord, new ChunkData( coord * chunkWidth ) );
		chunks[ coord ].PopulateVoxelMapData();
	}

	public bool IsVoxelInWorld( Vector3 pos ) => ( pos.x >= 0 && pos.x < WorldSizeInVoxels && pos.y >= 0 && pos.y < chunkHeight && pos.z >= 0 && pos.z < WorldSizeInVoxels );

	public VoxelData GetVoxelData( Vector3 voxelgPosition ) {
		if ( !IsVoxelInWorld( voxelgPosition ) )
			return null;

		ChunkData chunkData = RequestChunkData( GetChunkCoord( voxelgPosition ), false );

		if ( chunkData == null )
			return null;

		// voxel position in chunk via a remainder should be 0 - 15
		Vector3Int voxel = VoxelPositionInChunk( voxelgPosition );

		return chunkData.voxelMap[ voxel.x, voxel.y, voxel.z ];
	}

	public ChunkCoord GetChunkCoord( Vector3 chunkgPosition ) => GetChunkCoord( new Vector2( chunkgPosition.x, chunkgPosition.z ) );

	public ChunkCoord GetChunkCoord( Vector2 chunkgPosition ) {
		int x = Mathf.FloorToInt( chunkgPosition.x / chunkWidth );
		int z = Mathf.FloorToInt( chunkgPosition.y / chunkWidth );
		return new ChunkCoord( x, z );
	}

	public ChunkData RequestChunkData( ChunkCoord coord, bool create ) => RequestChunkData( new Vector2Int( coord.x, coord.z ), create );

	public ChunkData RequestChunkData( Vector2Int coord, bool create ) {
		ChunkData c;

		if ( chunks.ContainsKey( coord ) )
			c = chunks[ coord ];
		else if ( !create )
			c = null;
		else {
			LoadChunk( coord );
			c = chunks[ coord ];
		}

		return c;
	}

	public bool IsChunkInWorld( ChunkCoord coord ) => ( coord.x >= 0 && coord.x < worldSizeInChunks - 1 && coord.z >= 0 && coord.z < worldSizeInChunks - 1 );

	public Vector3Int VoxelPositionInChunk( Vector3 voxelPosition ) => new Vector3Int( ( int )voxelPosition.x % chunkWidth, ( int )voxelPosition.y, ( int )voxelPosition.z % chunkWidth );

	public void SetVoxel( Vector3 voxelgPosition, byte voxelID, int orientation ) {
		if ( !IsVoxelInWorld( voxelgPosition ) )
			return;

		ChunkData chunk = RequestChunkData( GetChunkCoord( voxelgPosition ), true );

		// voxel position in chunk via a remainder should be 0 - 15
		Vector3Int voxel = VoxelPositionInChunk( voxelgPosition );

		chunk.ModifyVoxel( voxel, voxelID, orientation );
	}

	public bool CheckForVoxel( Vector3 voxelgPosition ) {
		VoxelData voxel = GetVoxelData( voxelgPosition );
		return ( voxel != null && items[ voxel.id ].blockTypeInfo.canWalkOn );
	}

	public void EditVoxel( Vector3 voxelgPosition, byte newID, int orientation ) {
		Vector3Int voxel = VoxelPositionInChunk( voxelgPosition );
		ChunkData chunkData = RequestChunkData( GetChunkCoord( voxelgPosition ), false );

		chunkData.ModifyVoxel( voxel, newID, orientation );

		chunkData.UpdateSurroundingVoxels( voxel );
	}

	public void EditVoxel( Vector3 voxelgPosition, byte newID ) {
		Vector3Int voxel = VoxelPositionInChunk( voxelgPosition );
		ChunkData chunkData = RequestChunkData( GetChunkCoord( voxelgPosition ), false );

		chunkData.ModifyVoxel( voxel, newID, 0 );

		chunkData.UpdateSurroundingVoxels( voxel );
	}
}