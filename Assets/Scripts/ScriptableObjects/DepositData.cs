using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Deposit", menuName = "ScriptableObjects/Deposit Data" )]
public class DepositData : ScriptableObject {
	[SerializeField] private byte depositTypeID;

	[Tooltip( "X: Min, Y: Max" )]
	[SerializeField] private Vector2Int height;

	[SerializeField] private float noiseScale;
	[SerializeField] private float noiseThreshold;
	[SerializeField] private float noiseOffset;

	public float GetScale() => noiseScale;

	public float GetThreshold() => noiseThreshold;

	public float GetOffset() => noiseOffset;

	public byte GetDepositId() => depositTypeID;

	public Vector2Int GetHeight() => height;
}