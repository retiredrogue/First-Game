using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemSlot : MonoBehaviour {
	public bool isLinked = false;
	public ItemSlot itemSlot;
	public Image slotImage;
	public Image slotIcon;
	public TextMeshProUGUI slotAmount;

	public bool HasItem {
		get {
			if ( itemSlot == null )
				return false;
			else
				return itemSlot.HasItem;
		}
	}

	public void Link( ItemSlot _itemSlot ) {
		itemSlot = _itemSlot;
		isLinked = true;
		itemSlot.LinkUISlot( this );
		UpdateSlot( itemSlot.item );
	}

	public void UnLink() {
		itemSlot.UnLinkUISlot();
		itemSlot = null;
		UpdateSlot( null );
	}

	public void UpdateSlot( ItemData item ) {
		if ( itemSlot != null && itemSlot.HasItem ) {
			Debug.Log( item.id );
			//slotIcon.sprite = World.Instance.gameAssetsData.items[ itemSlot.item.id ].spriteIcon;
			slotIcon.sprite = item.spriteIcon;
			//slotAmount.text = World.Instance.gameAssetsData.items[ itemSlot.item.id ].amount.ToString();
			slotAmount.text = itemSlot.item.amount.ToString();
			slotIcon.enabled = true;
			slotAmount.enabled = true;
		} else
			Clear();
	}

	public void Clear() {
		slotIcon.sprite = null;
		slotAmount.text = "";
		slotIcon.enabled = false;
		slotAmount.enabled = false;
	}

	private void OnDestroy() {
		if ( itemSlot != null )
			itemSlot.UnLinkUISlot();
	}
}

public class ItemSlot {
	public ItemData item = null;
	private UIItemSlot uiItemSlot = null;

	public ItemSlot( UIItemSlot _uiItemSlot ) {
		item = null;
		uiItemSlot = _uiItemSlot;
		uiItemSlot.Link( this );
	}

	public ItemSlot( UIItemSlot _uiItemSlot, ItemData _item ) {
		item = _item;
		uiItemSlot = _uiItemSlot;
		uiItemSlot.Link( this );
	}

	public void LinkUISlot( UIItemSlot uiSlot ) {
		uiItemSlot = uiSlot;
	}

	public void UnLinkUISlot() {
		uiItemSlot = null;
	}

	public void EmptySlot() {
		item = null;
		if ( uiItemSlot != null )
			uiItemSlot.UpdateSlot( item );
	}

	public int Take( int amt ) {
		if ( amt > item.amount ) {
			int _amt = item.amount;
			EmptySlot();
			return _amt;
		} else if ( amt < item.amount ) {
			item.amount -= amt;
			uiItemSlot.UpdateSlot( item );
			return amt;
		} else {
			EmptySlot();
			return amt;
		}
	}

	public int Add( int amt ) {
		item.amount += amt;
		uiItemSlot.UpdateSlot( item );
		return amt;
	}

	public ItemData TakeAll() {
		ItemData handOver = ScriptableObject.CreateInstance<ItemData>();
		//handOver.id = item.id;
		handOver.amount = item.amount;
		EmptySlot();
		return handOver;
	}

	public void InsertStack( ItemData _item ) {
		item = _item;
		uiItemSlot.UpdateSlot( item );
	}

	public bool HasItem {
		get {
			if ( item != null )
				return true;
			else
				return false;
		}
	}
}