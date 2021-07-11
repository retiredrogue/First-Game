using UnityEngine;

public class Player : MonoBehaviour {
	public LayerMask targetBlock;

	private Transform cam;
	private World world;
	public Vector3 playerOrientation;

	[Tooltip( "x: max,y: min" )]
	public Vector2 jumpHeight;

	public float timeToJumpApex = .4f;
	private Vector2 jumpVelocity; // same as jumpHeight

	[Tooltip( "x: air,y: ground" )]
	public Vector2 accelerationTime = new Vector2( .2f, .1f );

	private CharacterController controller;
	private Vector3 inputDirection;

	public float walkSpeed = 4f;
	private float sprintSpeed;
	private float moveSpeed;
	private Vector2 targetVelocitySmoothing;

	private float gravity;
	private Vector3 velocity;

	private float mouseSensitivity;
	private float xRotation = 0f;

	public float reach = 8;

	public GameObject blockHighLight;

	private bool _inUI = false;

	public bool InUI {
		get { return _inUI; }

		set {
			_inUI = value;
			if ( _inUI ) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			} else {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
	}

	private Inventory inventory;

	[SerializeField] private UIInventory uIInventory;

	// Start is called before the first frame update
	private void Awake() {
		inventory = new Inventory();
		uIInventory.SetInventory( inventory );
	}

	private void Start() {
		world = GameObject.Find( "World" ).GetComponent<World>();
		controller = GetComponent<CharacterController>();
		cam = Camera.main.transform;

		moveSpeed = walkSpeed;
		sprintSpeed = walkSpeed * 1.5f;

		Cursor.lockState = CursorLockMode.Locked;

		mouseSensitivity = 15;
	}

	private void FixedUpdate() {
		if ( !InUI )
			CalculateVelocity();
		if ( world.isWorldLoaded )
			SetGravity();
	}

	private void Update() {
		if ( Input.GetKeyDown( KeyCode.E ) ) {
			InUI = !InUI;
		}
		if ( !InUI ) {
			GetPlayerMovements();
			CalculateVelocity();
			PlayerLook();

			if ( world.isWorldLoaded )
				BlockHighLightPosition();

			if ( Input.GetMouseButtonDown( 0 ) && blockHighLight.activeSelf )
				RemoveBlock( blockHighLight.transform.position );

			if ( Input.GetMouseButtonDown( 1 ) && blockHighLight.activeSelf )
				//use item
				PlaceBlock( blockHighLight.transform.position );

			controller.Move( velocity * Time.deltaTime );
			Cursor.lockState = CursorLockMode.Locked;
		}
		playerOrientation = transform.rotation.eulerAngles / 90;
		//Vector3 compassDirection = transform.forward;
		//compassDirection.y = 0;
		//if ( Vector3.Angle( compassDirection, Vector3.forward ) <= 45 )
		//	orientation = 0; // Player is facing north
		//else if ( Vector3.Angle( compassDirection, Vector3.right ) <= 45 )
		//	orientation = 2; // player facing east
		//else if ( Vector3.Angle( compassDirection, Vector3.back ) <= 45 )
		//	orientation = 5; // player facing south
		//else
		//	orientation = 3; // player facing west
	}

	private void CalculateVelocity() {
		Vector3 targetVelocity = inputDirection * moveSpeed;
		velocity.x = Mathf.SmoothDamp( velocity.x, targetVelocity.x, ref targetVelocitySmoothing.x,
		  ( controller.isGrounded ) ? accelerationTime.y : accelerationTime.x );
		velocity.z = Mathf.SmoothDamp( velocity.z, targetVelocity.z, ref targetVelocitySmoothing.y,
		  ( controller.isGrounded ) ? accelerationTime.y : accelerationTime.x );
		velocity.y += gravity * Time.deltaTime;
	}

	private void GetPlayerMovements() {
		if ( Input.GetKeyDown( KeyCode.Escape ) )
			Application.Quit();

		if ( controller.isGrounded && velocity.y < 0 )
			velocity.y = -3f;

		inputDirection = ( Input.GetAxisRaw( "Horizontal" ) * transform.right ) + ( Input.GetAxisRaw( "Vertical" ) * transform.forward );

		if ( Input.GetKeyDown( KeyCode.LeftShift ) )
			moveSpeed = sprintSpeed;

		if ( Input.GetKeyUp( KeyCode.LeftShift ) )
			moveSpeed = walkSpeed;

		if ( Input.GetKeyDown( KeyCode.Space ) )
			OnJumpInputDown();

		if ( Input.GetKeyUp( KeyCode.Space ) )
			OnJumpInputUp();
	}

	public void OnJumpInputUp() {
		if ( velocity.y > jumpHeight.y ) {
			velocity.y = jumpVelocity.y;
		}
	}

	public void OnJumpInputDown() {
		if ( controller.isGrounded ) {
			velocity.y = jumpVelocity.x;
		}
	}

	private void BlockHighLightPosition() {
		float checkIncrement = .05f;
		float step = checkIncrement;
		while ( step < reach ) {
			Vector3 pos = cam.position + ( cam.forward * step );
			if ( World.Instance.worldData.CheckForVoxel( pos ) ) {
				Vector3 voxelPos = World.Instance.worldData.GetVoxelData( pos ).GPosition;
				blockHighLight.transform.position = voxelPos;
				blockHighLight.gameObject.SetActive( true );
				return;
			}
			step += checkIncrement;
		}
		blockHighLight.gameObject.SetActive( false );
	}

	private void RemoveBlock( Vector3 voxelWorldPosition ) {
		if ( World.Instance.worldData.CheckForVoxel( voxelWorldPosition ) ) {
			DroppedItem.SpawnDroppedItemToWorld( voxelWorldPosition, GameAssets.Instance.items[ World.Instance.worldData.GetVoxelData( voxelWorldPosition ).id ], 1 );
			World.Instance.worldData.EditVoxel( voxelWorldPosition, 0/*air block*/ , Vector3.zero );
		}
	}

	//REWORK
	private void PlaceBlock( Vector3 highLightPosition ) {
		if ( Physics.Raycast( cam.position, cam.forward * reach, out RaycastHit hit, reach, targetBlock ) ) {
			//if ( toolBar.slots[ toolBar.slotIndex ].HasItemInSlot ) {
			//	Vector3 newVoxelPos = highLightPosition + hit.normal;
			//	World.Instance.worldData.EditVoxel( newVoxelPos, toolBar.slots[ toolBar.slotIndex ].itemSlot.item.id, Vector3.zero );
			//	toolBar.slots[ toolBar.slotIndex ].itemSlot.Take( 1 );
			//	Debug.Log( playerOrientation );
			//}
		}
	}

	private void SetGravity() {
		gravity = -( 2 * jumpHeight.x ) / Mathf.Pow( timeToJumpApex, 2 );
		jumpVelocity.x = Mathf.Abs( gravity ) * timeToJumpApex;
		jumpVelocity.y = Mathf.Sqrt( 2 * Mathf.Abs( gravity ) * jumpHeight.y );
	}

	private void PlayerLook() {
		Vector2 mouseInput = new Vector2( Input.GetAxis( "Mouse X" ), Input.GetAxis( "Mouse Y" ) );
		mouseInput *= ( ( mouseSensitivity * 10 ) * Time.deltaTime );
		xRotation -= mouseInput.y;
		xRotation = Mathf.Clamp( xRotation, -90f, 90f );
		cam.transform.localRotation = Quaternion.Euler( xRotation, 0f, 0f );
		transform.Rotate( Vector3.up * mouseInput.x );
	}

	private void OnTriggerEnter( Collider other ) {
		DroppedItem droppedItem = other.GetComponent<DroppedItem>();
		if ( droppedItem != null ) {
			inventory.AddItem( droppedItem.GetItem() );
			droppedItem.DestorySelf();
		}
	}
}