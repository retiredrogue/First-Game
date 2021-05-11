using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Item", menuName = "ScriptableObjects/Item Data" )]
public class ItemData : ScriptableObject {
	public byte id;

	public int amount;

	[SerializeField]
	private int stackSize;

	public BlockData blockTypeInfo;

	public int StackSize { get { return stackSize; } }
}