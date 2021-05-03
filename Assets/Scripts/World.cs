using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
	private static World _instance;
	public static World Instance => _instance;

	[Range( 0f, 1f )]
	public float globalLightLevel;

	public Color day;
	public Color night;

	public Transform player;
	private Vector3 spawnPoint;
	public ChunkCoord playerChunkCoord;
	private ChunkCoord playerLastChunkCoord;

	public WorldData worldData;
	public GameAssetsData gameAssetsData;

	private List<ChunkCoord> activeChunks = new List<ChunkCoord>();
	private List<Chunk> chunksToUpdate = new List<Chunk>();
	public Queue<Chunk> chunksToDraw = new Queue<Chunk>();

	private bool applyingModifications = false;
	private Queue<Queue<VoxelMod>> modifications = new Queue<Queue<VoxelMod>>();

	private int loadDistance;
	private int viewDistance = 3;

	public bool isWorldLoaded = false;

	private void Awake() {
		if ( _instance != null && _instance != this )
			Destroy( this.gameObject );
		else
			_instance = this;

		worldData = new WorldData( gameAssetsData.worldName, gameAssetsData.seed );

		loadDistance = viewDistance + 1;
	}

	private void Start() {
		Shader.SetGlobalFloat( "minGlobalLightLevel", WorldData.lightLevel.x );
		Shader.SetGlobalFloat( "maxGlobalLightLevel", WorldData.lightLevel.y );
		globalLightLevel = .9f;
		SetGlobalLightValue();

		LoadChunks( worldData.worldCenter );

		SetPlayer();

		isWorldLoaded = true;
	}

	private void SetPlayer() {
		spawnPoint = worldData.worldCenter + new Vector3( Random.Range( -8, 8 ), WorldData.chunkHeight + 50, Random.Range( -8, 8 ) );
		player.position = spawnPoint;

		playerLastChunkCoord = worldData.GetChunkCoord( player.position );
	}

	public void SetGlobalLightValue() {
		Shader.SetGlobalFloat( "GlobalLightLevel", globalLightLevel );
		Camera.main.backgroundColor = Color.Lerp( night, day, globalLightLevel );
	}

	// Update is called once per frame
	private void Update() {
		DayNightCycle();

		playerChunkCoord = worldData.GetChunkCoord( player.position );

		if ( !playerChunkCoord.CompareCoords( playerLastChunkCoord ) )
			LoadChunks( player.position );

		if ( chunksToDraw.Count > 0 )
			chunksToDraw.Dequeue().CreateMesh();

		if ( !applyingModifications )
			ApplyModifications();

		if ( chunksToUpdate.Count > 0 )
			UpdateChunks();
	}

	public void AddChunkToUpdate( Chunk chunk ) {
		AddChunkToUpdate( chunk, false );
	}

	public void DayNightCycle() {
		GameTime.Tick();
		Debug.Log( GameTime.hours + ":" + GameTime.minutes );
		if ( GameTime.ticksPassed >= 16200 && GameTime.ticksPassed <= 66600 && globalLightLevel <= .9 )
			globalLightLevel += .0005f;
		if ( GameTime.ticksPassed >= 66600 && globalLightLevel >= .05 )
			globalLightLevel -= .0005f;

		SetGlobalLightValue();
	}

	public void AddChunkToUpdate( Chunk chunk, bool insert ) {
		if ( !chunksToUpdate.Contains( chunk ) ) {
			if ( insert )
				chunksToUpdate.Insert( 0, chunk );
			else
				chunksToUpdate.Add( chunk );
		}
	}

	private void ApplyModifications() {
		applyingModifications = true;

		while ( modifications.Count > 0 ) {
			Queue<VoxelMod> queue = modifications.Dequeue();

			while ( queue.Count > 0 ) {
				VoxelMod v = queue.Dequeue();

				worldData.SetVoxel( v.position, v.id, 1 );
			}
		}

		applyingModifications = false;
	}

	private void LoadChunks( Vector3 loadPosition ) {
		ChunkCoord coord = worldData.GetChunkCoord( loadPosition );
		playerLastChunkCoord = playerChunkCoord;

		List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>( activeChunks );

		activeChunks.Clear();

		// Loop through all chunks currently within load distance of the player.
		for ( int x = coord.x - loadDistance; x < coord.x + loadDistance + 1; x++ ) {
			for ( int z = coord.z - loadDistance; z < coord.z + loadDistance + 1; z++ ) {
				ChunkCoord thisChunkCoord = new ChunkCoord( x, z );

				// If the current chunk is in the world...
				if ( worldData.IsChunkInWorld( thisChunkCoord ) ) {
					// Check if it active, if not, activate it.
					if ( worldData.chunkMap[ x, z ] == null ) {
						worldData.chunkMap[ x, z ] = new Chunk( thisChunkCoord );
						worldData.LoadChunk( new Vector2Int( x, z ) );
					}
					//displays chunks in view
					else if ( x >= coord.x - viewDistance && x < coord.x + viewDistance + 1 && z >= coord.z - viewDistance && z < coord.z + viewDistance + 1 ) {
						worldData.chunkMap[ x, z ].isActive = true;
						activeChunks.Add( thisChunkCoord );
					} else {
						worldData.chunkMap[ x, z ].isActive = false;
					}
				}
				// Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
				for ( int i = 0; i < previouslyActiveChunks.Count; i++ ) {
					if ( previouslyActiveChunks[ i ].CompareCoords( thisChunkCoord ) )
						previouslyActiveChunks.RemoveAt( i );
				}
			}
		}
	}

	private void UpdateChunks() {
		chunksToUpdate[ 0 ].UpdateChunkData();
		if ( !activeChunks.Contains( chunksToUpdate[ 0 ].chunkData.coord ) )
			activeChunks.Add( chunksToUpdate[ 0 ].chunkData.coord );
		chunksToUpdate.RemoveAt( 0 );
	}

	public byte GetVoxel( Vector3 globalPos ) {
		int yPos = Mathf.FloorToInt( globalPos.y );

		/* IMMUTABLE PASS */

		// If outside world, return air.
		if ( !worldData.IsVoxelInWorld( globalPos ) )
			return 0;

		// If bottom block of chunk, return bedrock.
		if ( yPos == 0 )
			return 1;

		/* BIOME SELECTION PASS*/

		int solidGroundHeight = 42;
		float sumOfHeights = 0f;
		int count = 0;
		float strongestWeight = 0f;
		int strongestBiomeIndex = 0;

		for ( int i = 0; i < worldData.biomes.Length; i++ ) {
			float weight = Noise.Get2DPerlin( new Vector2( globalPos.x, globalPos.z ), worldData.biomes[ i ].nosieOffset, worldData.biomes[ i ].nosieScale );

			// Keep track of which weight is strongest.
			if ( weight > strongestWeight ) {
				strongestWeight = weight;
				strongestBiomeIndex = i;
			}

			// Get the height of the terrain (for the current biome) and multiply it by its weight.
			float height = worldData.biomes[ i ].terrainHeight * Noise.Get2DPerlin( new Vector2( globalPos.x, globalPos.z ), 0, worldData.biomes[ i ].terrainScale ) * weight;

			// If the height value is greater 0 add it to the sum of heights.
			if ( height > 0 ) {
				sumOfHeights += height;
				count++;
			}
		}

		// Set biome to the one with the strongest weight.
		Biome biome = worldData.biomes[ strongestBiomeIndex ];

		// Get the average of the heights.
		sumOfHeights /= count;

		int terrainHeight = Mathf.FloorToInt( sumOfHeights + solidGroundHeight );

		/* BASIC TERRAIN PASS */

		byte voxelID = 0;

		if ( yPos == terrainHeight )
			voxelID = biome.surfaceBlock;
		else if ( yPos < terrainHeight && yPos > terrainHeight - 4 )
			voxelID = biome.subSurfaceBlock;
		else if ( yPos > terrainHeight )
			return 0;
		else
			voxelID = 2;

		/* SECOND PASS */

		if ( voxelID == 2 ) {
			foreach ( DepositData deposit in biome.deposits ) {
				if ( yPos > deposit.height.x && yPos < deposit.height.y )
					if ( Noise.Get3DPerlin( globalPos, deposit.noiseOffset, deposit.noiseScale, deposit.noiseThreshold ) )
						voxelID = deposit.blockID;
			}
		}

		/* TREE PASS */

		if ( yPos == terrainHeight && biome.placeMajorFlora ) {
			if ( Noise.Get2DPerlin( new Vector2( globalPos.x, globalPos.z ), 0, biome.majorFloraZoneScale ) > biome.majorFloraZoneThreshold ) {
				if ( Noise.Get2DPerlin( new Vector2( globalPos.x, globalPos.z ), 0, biome.majorFloraPlacementScale ) > biome.majorFloraPlacementThreshold ) {
					modifications.Enqueue( Structures.GenerateMajorFlora( biome.majorFloraIndex, globalPos, biome.height.x, biome.height.y ) );
				}
			}
		}

		return voxelID;
	}
}

public class VoxelMod {
	public Vector3 position;
	public byte id;

	public VoxelMod() {
		position = new Vector3();
		id = 0;
	}

	public VoxelMod( Vector3 _position, byte _id ) {
		position = _position;
		id = _id;
	}
}

[System.Serializable]
public class GameAssetsData {
	public BlockTypeData[] blockTypes;
	public Item[] items;
	public Biome[] biomes;
	public Material[] materials;
	public int seed;
	public string worldName;
}