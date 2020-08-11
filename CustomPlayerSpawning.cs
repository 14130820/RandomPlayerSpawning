using Exiled.API.Features;
using HarmonyLib;

namespace ArithFeather.CustomPlayerSpawning {
	public class CustomPlayerSpawning : Plugin<Config> {

		//public Spawner Data = new Spawner();

		public override string Author => "Arith";
		public override System.Version Version => new System.Version("2.00");

		private Harmony _harmony = new Harmony("RandomPlayerSpawning");

		public override void OnEnabled() {
			DefaultSpawnEditor.Reload();
			base.OnEnabled();
			_harmony.PatchAll();
			Exiled.Events.Handlers.Server.WaitingForPlayers += DefaultSpawnEditor.CreateNewSpawns;
			Exiled.Events.Handlers.Server.ReloadedConfigs += DefaultSpawnEditor.Reload;
			//Data.Enable();
		}

		public override void OnDisabled() {
			_harmony.UnpatchAll();
			Exiled.Events.Handlers.Server.WaitingForPlayers -= DefaultSpawnEditor.CreateNewSpawns;
			base.OnDisabled();
			//Data.Disable();
		}
	}
}
