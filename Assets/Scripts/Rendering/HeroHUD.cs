﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace HeroesRace 
{
	public class HeroHUD: MonoBehaviour 
	{
		public Sprite[] sprites;
		public Image powerUp;

		public void UpdatePower (PowerUp power) 
		{
			if (power != PowerUp.None)
			{
				int idx = (int) power - 1;
				powerUp.sprite = sprites[idx];
				powerUp.enabled = true;
			}
			else powerUp.enabled = false;
			#warning En el futuro, quizá hacer esto más dinámico
		}
	} 
}
