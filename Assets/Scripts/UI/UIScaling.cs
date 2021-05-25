using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScaling : MonoBehaviour {
	private Vector2 screenSize;
	private Vector2 scaling;

	[SerializeField] private RectTransform toolBar;

	[Range( 0f, 1f )]
	[SerializeField] private float toolBarPositionPrecentage;

	private void Start() {
		screenSize = Screen.safeArea.size;

		scaling = new Vector2( Screen.safeArea.width / Screen.currentResolution.width, Screen.safeArea.height / Screen.currentResolution.height );

		SetUIElements();
	}

	private void LateUpdate() {
		if ( screenSize != Screen.safeArea.size ) {
			SetUIElements();
		}
	}

	private void SetUIElements() {
		toolBar.anchoredPosition = SetPosition( toolBarPositionPrecentage );
		toolBar.localScale = SetScale();
	}

	private Vector2 SetPosition( float screenPercentage ) {
		if ( screenSize.y >= 0 )
			screenSize.y *= -1;

		return screenSize * screenPercentage;
	}

	private Vector2 SetScale() {
		float scale = 1f;
		if ( scaling.x > scaling.y )
			scale = scaling.x;
		else
			scale = scaling.y;
		return new Vector2( scale, scale );
	}
}