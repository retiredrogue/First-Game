using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public UIItemSlot[] slots;

	// Start is called before the first frame update
	private void Start() {
		foreach ( UIItemSlot s in slots ) {
			s.UpdateSlot();
		}
	}

	// Update is called once per frame
	private void Update() {
	}
}