using UnityEngine;

[CreateAssetMenu( fileName = "New BlockType", menuName = "ScriptableObjects/Block Data" )]
public class BlockTypeData : ScriptableObject {
	public bool canWalkOn;
	public bool canPassThrough;
	public VoxelSturctureData voxelStructure;
	public bool isFluid;
	public bool isRotatable;
	public bool doesFall;

	[Range( 0, 15 ), Tooltip( "15 is industructable, 0 instant brake" )]
	public byte hardness;

	public bool renderNeighborFaces => ( opacityValue < 15 ) ? true : false;

	[Range( 0, 15 ), Tooltip( "15 is solid, 0 clear like glass" )]
	public byte opacityValue;

	public Sprite icon;

	[Header( "Texture Values" )]
	public int frontFaceTexture;

	public int topFaceTexture;
	public int rightFaceTexture;
	public int leftFaceTexture;
	public int bottomFaceTexture;
	public int backFaceTexture;

	// Front, Top, Right, Left,  Bottom, Back
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