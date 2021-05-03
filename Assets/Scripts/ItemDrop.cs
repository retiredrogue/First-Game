using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemDrop : MonoBehaviour {
	public int blockID;
	public int itemID;

	public event EventHandler OnItemDropped;

	public event EventHandler OnItemGrabbed;
}