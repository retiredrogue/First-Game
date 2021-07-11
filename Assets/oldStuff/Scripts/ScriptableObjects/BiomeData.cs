using UnityEngine;

[CreateAssetMenu( fileName = "New Biome", menuName = "ScriptableObjects/Biome Data" )]
public class BiomeData : ScriptableObject {

	[Header( "Major Flora" )]
	public int nosieOffset;

	public float nosieScale;

	public int terrainHeight;
	public float terrainScale;

	public byte surfaceBlock;
	public byte subSurfaceBlock;

	[Header( "Major Flora Structure" )]
	public int majorFloraIndex;

	public float majorFloraZoneScale;

	[Range( 0.1f, 1f )]
	public float majorFloraZoneThreshold;

	public float majorFloraPlacementScale;

	[Range( 0.1f, 1f )]
	public float majorFloraPlacementThreshold;

	public bool placeMajorFlora;

	[Tooltip( "X: Min, Y: Max" )]
	public Vector2Int height;

	public DepositData[] deposits;

	public DepositData[] GetDeposits() => deposits;

	public float GetTerrainNoiseScale() => nosieScale;

	public int GetTerrainNoiseOffset() => nosieOffset;

	public int GetTerrainHeight() => terrainHeight;

	public float GetTerrainScale() => terrainScale;

	public byte GetSurfaceBlock() => surfaceBlock;

	public byte GetSubSurfaceBlock() => subSurfaceBlock;
}