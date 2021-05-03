using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Deposit", menuName = "ScriptableObjects/Deposit Data" )]
public class DepositData : ScriptableObject {
	public byte blockID;

	[Tooltip( "X: Min, Y: Max" )]
	public Vector2Int height;

	public float noiseScale;
	public float noiseThreshold;
	public float noiseOffset;
}