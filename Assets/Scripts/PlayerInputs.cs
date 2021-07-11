using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour {
	private bool jump = false;
	private bool leftMouse = false;
	private bool rightMouse = false;

	private Vector3 moveDirection = Vector3.zero;

	private Vector2 lookDirection = Vector2.zero;

	private int scrollIndex = 0;

	private void Update() {
		moveDirection = transform.right * Input.GetAxisRaw( "Horizontal" ) + transform.forward * Input.GetAxisRaw( "Vertical" );

		lookDirection = new Vector2( Input.GetAxisRaw( "Mouse X" ), Input.GetAxisRaw( "Mouse Y" ) );

		jump = Input.GetKey( KeyCode.Space );

		//left mouse button released
		leftMouse = Input.GetMouseButtonUp( 0 );

		//right mouse button released
		rightMouse = Input.GetMouseButtonUp( 1 );

		if ( Input.mouseScrollDelta.y > 0 )
			if ( scrollIndex == 9 )
				scrollIndex = 0;
			else
				scrollIndex++;
		if ( Input.mouseScrollDelta.y < 0 )
			if ( scrollIndex == 0 )
				scrollIndex = 9;
			else
				scrollIndex--;
	}

	public bool LeftMouse() {
		return leftMouse;
	}

	public int Index() {
		return scrollIndex;
	}

	public bool RightMouse() {
		return rightMouse;
	}

	public Vector3 MoveDirection() {
		return moveDirection;
	}

	public Vector2 LookDirection() {
		return lookDirection;
	}

	public bool JumpInput() {
		return jump;
	}
}