using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRenderer : MonoBehaviour {
	private MeshRenderer[] renderers;

	private BoxCollider[] colliders;

	private void Start() {
		renderers = GetComponentsInChildren<MeshRenderer>();
		colliders = GetComponentsInChildren<BoxCollider>();
	}

	public void ChangeMaterial( Material newMaterial ) {
		for ( int i = 0; i < renderers.Length; i++ ) {
			renderers[ i ].material = newMaterial;
			colliders[ i ].enabled = true;
		}
	}
}