using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu( fileName = "New Item", menuName = "ScriptableObjects/Item Data" )]
public class ItemData : ScriptableObject {

	[Header( "UI Info" )]
	public byte id;

	public Sprite spriteIcon;
	public int amount;

	[SerializeField]
	private int maxStackSize;

	public BlockData blockTypeInfo;

	public int StackSize { get { return maxStackSize; } }

	public bool IsStackable() => ( maxStackSize > 0 ) ? true : false;
}