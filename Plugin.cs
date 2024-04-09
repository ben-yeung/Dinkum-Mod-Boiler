using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace PluginName {
	[BepInAutoPlugin]
	public partial class ABPlugin : BaseUnityPlugin {
		private readonly Harmony harmony = new Harmony(Id);
		public static ABPlugin Instance;
		private ConfigEntry<bool> _isEnabled;
		private ConfigEntry<KeyCode> _toggleButton;

		private void Awake() {
			Instance = this;
			this._isEnabled = this.Config.Bind<bool>("_General", "Enabled", true, "Enable this plugin? Note: See below for the keybind to toggle this in game");
			this._toggleButton = this.Config.Bind<KeyCode>("Bindings", "Toggle_Key", KeyCode.B, "Keybind for enabling and disabling the plugin in-game.");
			
			
			try {
				this.harmony.PatchAll();
				ABPlugin.Log($"Plugin PluginName is loaded!");
			}
			catch (Exception err) {
				ABPlugin.Log($"Error during Awake: {err?.ToString()}");
			}
		}

		private void Update() {
			CharMovement localChar = NetworkMapSharer.Instance.localChar;
			if (!((object) localChar != (object) null) || !localChar.isLocalPlayer || Inventory.Instance.menuOpen || ChatBox.chat.chatOpen || !Inventory.Instance.CanMoveCharacter())
					return;

			if (Input.GetKeyDown(this._toggleButton.Value)) {
				this._isEnabled.Value = !this._isEnabled.Value;
				if (this._isEnabled.Value) {
					NotificationManager.manage.createChatNotification($"Mod is now enabled");
				} else {
      				NotificationManager.manage.createChatNotification($"Mod is now disabled");
				}
			}

		}
		public static bool checkEnabled() {
			return Instance._isEnabled.Value;
		}

		public static void Log(System.Object msg) {
			Instance.Logger.LogInfo(msg);
		}

	}

	// When a lobby creation is detected then set the user to be the host
	[HarmonyPatch(typeof(SteamLobby), "CreateLobbyWithSettings")]
	public static class ExamplePostFix {
		[HarmonyPostfix]
		static void Init() {
			ABPlugin.Log($"Detected lobby create");
		}
	}

}