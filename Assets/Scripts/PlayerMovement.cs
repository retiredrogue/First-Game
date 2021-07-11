using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterController ) )]
public class PlayerMovement : MonoBehaviour {
	[SerializeField] private float walkSpeed = 5f;
	[SerializeField] private float turnSpeed = 6f;

	private float xRotation = 0f;
	private Vector3 velocity = Vector3.zero;

	[Tooltip( "x: on ground,y: in air" ), SerializeField]
	private Vector2 accelerationTime = new Vector2( .1f, .2f );

	[Tooltip( "x: min, y: max" ), SerializeField]
	private Vector2 jumpHeight;

	[SerializeField] private float timeToJumpApex = .4f;

	private Vector2 jumpVelocity;
	private Vector2 targetVelocitySmoothing;
	private float gravity;

	private CharacterController controller;

	private void Start() {
		controller = GetComponent<CharacterController>();
		SetGravity();
	}

	public void Move( Vector3 direction, bool jumpRequest ) {
		JumpCheck( jumpRequest );
		CalculateVelocity( direction );

		// apply constant force down "Gravity"
		if ( controller.isGrounded && velocity.y < 0 )
			velocity.y = -1;

		controller.Move( velocity * Time.deltaTime );
	}

	public void Look( Transform camera, Vector2 lookdirection ) {
		lookdirection *= ( ( turnSpeed * 20 ) * Time.deltaTime );

		xRotation -= lookdirection.y;// - cause it flips rotation if adding it
		xRotation = Mathf.Clamp( xRotation, -90f, 90f );

		camera.transform.localRotation = Quaternion.Euler( xRotation, 0f, 0f );

		transform.Rotate( Vector3.up * lookdirection.x );
	}

	private void SetGravity() {
		//sets gravity based on max height you want
		gravity = -( 2 * jumpHeight.y ) / Mathf.Pow( timeToJumpApex, 2 );
		//sets jump force based on gravity
		jumpVelocity.y = Mathf.Abs( gravity ) * timeToJumpApex;//max jumpforce
		jumpVelocity.x = Mathf.Sqrt( 2 * Mathf.Abs( gravity ) * jumpHeight.x );//min jumpforce
	}

	private void CalculateVelocity( Vector3 direction ) {
		Vector3 targetVelocity = direction * walkSpeed;

		velocity.x = Mathf.SmoothDamp( velocity.x, targetVelocity.x, ref targetVelocitySmoothing.x,
		  ( controller.isGrounded ) ? accelerationTime.x : accelerationTime.y );
		velocity.z = Mathf.SmoothDamp( velocity.z, targetVelocity.z, ref targetVelocitySmoothing.y,
		  ( controller.isGrounded ) ? accelerationTime.x : accelerationTime.y );
		velocity.y += gravity * Time.deltaTime;
	}

	private void JumpCheck( bool jumpRequest ) {
		if ( jumpRequest && controller.isGrounded )
			velocity.y = jumpVelocity.y;
		if ( !jumpRequest && velocity.y > jumpVelocity.x )
			velocity.y = jumpVelocity.x;
	}
}