using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter {
	private NoiseSettings.SimpleNoiseSettings settings;
	private Noise noise = new Noise();

	public SimpleNoiseFilter( NoiseSettings.SimpleNoiseSettings settings ) {
		this.settings = settings;
	}

	public float Evaluate( Vector3 point ) {
		float noiseValue = 0;
		float fequency = settings.baseRoughness;
		float amplitude = 1;

		for ( int i = 0; i < settings.numLayers; i++ ) {
			float v = noise.Evaluate( point * fequency + settings.centre );
			noiseValue += ( v + 1 ) * .5f * amplitude;
			fequency *= settings.roughness;
			amplitude *= settings.persistence;
		}

		noiseValue = Mathf.Max( 0, noiseValue - settings.minValue );

		return noiseValue * settings.strength;
	}
}