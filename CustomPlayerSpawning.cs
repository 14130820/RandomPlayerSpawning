using Exiled.API.Features;
using HarmonyLib;

namespace ArithFeather.CustomPlayerSpawning {
	public class CustomPlayerSpawning : Plugin<Config> {

		public override string Author => "Arith";
		public override System.Version Version => new System.Version("2.00");

		private Harmony _harmony = new Harmony("RandomPlayerSpawning");

		public override void OnEnabled() {
			base.OnEnabled();

			SpawnPointCreator.Reload();

			_harmony.PatchAll();

			AriToolKit.PointEditor.PointAPI.OnLoadSpawnPoints += SpawnPointCreator.OrganizeSpawns;
			Exiled.Events.Handlers.Server.ReloadedConfigs += SpawnPointCreator.Reload;
		}

		public override void OnDisabled() {

		}
	}
}
