using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
	private readonly List<ItemData> itemList;

	public Inventory() {
		itemList = new List<ItemData>();

		AddItem( GameAssets.Instance.items[ 1 ] );
		AddItem( GameAssets.Instance.items[ 2 ] );
		AddItem( GameAssets.Instance.items[ 3 ] );
		AddItem( GameAssets.Instance.items[ 4 ] );

		Debug.Log( itemList.Count );
	}

	public void AddItem( ItemData item ) {
		itemList.Add( item );
	}

	public List<ItemData> GetItemList() {
		return itemList;
	}
}