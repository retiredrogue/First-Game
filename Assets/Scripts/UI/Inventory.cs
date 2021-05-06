using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public UIItemSlot[] slots;

	public List<UIItemSlot> emptySlots = new List<UIItemSlot>();

	private void Start() {
		foreach ( UIItemSlot s in slots ) {
			s.UpdateSlot();
			if ( !s.HasItem )
				emptySlots.Add( s );
		}
	}
}