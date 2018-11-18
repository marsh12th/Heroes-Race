﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HeroesRace 
{
	public class HeroCamera : MonoBehaviour 
	{
		#region DATA
		internal HeroHUD hud;
		[Info] public Hero target;

		private Vector3 actualOffset;
		public readonly Vector3 offset = new Vector3 (2.78f, 1.4f, 9.25f);
		public const float FloorHeigth = 5.2f;


		internal Vector3 @override;
		#endregion

		private void Update () 
		{
			if (!target) return;

			// Project [tower->hero] over XZ plane
			var forward = target.transform.position;
			forward.y = 0f; forward.Normalize ();

			// Compute the rotation matrix to lately extract local offset based
			// on a space that always looks outside of the circle 
			var mat = Matrix4x4.Rotate (Quaternion.LookRotation (forward));

			if (@override == Vector3.zero)
			{
				// Lerp side-direction
				actualOffset.x = Mathf.Lerp
				(
					actualOffset.x,
					(target.movingDir > 0f) ? +offset.x : -offset.x,
					Time.deltaTime * 2f
				);
				// Lerp floor-height
				actualOffset.y = Mathf.Lerp
				(
					actualOffset.y,
					offset.y + (target.floor * FloorHeigth),
					Time.deltaTime * 3f
				);
			}
			else actualOffset = @override;
			
			// Get the final position (make Height inmutable)
			var pos = target.transform.position; /* */ pos.y = offset.y;
			pos += mat.MultiplyVector (actualOffset);

			// Lerp the position for a smooth camera follow
			transform.position = Vector3.Lerp (transform.position, pos, Time.deltaTime * 7f);

			// Project camera position to get the orientation
			var camForward = transform.position; camForward.y = 0f;
			transform.rotation = Quaternion.LookRotation (-camForward.normalized);
		}

		private void Awake () 
		{
			actualOffset = offset;
			hud = Instantiate (Resources.Load<HeroHUD> ("Prefabs/HUD"));
			hud.name = hud.name.Replace ("(Clone)", "");
		}
	} 
}
