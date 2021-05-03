using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar : MonoBehaviour {
	public UIItemSlot[] slots;
	public RectTransform highlight;
	public Player player;
	public int slotIndex = 0;

	private void Start() {
		byte index = 1;
		foreach ( UIItemSlot s in slots ) {
			ItemData item = new ItemData( index, Random.Range( 2, 65 ) );
			ItemSlot slot = new ItemSlot( s, item );
			index++;
		}
	}

	private void Update() {
		float scroll = Input.GetAxis( "Mouse ScrollWheel" );

		if ( scroll != 0 ) {
			if ( scroll > 0 )
				slotIndex--;
			else
				slotIndex++;

			if ( slotIndex > slots.Length - 1 )
				slotIndex = 0;
			if ( slotIndex < 0 )
				slotIndex = slots.Length - 1;

			highlight.position = slots[ slotIndex ].slotIcon.transform.position;
		}
	}
}