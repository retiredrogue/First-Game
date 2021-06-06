using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

	public event EventHandler OnItemListChanged;

	private List<ItemData> itemList;

	public Inventory() {
		itemList = new List<ItemData>();
	}

	public void AddItem( ItemData item ) {
		if ( item.IsStackable() ) {
			bool itemAlreadyInInventory = false;

			foreach ( ItemData inventoryItem in itemList ) {
				if ( inventoryItem.id == item.id ) {
					inventoryItem.amount += item.amount;
					itemAlreadyInInventory = true;
				}
			}
			if ( !itemAlreadyInInventory )
				itemList.Add( item );
		} else
			itemList.Add( item );

		OnItemListChanged?.Invoke( this, EventArgs.Empty );
	}

	public void RemoveItem( ItemData item ) {
		if ( item.IsStackable() ) {
			ItemData itemInInventory = null;

			foreach ( ItemData inventoryItem in itemList ) {
				if ( inventoryItem.id == item.id ) {
					inventoryItem.amount -= 1;
					itemInInventory = inventoryItem;
				}
			}
			if ( itemInInventory != null && itemInInventory.amount <= 0 )
				itemList.Remove( itemInInventory );
		} else
			itemList.Remove( item );

		OnItemListChanged?.Invoke( this, EventArgs.Empty );
	}

	public List<ItemData> GetItemList() {
		return itemList;
	}
}