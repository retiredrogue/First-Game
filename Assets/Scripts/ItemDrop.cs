using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemDrop : MonoBehaviour {
	public byte blockID;
	public int itemID;
	public VoxelSturctureData sturctureData;

	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	private int vertexIndex = 0;
	private List<Vector3> vertices = new List<Vector3>();

	private List<int> transparentTriangles = new List<int>();

	private List<Vector2> uvs = new List<Vector2>();
	private List<Vector3> normals = new List<Vector3>();

	public event EventHandler OnItemDropped;

	public event EventHandler OnItemGrabbed;

	public void Start() {
		UpdateChunkData();
	}

	public void UpdateChunkData() {
		ClearMeshData();
		UpdateVoxelMeshData( Vector3.zero );
	}

	private void ClearMeshData() {
		vertexIndex = 0;
		vertices.Clear();
		transparentTriangles.Clear();
		uvs.Clear();
		normals.Clear();
	}

	public void UpdateVoxelMeshData( Vector3 globalPos ) {
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

	public void CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();

		mesh.subMeshCount = 2;
		mesh.triangles = transparentTriangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.normals = normals.ToArray();

		meshFilter.mesh = mesh;
	}

	private void AddTextures( int textureID, Vector2 uv ) {
		float y = textureID / WorldData.textureAtlasSizeInBlocks;
		float x = textureID - ( y * WorldData.textureAtlasSizeInBlocks );

		x *= WorldData.normalizedBlockTextureSize;
		y *= WorldData.normalizedBlockTextureSize;

		y = 1f - y - WorldData.normalizedBlockTextureSize;

		x += WorldData.normalizedBlockTextureSize * uv.x;
		y += WorldData.normalizedBlockTextureSize * uv.y;

		uvs.Add( new Vector2( x, y ) );
	}
}