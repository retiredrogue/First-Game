using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItemHandler : MonoBehaviour {
	[SerializeField] private UIItemSlot cursorSlot = null;
	private ItemSlot cursorItemSlot;

	[SerializeField] private GraphicRaycaster m_Raycaster = null;
	private PointerEventData m_PointerEventData;
	[SerializeField] private EventSystem m_EventSystem = null;

	private Player player;

	private void Start() {
		player = GameObject.Find( "Player" ).GetComponent<Player>();

		cursorItemSlot = new ItemSlot( cursorSlot );
	}

	private void Update() {
		if ( !player.InUI )
			return;

		cursorSlot.transform.position = Input.mousePosition;

		if ( Input.GetMouseButtonDown( 0 ) ) {
			HandleSlotClick( CheckForSlot() );
		}
	}

	private void HandleSlotClick( UIItemSlot clickedSlot ) {
		if ( clickedSlot == null )
			return;

		if ( !cursorSlot.HasItemInSlot && !clickedSlot.HasItemInSlot )
			return;

		//if ( clickedSlot.itemSlot.isCreative ) {
		//	cursorItemSlot.EmptySlot();
		//	cursorItemSlot.InsertStack( clickedSlot.itemSlot.item );
		//}

		if ( !cursorSlot.HasItemInSlot && clickedSlot.HasItemInSlot ) {
			cursorItemSlot.InsertStack( clickedSlot.itemSlot.TakeAll() );
			return;
		}

		if ( cursorSlot.HasItemInSlot && !clickedSlot.HasItemInSlot ) {
			clickedSlot.itemSlot.InsertStack( cursorItemSlot.TakeAll() );
			return;
		}

		if ( cursorSlot.HasItemInSlot && clickedSlot.HasItemInSlot ) {
			if ( cursorSlot.itemSlot.item.id != clickedSlot.itemSlot.item.id ) {
				ItemData oldCursorSlot = cursorSlot.itemSlot.TakeAll();
				ItemData oldSlot = clickedSlot.itemSlot.TakeAll();

				clickedSlot.itemSlot.InsertStack( oldCursorSlot );
				cursorSlot.itemSlot.InsertStack( oldSlot );
			}
		}
	}

	private UIItemSlot CheckForSlot() {
		m_PointerEventData = new PointerEventData( m_EventSystem ) {
			position = Input.mousePosition
		};

		List<RaycastResult> results = new List<RaycastResult>();
		m_Raycaster.Raycast( m_PointerEventData, results );

		foreach ( RaycastResult result in results ) {
			Debug.Log( result );
			if ( result.gameObject.CompareTag( "UIItemSlot" ) ) {
				Debug.Log( "found slot" );
				return result.gameObject.GetComponent<UIItemSlot>();
			}
		}

		return null;
	}
}