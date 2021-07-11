using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private PlayerInputs inputs;
	private PlayerMovement movements;
	private BlockInteraction interactions;
	private PlayerUIDisplay display;

	public Chest playerToolBar;

	public Camera playerCamera;

	private void Start() {
		Cursor.lockState = CursorLockMode.Locked;
		inputs = GetComponent<PlayerInputs>();
		movements = GetComponent<PlayerMovement>();
		interactions = GetComponent<BlockInteraction>();
		display = GetComponent<PlayerUIDisplay>();
	}

	private void FixedUpdate() {
		display.SlotChanged( inputs.Index() );

		interactions.GhostObject( playerToolBar.inventory[ inputs.Index() ], inputs.Index() );

		if ( inputs.LeftMouse() )
			interactions.PlaceObject();

		if ( inputs.RightMouse() )
			interactions.RemoveObject();

		movements.Look( playerCamera.transform, inputs.LookDirection() );

		movements.Move( inputs.MoveDirection(), inputs.JumpInput() );
	}
}