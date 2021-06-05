using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( Planet ) )]
public class PlanetEditor : Editor {
	private Planet planet;
	private Editor shapeEditor;
	private Editor colourEditor;

	public override void OnInspectorGUI() {
		using ( var check = new EditorGUI.ChangeCheckScope() ) {
			base.OnInspectorGUI();
			if ( check.changed )
				planet.GeneratePlanet();
		}

		if ( GUILayout.Button( "Generate Planet" ) ) {
			planet.GeneratePlanet();
		}

		DrawSetttingsEditor( planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor );
		DrawSetttingsEditor( planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor );
	}

	private void DrawSetttingsEditor( Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor ) {
		if ( settings != null ) {
			foldout = EditorGUILayout.InspectorTitlebar( foldout, settings );

			using ( var check = new EditorGUI.ChangeCheckScope() ) {
				if ( foldout ) {
					CreateCachedEditor( settings, null, ref editor );
					editor.OnInspectorGUI();

					if ( check.changed ) {
						if ( onSettingsUpdated != null )
							onSettingsUpdated();
					}
				}
			}
		}
	}

	private void OnEnable() {
		planet = ( Planet )target;
	}
}