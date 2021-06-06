using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter {
	private NoiseSettings.RigidNoiseSettings settings;
	private Noise noise = new Noise();

	public RigidNoiseFilter( NoiseSettings.RigidNoiseSettings settings ) {
		this.settings = settings;
	}

	public float Evaluate( Vector3 point ) {
		float noiseValue = 0;
		float fequency = settings.baseRoughness;
		float amplitude = 1;
		float weight = 1;

		for ( int i = 0; i < settings.numLayers; i++ ) {
			float v = 1 - Mathf.Abs( noise.Evaluate( point * fequency + settings.centre ) );
			v *= v;
			v *= weight;
			weight = Mathf.Clamp01( v * settings.weightMultipler );// clamps 0-1

			noiseValue += v * amplitude;
			fequency *= settings.roughness;
			amplitude *= settings.persistence;
		}

		noiseValue = Mathf.Max( 0, noiseValue - settings.minValue );

		return noiseValue * settings.strength;
	}
}