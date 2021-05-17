using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public UIItemSlot[] slots;

	public List<UIItemSlot> emptySlots = new List<UIItemSlot>();

	private void Start() {
		byte index = 1;
		foreach ( UIItemSlot s in slots ) {
			ItemData item = World.Instance.gameAssetsData.items[ index ];
			item.amount = Random.Range( 2, 65 );
			ItemSlot slot = new ItemSlot( s, item );
			if ( index < World.Instance.gameAssetsData.items.Length - 1 )
				index++;
			else
				return;
		}
	}

	private void Update() {
		foreach ( UIItemSlot s in slots ) {
			s.UpdateSlot();
			if ( !s.HasItem && !emptySlots.Contains( s ) )
				emptySlots.Add( s );
			if ( s.HasItem && emptySlots.Contains( s ) )
				emptySlots.Remove( s );
		}
	}
}