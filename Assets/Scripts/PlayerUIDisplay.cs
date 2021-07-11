using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIDisplay : MonoBehaviour {
	public TextMeshProUGUI slotDisplay;

	public void SlotChanged( int slotID ) {
		slotDisplay.text = slotID.ToString();
	}
}