using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {
	private readonly GameObject chunkObject;
	private readonly MeshCollider meshCollider;
	private readonly MeshFilter meshFilter;
	private readonly MeshRenderer meshRenderer;

	public ChunkData chunkData;

	private int vertexIndex = 0;
	private readonly List<Vector3> vertices = new List<Vector3>();

	private readonly List<int> triangles = new List<int>();
	private readonly List<int> transparentTriangles = new List<int>();

	private readonly List<Vector2> uvs = new List<Vector2>();
	private readonly List<Color> colors = new List<Color>();
	private readonly List<Vector3> normals = new List<Vector3>();

	public Vector3 gPosition;

	private bool _isActive;

	public bool IsActive {
		get { return _isActive; }
		set {
			_isActive = value;
			if ( chunkObject != null )
				chunkObject.SetActive( value );
		}
	}

	public Chunk( ChunkCoord _coord ) {
		chunkData = World.Instance.worldData.RequestChunkData( new Vector2Int( _coord.x, _coord.z ), true );

		chunkData.coord = _coord;
		gPosition = new Vector3( chunkData.GPosition.x, 0, chunkData.GPosition.y );

		chunkObject = new GameObject();
		meshFilter = chunkObject.AddComponent<MeshFilter>();
		meshRenderer = chunkObject.AddComponent<MeshRenderer>();
		meshCollider = chunkObject.AddComponent<MeshCollider>();
		meshRenderer.materials = World.Instance.worldData.materials;

		chunkObject.transform.SetParent( World.Instance.transform );
		chunkObject.transform.position = gPosition;
		chunkObject.name = ( "Chunk: " + chunkData.coord.x + ", " + chunkData.coord.z ).ToString();

		chunkData.chunk = this;

		World.Instance.AddChunkToUpdate( this );
	}

	public void UpdateChunkData() {
		ClearMeshData();

		for ( int x = 0; x < WorldData.chunkWidth; x++ ) {
			for ( int y = 0; y < WorldData.chunkHeight; y++ ) {
				for ( int z = 0; z < WorldData.chunkWidth; z++ ) {
					VoxelData voxel = chunkData.voxelMap[ x, y, z ];
					if ( World.Instance.worldData.blocks[ voxel.id ].canWalkOn )
						UpdateVoxelMeshData( voxel.lPosition );
				}
			}
		}

		World.Instance.chunksToDraw.Enqueue( this );
	}

	private void ClearMeshData() {
		vertexIndex = 0;
		vertices.Clear();
		triangles.Clear();
		transparentTriangles.Clear();
		uvs.Clear();
		colors.Clear();
		normals.Clear();
	}

	public void UpdateVoxelMeshData( Vector3 pos ) {
		int x = Mathf.FloorToInt( pos.x );
		int y = Mathf.FloorToInt( pos.y );
		int z = Mathf.FloorToInt( pos.z );

		VoxelData voxel = chunkData.voxelMap[ x, y, z ];

		for ( int i = 0; i < 6; i++ ) {
			VoxelData neighbour = voxel.neighbours[ i ];

			if ( neighbour != null && neighbour.Properties.RenderNeighborFaces ) {
				float lightLevel = neighbour.LightAsFloat;

				int faceVertCount = 0;

				for ( int j = 0; j < voxel.Properties.voxelStructure.faces[ i ].vertData.Length; j++ ) {
					VertData vertData = voxel.Properties.voxelStructure.faces[ i ].vertData[ j ];
					vertices.Add( pos + vertData.vertex );
					normals.Add( voxel.Properties.voxelStructure.faces[ i ].normal );
					colors.Add( new Color( 0, 0, 0, lightLevel ) );
					AddTextures( voxel.Properties.GetVoxelFace( i ), voxel.Properties.voxelStructure.faces[ i ].vertData[ j ].uv );
					faceVertCount++;
				}

				if ( !voxel.Properties.RenderNeighborFaces ) {
					for ( int j = 0; j < voxel.Properties.voxelStructure.faces[ i ].triangles.Length; j++ )
						triangles.Add( vertexIndex + voxel.Properties.voxelStructure.faces[ i ].triangles[ j ] );
				} else {
					for ( int j = 0; j < voxel.Properties.voxelStructure.faces[ i ].triangles.Length; j++ )
						transparentTriangles.Add( vertexIndex + voxel.Properties.voxelStructure.faces[ i ].triangles[ j ] );
				}

				vertexIndex += faceVertCount;
			}
		}
	}

	public void CreateMesh() {
		Mesh mesh = new Mesh {
			vertices = vertices.ToArray(),

			subMeshCount = 2
		};
		mesh.SetTriangles( triangles.ToArray(), 0 );
		mesh.SetTriangles( transparentTriangles.ToArray(), 1 );
		mesh.uv = uvs.ToArray();
		mesh.colors = colors.ToArray();
		mesh.normals = normals.ToArray();

		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;
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

public class ChunkCoord {
	public int x;
	public int z;

	public ChunkCoord() {
		x = 0;
		z = 0;
	}

	public ChunkCoord( int _x, int _z ) {
		x = _x;
		z = _z;
	}

	public ChunkCoord( Vector3 pos ) {
		int xCheck = Mathf.FloorToInt( pos.x );
		int zCheck = Mathf.FloorToInt( pos.z );

		x = xCheck / WorldData.chunkWidth;
		z = zCheck / WorldData.chunkWidth;
	}

	public bool CompareCoords( ChunkCoord other ) {
		if ( other == null )
			return false;
		else if ( other.x == x && other.z == z )
			return true;
		else
			return false;
	}
}