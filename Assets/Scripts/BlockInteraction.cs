using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {
	[SerializeField] private float playerReach;
	[SerializeField] private float buildReach;
	[SerializeField] private Material material;

	private Transform ghost = null;
	private int lastIndex = 0;

	public void GhostObject( Transform objectToPlace, int index ) {
		if ( objectToPlace == null && ghost != null ) {
			Destroy( ghost.gameObject );
			ghost = null;
		}
		if ( objectToPlace != null ) {
			if ( index != lastIndex ) {
				if ( ghost != null ) {
					Destroy( ghost.gameObject );
					ghost = null;
				}
				lastIndex = index;
			}

			if ( FindObject( buildReach ).transform != null ) {
				if ( ghost == null ) {
					ghost = Instantiate( objectToPlace, FindObject( buildReach ).point, Quaternion.identity );
					ghost.name = objectToPlace.name + "(Ghost)";
				}

				ghost.position = FindObject( buildReach ).point;
			}
		}
	}

	public void PlaceObject() {
		if ( FindObject( buildReach ).transform != null ) {
			ghost.gameObject.GetComponent<ObjectRenderer>().ChangeMaterial( material );
			Transform placedObject = Instantiate( ghost, FindObject( buildReach ).point, Quaternion.identity );
			placedObject.name = ghost.name.Replace( "(Ghost)", "" );
			Destroy( ghost.gameObject );
			ghost = null;
		}
	}

	public void RemoveObject() {
	}

	public RaycastHit FindObject( float maxDistance ) {
		RaycastHit hit;

		if ( Physics.Raycast( Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance ) ) {
		}
		return hit;
	}
}