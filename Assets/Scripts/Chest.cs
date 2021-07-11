using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Inventory", menuName = "ScriptableObjects/Inventory" )]
public class Chest : ScriptableObject {
	public Transform[] inventory = new Transform[ 10 ];
}