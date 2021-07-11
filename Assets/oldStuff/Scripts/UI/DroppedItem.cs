using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour {
	private ItemData item;

	public static DroppedItem SpawnDroppedItemToWorld( Vector3 position, ItemData item, int amount ) {
		Transform transform = Instantiate( GameAssets.Instance.droppedItemPf, position, Quaternion.identity );

		DroppedItem droppedItem = transform.GetComponent<DroppedItem>();
		droppedItem.SetItem( item );
		item.amount = amount;

		return droppedItem;
	}

	public void SetItem( ItemData item ) {
		this.item = item;
	}

	public ItemData GetItem() => item;

	public void DestorySelf() => Destroy( gameObject );

	private readonly float blockOffset = .375f;

	public VoxelStructureData structureData;

	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private new BoxCollider collider;

	private int vertexIndex = 0;
	private readonly List<Vector3> vertices = new List<Vector3>();

	private readonly List<int> transparentTriangles = new List<int>();

	private readonly List<Vector2> uvs = new List<Vector2>();
	private readonly List<Vector3> normals = new List<Vector3>();

	private void Start() {
		transform.position += new Vector3( blockOffset, blockOffset, blockOffset );

		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		collider = GetComponent<BoxCollider>();

		collider.size /= 4;
		collider.isTrigger = true;
		collider.center = collider.size / 2;

		meshRenderer.material = GameAssets.Instance.materials[ 1 ];

		MakeDroppedBlockMesh();
	}

	private void MakeDroppedBlockMesh() {
		ClearMeshData();
		BlockData voxel = GameAssets.Instance.items[ item.id ].blockTypeInfo;

		for ( int i = 0; i < 6; i++ ) {
			int faceVertCount = 0;

			for ( int j = 0; j < structureData.faces[ i ].vertData.Length; j++ ) {
				VertData vertData = structureData.faces[ i ].vertData[ j ];
				vertices.Add( vertData.vertex );
				normals.Add( structureData.faces[ i ].normal );
				AddTextures( voxel.GetVoxelFace( i ), structureData.faces[ i ].vertData[ j ].uv );
				faceVertCount++;
			}
			for ( int j = 0; j < structureData.faces[ i ].triangles.Length; j++ )
				transparentTriangles.Add( vertexIndex + structureData.faces[ i ].triangles[ j ] );
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