using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour {
	public byte blockID;
	public int itemID;
	public VoxelSturctureData sturctureData;

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	private int vertexIndex = 0;
	private readonly List<Vector3> vertices = new List<Vector3>();

	private readonly List<int> transparentTriangles = new List<int>();

	private readonly List<Vector2> uvs = new List<Vector2>();
	private readonly List<Vector3> normals = new List<Vector3>();

	private void Start() {
		World.Instance.player.GetComponent<Player>().OnBlockBroke += ItemDrop_OnBlockBroke;
	}

	private void ItemDrop_OnBlockBroke( object sender, Player.OnBlockBrokeEventArgs e ) {
		Vector3 newPos = e.pos + new Vector3( .5f, .5f, .5f );
		blockID = e.blockID;
		Debug.Log( "make Block" );
		MakeDroppedBlockMesh( newPos );
	}

	private void MakeDroppedBlockMesh( Vector3 globalPos ) {
		ClearMeshData();
		BlockData voxel = World.Instance.worldData.blocks[ blockID ];

		for ( int i = 0; i < 6; i++ ) {
			int faceVertCount = 0;

			for ( int j = 0; j < sturctureData.faces[ i ].vertData.Length; j++ ) {
				VertData vertData = sturctureData.faces[ i ].vertData[ j ];
				vertices.Add( globalPos + vertData.vertex );
				normals.Add( sturctureData.faces[ i ].normal );
				AddTextures( voxel.GetVoxelFace( i ), sturctureData.faces[ i ].vertData[ j ].uv );
				faceVertCount++;
			}
			for ( int j = 0; j < sturctureData.faces[ i ].triangles.Length; j++ )
				transparentTriangles.Add( vertexIndex + sturctureData.faces[ i ].triangles[ j ] );
			vertexIndex += faceVertCount;
		}
		CreateMesh();
	}

	private void ClearMeshData() {
		vertexIndex = 0;
		vertices.Clear();
		transparentTriangles.Clear();
		uvs.Clear();
		normals.Clear();
	}

	public void CreateMesh() {
		Mesh mesh = new Mesh {
			vertices = vertices.ToArray(),

			subMeshCount = 2,
			triangles = transparentTriangles.ToArray(),
			uv = uvs.ToArray(),
			normals = normals.ToArray()
		};

		meshFilter.mesh = mesh;
	}

	private void AddTextures( int textureID, Vector2 uv ) {
		float y = textureID / WorldData.textureAtlasSizeInBlocks;
		float x = textureID - ( y * WorldData.textureAtlasSizeInBlocks );

		x *= WorldData.NormalizedBlockTextureSize;
		y *= WorldData.NormalizedBlockTextureSize;

		y = 1f - y - WorldData.NormalizedBlockTextureSize;

		x += WorldData.NormalizedBlockTextureSize * uv.x;
		y += WorldData.NormalizedBlockTextureSize * uv.y;

		uvs.Add( new Vector2( x, y ) );
	}
}