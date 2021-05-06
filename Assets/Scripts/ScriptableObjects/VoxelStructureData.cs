using UnityEngine;

[CreateAssetMenu( fileName = "New Voxel Sturcture", menuName = "ScriptableObjects/Voxel Sturcture Data" )]
public class VoxelStructureData : ScriptableObject {
	public FaceVoxelStructure[] faces = new FaceVoxelStructure[ 6 ];
}

[System.Serializable]
public class VertData {
	public Vector3 vertex;
	public Vector2 uv;

	public VertData( Vector3 _vertex, Vector2 _uv ) {
		vertex = _vertex;
		uv = _uv;
	}

	public Vector3 GetRotatedPosition( Vector3 angles ) {
		Vector3 centre = new Vector3( 0.5f, 0.5f, 0.5f );
		Vector3 direction = vertex - centre;
		direction = Quaternion.Euler( angles ) * direction;
		return direction + centre;
	}
}

[System.Serializable]
public class FaceVoxelStructure {
	public string faceDirection;
	public Vector3 normal;
	public VertData[] vertData = new VertData[ 4 ];
	public int[] triangles = new int[ 6 ];

	public VertData GetVertData( int index ) {
		return vertData[ index ];
	}
}