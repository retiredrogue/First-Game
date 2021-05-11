using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : MonoBehaviour {
	public byte blockID;
	public int itemID;

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

		if ( blockID == 3/*grass*/ )
			blockID = 8;

		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		collider = GetComponent<BoxCollider>();

		collider.size /= 4;
		collider.isTrigger = true;
		collider.center = collider.size / 2;

		meshRenderer.material = World.Instance.worldData.materials[ 1 ];

		MakeDroppedBlockMesh();
	}

	private void MakeDroppedBlockMesh() {
		ClearMeshData();
		BlockData voxel = World.Instance.worldData.items[ blockID ].blockTypeInfo;

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

	private void OnTriggerEnter( Collider other ) {
		if ( other.CompareTag( "Player" ) ) {
			Player player = other.gameObject.GetComponent<Player>();

			//tool bar check
			for ( int i = 0; i < player.toolBar.slots.Length; i++ ) {
				UIItemSlot slot = player.toolBar.slots[ i ];
				if ( slot.HasItem && slot.itemSlot.item.id == blockID &&
						 slot.itemSlot.item.amount < slot.itemSlot.item.StackSize ) {
					player.toolBar.slots[ i ].itemSlot.Add( 1 );

					Destroy( gameObject );
					return;
				}
			}

			Debug.Log( "toolBar Slot has no equillalent item" );

			//inventory check
			for ( int i = 0; i < player.inventory.slots.Length; i++ ) {
				UIItemSlot slot = player.inventory.slots[ i ];
				if ( slot.HasItem && slot.itemSlot.item.id == blockID &&
						 slot.itemSlot.item.amount < slot.itemSlot.item.StackSize ) {
					player.inventory.slots[ i ].itemSlot.Add( 1 );

					Destroy( gameObject );
					return;
				}
			}
			Debug.Log( "inventory Slot has no equillalent item" );

			// no item previouns item found add to last empty slot
			if ( player.inventory.emptySlots.Count != 0 ) {
				Debug.Log( "item added to last inventory Slot" );

				player.inventory.emptySlots[ player.inventory.emptySlots.Count - 1 ].itemSlot.Add( 1 );
				player.inventory.emptySlots.RemoveAt( player.inventory.emptySlots.Count - 1 );
				Destroy( gameObject );
				return;
			} else // No room for item
				Debug.Log( "inventory full" );
		}
	}
}