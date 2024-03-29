﻿using BepInEx;
using BepInEx.Configuration;
using Photon.Pun;
using System.IO;
using System.Net;
using UnityEngine;

namespace RGBMonke
{
	[BepInPlugin("org.Sheriff.gorillatag.rgbmonke", "RGB Monke", "1.0.0")]
	public class RGBMonke : BaseUnityPlugin
	{
		public static ConfigEntry<bool> enabled;
		public static ConfigEntry<bool> randomColor;
		public static ConfigEntry<float> cycleSpeed;
		public static ConfigEntry<float> glowAmount; 
		
		private Color color = new Color(0, 0, 0);
		private float hue = 0f;
		private float timer = 0f;
		private float updateRate = 1 / 1;
		private float updateTimer = 0;

		private void Awake()
		{

			Debug.Log("Starting RGB Monke");
			ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "RGBMonke.cfg"), true);

			enabled = config.Bind<bool>("Config", "Enabled", true, "Whether the plugin is enabled or not");
			randomColor = config.Bind<bool>("Config", "RandomColor", false, "Whether to cycle through colours of rainbow or choose random colors");
			cycleSpeed = config.Bind<float>("Config", "CycleSpeed", 0.005f, "The speed the color cycles at each frame (1=Full colour cycle). If random colour is enabled, this is the time in seconds before switching color");
			glowAmount = config.Bind<float>("Config", "GlowAmount", 1.0f, "The brightness of your monkey. The higher the value, the more emissive your monkey is");

		}

		public void Update()
		{
			updateTimer += Time.deltaTime;

			bool Enabled = PhotonNetwork.CurrentRoom == null ? false : !PhotonNetwork.CurrentRoom.IsVisible; 
			if (Enabled)
			{
				if (enabled.Value)
				{
					if (randomColor.Value)
					{
						if (Time.time > timer)
						{
							color = Random.ColorHSV(0, 1, glowAmount.Value, glowAmount.Value, glowAmount.Value, glowAmount.Value);
							timer = Time.time + cycleSpeed.Value;
						}

					}
					else
					{
						if (hue >= 1)
						{
							hue = 0;
						}

						hue += cycleSpeed.Value;
						color = Color.HSVToRGB(hue, 1.0f * glowAmount.Value, 1.0f * glowAmount.Value);
					}

					if (updateTimer > updateRate)
					{
						updateTimer = 0;
					}
					else
					{
						return;
					}

					GorillaTagger.Instance.UpdateColor(this.color.r, this.color.g, this.color.b);
					GorillaTagger.Instance.myVRRig.photonView.RPC("InitializeNoobMaterial", RpcTarget.All, new object[]
					{
						color.r,
						color.g,
						color.b
					});

				}
			}
		}
	}
}