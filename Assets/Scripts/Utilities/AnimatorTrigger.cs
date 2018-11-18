﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroesRace 
{
	public class AnimatorTrigger : StateMachineBehaviour 
	{
		#region DATA
		public string triggerName;
		private SmartAnimator anim;

		private BoxCollider trigger;
		private bool triggered;

		private static readonly Collider[] hits = new Collider[3];
		private bool initialized;
		#endregion

		public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
		{
			if (!stateInfo.IsTag ("Default")) return;
			// Skip if in transition OR already hit the trigger
			if (triggered || animator.IsInTransition (0)) return;

			// Manual OnTriggerEnter
			var t = animator.transform;
			int n = Physics.OverlapBoxNonAlloc (t.position + trigger.center, trigger.size / 2f, hits, t.rotation, 1 << 8);

			for (int i=0; i!=n; ++i)
			{
				if (hits[i].tag == "Player") 
				{
					anim.SetTrigger (triggerName);
					triggered = true;
					break;
				}
			}
		}

		public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
		{
			// Get first-time references
			if (Net.IsServer && !initialized)
			{
				Initialize (animator);
				initialized = true;
			}
			else
			// Useless on Clients
			if (Net.IsClient)
			{
				Destroy (this);
				return;
			}

			// Resume trigger checking
			if (stateInfo.IsTag ("Default"))
			{
				anim.SetTrigger (triggerName, reset: true);
				triggered = false;
			}
		}

		private void Initialize (Animator animator) 
		{
			anim = animator.GoSmart (networked: true);
			anim.NetAnimator.SetParameterAutoSend (0, true);
			trigger = animator.GetComponent<BoxCollider> ();
		}
	} 
}