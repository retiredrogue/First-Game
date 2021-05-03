using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
	public byte id;
	public int amount;

	public Item( byte _id, int _amount ) {
		id = _id;
		amount = _amount;
	}
}