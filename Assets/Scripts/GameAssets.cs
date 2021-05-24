using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour {
	public static GameAssets Instance { get; private set; }

	private void Awake() {
		Instance = this;
	}

	public ItemData[] items;
	public BiomeData[] biomes;

	public Material[] materials;

	[Header( "Settings" )]
	public int seed;

	public string worldName;

	public Transform droppedItemPf;
}