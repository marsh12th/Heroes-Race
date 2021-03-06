﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterfall : MonoBehaviour
{
	[Range(-1f, 1f)] public float[] keys;   // La velocidad de cada Key
	public float rotationSpeed;				// La velocidad de rotacion (sobre si mismo)

	SkinnedMeshRenderer mesh;
	float[] values;
	private void Update()
	{
		for ( var k=0; k!=keys.Length; k++ )
		{
			values[k] += keys[k] * 100f * Time.deltaTime;
			mesh.SetBlendShapeWeight (k, values[k]);

			/// Invertir velocidad si se alcanzan minimos/maximos
			if (values[k] <= 0 || values[k] >= 100)
			{
				keys[k] *= -1;
				values[k] = Mathf.Clamp (values[k], 0f, 100f);
			}
		}

		transform.Rotate (Vector3.up, rotationSpeed * Time.deltaTime);
	}

	private void Awake() 
	{
		/// Referencias internas
		mesh = GetComponent<SkinnedMeshRenderer> ();
		values = new float[keys.Length];
	}
}
