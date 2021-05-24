using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {
	private Inventory inventory;

	private Transform itemSlotContainer;
	public Transform itemSlotTemplatePf;

	private void Awake() {
		itemSlotContainer = transform.Find( "ToolBar" );
	}

	public void SetInventory( Inventory inventory ) {
		this.inventory = inventory;
		RefreshInventoryItems();
	}

	private void RefreshInventoryItems() {
		int x = 0, y = 0;
		float itemSlotCellSize = 50f;
		float itemSlotCellOffset = 5f;
		foreach ( ItemData item in inventory.GetItemList() ) {
			RectTransform itemSlotRectTransform = Instantiate( itemSlotTemplatePf, itemSlotContainer ).GetComponent<RectTransform>();
			itemSlotRectTransform.gameObject.SetActive( true );
			itemSlotRectTransform.anchoredPosition = new Vector2( x * ( itemSlotCellSize + itemSlotCellOffset ) + itemSlotCellOffset, -y * ( itemSlotCellSize + itemSlotCellOffset ) );
			Image image = itemSlotRectTransform.Find( "Image" ).GetComponent<Image>();
			image.enabled = true;
			image.sprite = item.spriteIcon;

			x++;
			if ( x > 4 ) {
				x = 0;
				y++;
			}
		}
	}
}