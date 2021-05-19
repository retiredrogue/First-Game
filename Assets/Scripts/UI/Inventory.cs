using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public UIItemSlot[] slots;

	public List<UIItemSlot> emptySlots = new List<UIItemSlot>();

	private void Start() {
		for ( int i = 0; i < slots.Length; i++ ) {
			if ( i < World.Instance.gameAssetsData.items.Length ) {
				ItemData item = World.Instance.gameAssetsData.items[ i ];
				item.amount = Random.Range( 2, 65 );
				new ItemSlot( slots[ i ], item );
			} else
				new ItemSlot( slots[ i ] );
		}
	}

	//private void Update() {
	//	foreach ( UIItemSlot s in slots ) {
	//		if ( !s.HasItem && !emptySlots.Contains( s ) )
	//			emptySlots.Add( s );
	//		if ( s.HasItem && emptySlots.Contains( s ) )
	//			emptySlots.Remove( s );
	//	}
	//}
}