using UnityEngine;

[CreateAssetMenu( fileName = "New Biome", menuName = "ScriptableObjects/Biome Data" )]
public class BiomeData : ScriptableObject {

	[Header( "Major Flora" )]
	[SerializeField] private int nosieOffset;

	[SerializeField] private float nosieScale;

	[SerializeField] private int terrainHeight;
	[SerializeField] private float terrainScale;

	[SerializeField] private byte surfaceBlock;
	[SerializeField] private byte subSurfaceBlock;

	[Header( "Major Flora Structure" )]
	[SerializeField] private int majorFloraIndex;

	[SerializeField] private float majorFloraZoneScale;

	[Range( 0.1f, 1f )]
	[SerializeField] private float majorFloraZoneThreshold;

	[SerializeField] private float majorFloraPlacementScale;

	[Range( 0.1f, 1f )]
	[SerializeField] private float majorFloraPlacementThreshold;

	[SerializeField] private bool placeMajorFlora;

	[Tooltip( "X: Min, Y: Max" )]
	[SerializeField] private Vector2Int height;

	[SerializeField] private DepositData[] deposits;

	public DepositData[] GetDeposits() => deposits;

	public float GetTerrainNoiseScale() => nosieScale;

	public int GetTerrainNoiseOffset() => nosieOffset;

	public int GetTerrainHeight() => terrainHeight;

	public float GetTerrainScale() => terrainScale;

	public byte GetSurfaceBlock() => surfaceBlock;

	public byte GetSubSurfaceBlock() => subSurfaceBlock;
}