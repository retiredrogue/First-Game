using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Deposit", menuName = "ScriptableObjects/Deposit Data" )]
public class DepositData : ScriptableObject {
	public byte depositTypeID;

	[Tooltip( "X: Min, Y: Max" )]
	public Vector2Int height;

	public float noiseScale;
	public float noiseThreshold;
	public float noiseOffset;

	public float GetScale() => noiseScale;

	public float GetThreshold() => noiseThreshold;

	public float GetOffset() => noiseOffset;

	public byte GetDepositId() => depositTypeID;

	public Vector2Int GetHeight() => height;
}