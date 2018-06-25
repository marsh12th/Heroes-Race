﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace HeroesRace 
{
	public static class Extensions
	{
		#region ANIMATORS
		public static SmartAnimator GoSmart (this Animator a, bool networked = false) 
		{
			var anim = new SmartAnimator (a, networked);
			return anim;
		}
		#endregion

		#region BEHAVIOUR
		private static object thisLock = new object ();
		public static T SpawnSingleton<T> () where T : Behaviour
		{
			// Keep it thread-safe
			lock (thisLock)
			{
				// Be sure there isn't any other on scene
				var intruder = UnityEngine.Object.FindObjectOfType<T> ();
				if (intruder) throw new UnityException ("Requested type is already spawned on scene!");

				// Locate prefab
				var prefab = Resources.Load<T> ("Prefabs/" + typeof (T).Name);
				if (prefab != null)
				{
					var go = UnityEngine.Object.Instantiate (prefab);
					go.name = "[Singleton] " + typeof (T).Name;
					UnityEngine.Object.DontDestroyOnLoad (go);

					return go;
				}
				else throw new UnityException ("Prefab asset not found!");
			}
		}
		#endregion

		#region ENUM 
		public static T EnumParse<T> (this string s) where T : struct, IConvertible
		{
			return (T)Enum.Parse (typeof (T), s);
		}

		/// <summary>
		/// Usage: "if ( someEnum.HasFlag (someEnumFlag) ) {..}"
		/// </summary>
		public static bool HasFlag<T> (this T e, T flag) where T : struct, IConvertible
		{
			var value = e.ToInt32 (CultureInfo.InvariantCulture);
			var target = flag.ToInt32 (CultureInfo.InvariantCulture);

			return ((value & target) == target);
		}

		/// <summary>
		/// Usage: "someEnum = someEnum.SetFlag (someEnumFlag);"
		/// </summary>
		public static T SetFlag<T> (this T e, T flag) where T : struct, IConvertible
		{
			var value = e.ToInt32 (CultureInfo.InvariantCulture);
			var newFlag = flag.ToInt32 (CultureInfo.InvariantCulture);

			return (T)(object)(value | newFlag);
		}

		/// <summary>
		/// Usage: "someEnum = someEnum.UnsetFlag (someEnumFlag);"
		/// </summary>
		public static T UnsetFlag<T> (this T en, T flag) where T : struct, IConvertible
		{
			int value = en.ToInt32 (CultureInfo.InvariantCulture);
			int newFlag = flag.ToInt32 (CultureInfo.InvariantCulture);

			return (T)(object)(value & ~newFlag);
		}
		#endregion

		#region ANIMATION
		public static void PlayRewind (this Animation a, string clip) 
		{
			a[clip].normalizedTime = 0f;
			a[clip].speed = 1f;
			a.Play (clip);
		}

		public static void PlayInReverse (this Animation a, string clip) 
		{
			a[clip].normalizedTime = 1f;
			a[clip].speed *= -1f;
			a.Play (clip);
		}
		#endregion

		#region pfff
		public static IEnumerator WaitAnimator (this Animator a, string animation, int layer = 0)
		{
			while (a.GetCurrentAnimatorStateInfo (layer).IsName (animation))
				yield return null;
		}
		#endregion
	} 
}
