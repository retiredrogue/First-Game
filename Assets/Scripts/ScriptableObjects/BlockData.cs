using UnityEngine;

[System.Serializable]
public class BlockData {
	[SerializeField] private bool canWalkOn, canPassThrough, isFluid, canRotate, canFall;
	[SerializeField] private VoxelStructureData voxelStructure;

	[Range( 0, 15 ), Tooltip( "15 is industructable, 0 instant brake" )]
	[SerializeField] private byte hardness;

	[SerializeField] private bool RenderNeighborFaces => ( opacityValue < 15 );

	[Range( 0, 15 ), Tooltip( "15 is solid, 0 clear like glass" )]
	[SerializeField] private byte opacityValue;

	[Header( "Texture Values" )]
	[SerializeField] private int frontFaceTexture;

	[SerializeField] private int topFaceTexture;
	[SerializeField] private int rightFaceTexture;
	[SerializeField] private int leftFaceTexture;
	[SerializeField] private int bottomFaceTexture;
	[SerializeField] private int backFaceTexture;

	public void RotateFaces( Vector3 otherFaceNormal ) {
		Debug.Log( otherFaceNormal );
	}

	public VoxelStructureData GetVoxelStructure() => voxelStructure;

	public bool GetRotatable() => canRotate;

	public bool GetFlowable() => isFluid;

	public bool GetWalkable() => canWalkOn;

	public bool Get() => canFall;

	public bool GetPassable() => canPassThrough;

	public byte GetOpacity() => opacityValue;

	public bool GetNeighborRendering() => RenderNeighborFaces;

	public int GetVoxelFace( int faceIndex ) {
		switch ( faceIndex ) {
			case 0:
				return frontFaceTexture;

			case 1:
				return topFaceTexture;

			case 2:
				return rightFaceTexture;

			case 3:
				return leftFaceTexture;

			case 4:
				return bottomFaceTexture;

			case 5:
				return backFaceTexture;

			default:
				Debug.Log( "Error in GetTextureID; invalid face index" );
				return 0;
		}
	}
}