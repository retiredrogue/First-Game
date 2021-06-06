using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventory : MonoBehaviour {
	public RectTransform slotHighLight;
	private int slotIndex = 0;

	public int GetCurrentSlotIndex() => slotIndex;

	[SerializeField]
	private Inventory inventory;

	private Transform itemSlotContainer;
	public Transform itemSlotTemplatePf;

	private float itemSlotCellSize = 50f;
	private float itemSlotCellOffset = 5f;

	private void Awake() {
		itemSlotContainer = transform.Find( "ToolBar" );
	}

	public void SetInventory( Inventory inventory ) {
		this.inventory = inventory;
		inventory.OnItemListChanged += Inventory_OnItemListChanged;
		RefreshInventoryItems();
	}

	private void Inventory_OnItemListChanged( object sender, System.EventArgs e ) {
		RefreshInventoryItems();
	}

	private void RefreshInventoryItems() {
		foreach ( Transform child in itemSlotContainer ) {
			if ( child.name == "HighLight" )
				continue;
			Destroy( child.gameObject );
		}

		int x = 0, y = 0;

		foreach ( ItemData item in inventory.GetItemList() ) {
			RectTransform itemSlotRectTransform = Instantiate( itemSlotTemplatePf, itemSlotContainer ).GetComponent<RectTransform>();

			itemSlotRectTransform.gameObject.SetActive( true );

			itemSlotRectTransform.anchoredPosition = new Vector2( x * ( itemSlotCellSize + itemSlotCellOffset ) + itemSlotCellOffset, -y * ( itemSlotCellSize + itemSlotCellOffset ) );

			Image image = itemSlotRectTransform.Find( "Image" ).GetComponent<Image>();
			image.enabled = true;
			image.sprite = item.spriteIcon;

			TextMeshProUGUI uiText = itemSlotRectTransform.Find( "Amount" ).GetComponent<TextMeshProUGUI>();
			uiText.SetText( item.amount.ToString() );

			x++;
			if ( x > 10 ) {
				x = 0;
				//y++;
			}
		}
	}

	private void Update() {
		float scroll = Input.GetAxis( "Mouse ScrollWheel" );

		if ( scroll != 0 ) {
			if ( scroll > 0 )
				slotIndex--;
			else
				slotIndex++;

			if ( slotIndex > inventory.GetItemList().Count - 1 )
				slotIndex = 0;
			if ( slotIndex < 0 )
				slotIndex = inventory.GetItemList().Count - 1;

			slotHighLight.anchoredPosition = new Vector2( slotIndex * ( itemSlotCellSize + itemSlotCellOffset ) + itemSlotCellOffset, slotHighLight.anchoredPosition.y );
		}
	}
}