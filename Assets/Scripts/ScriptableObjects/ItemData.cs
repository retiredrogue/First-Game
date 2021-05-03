using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Item", menuName = "ScriptableObjects/Item Data" )]
public class ItemData : ScriptableObject {
	public int maxStackSize;
	public byte id;
	public int amount;

	public ItemData( byte _id, int _amount ) {
		id = _id;
		amount = _amount;
	}
}