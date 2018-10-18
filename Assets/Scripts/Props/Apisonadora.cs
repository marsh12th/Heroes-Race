﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HeroesRace 
{
	public class Apisonadora : NetBehaviour 
	{
		#region DATA
		public Material[] variants;

		private float waitTime;
		private bool readyToHit;

		internal SmartAnimator anim;
		private static int count;
		#endregion

		private IEnumerator Hit () 
		{
			float mark = Time.time + waitTime;
			while (Time.time <= mark) yield return null;
			anim.SetTrigger ("Hit");

			while (!anim.IsInState ("None")) yield return null;
			readyToHit = true;
		}

		[ServerCallback]
		private void Update () 
		{
			if (readyToHit)
			{
				StartCoroutine (Hit ());
				readyToHit = false;
			}
		}

		protected override void OnAwake () 
		{
			if (count == 3) count = 0;
			var variant = variants[count++];
			GetComponentInChildren<Renderer> ().sharedMaterial = variant;

			if (NetworkServer.active) 
			{
				anim = GetComponent<Animator> ().GoSmart (networked: true);
				waitTime = Random.Range (0.75f, 1.5f);
				readyToHit = true;
			}
		}
	} 
}
