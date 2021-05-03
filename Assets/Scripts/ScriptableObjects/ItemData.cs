using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Item", menuName = "ScriptableObjects/Item Data" )]
public class ItemData : ScriptableObject {
	public int maxStackSize;
}