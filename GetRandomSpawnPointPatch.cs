using HarmonyLib;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning {
	[HarmonyPatch(typeof(SpawnpointManager), "GetRandomPosition")]
	internal static class GetRandomSpawnPointPatch {
		public delegate PlayerSpawnPoint GetRandomSpawnPoint(RoleType role);
		public static event GetRandomSpawnPoint OnGetRandomSpawnPoint;

		private static bool Prefix(ref GameObject __result, RoleType classID)
		{
			var spawnPoint = OnGetRandomSpawnPoint?.Invoke(classID);
			__result = spawnPoint?.GameObject;
			return __result == null;
		}
	}
}
