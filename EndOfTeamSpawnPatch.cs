using HarmonyLib;
using Respawning;

namespace ArithFeather.CustomPlayerSpawning
{
	[HarmonyPatch(typeof(RespawnManager), "Spawn")]
	internal static class EndOfTeamSpawnPatch
	{
		public delegate void EndTeamRespawn();
		public static event EndTeamRespawn OnEndTeamRespawn;

		private static void Postfix()
		{
			if (!CustomPlayerSpawning.Configs.IsEnabled) return;
			OnEndTeamRespawn?.Invoke();
		}
	}
}
