using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public UIItemSlot[] slots;

	private void Start() {
		foreach ( UIItemSlot s in slots ) {
			s.UpdateSlot();
		}
	}
}