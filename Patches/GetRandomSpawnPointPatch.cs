using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning.Patches
{
	[HarmonyPatch(typeof(SpawnpointManager), "GetRandomPosition")]
	internal static class GetRandomSpawnPointPatch
	{
		public delegate PlayerSpawnPoint GetRandomSpawnPoint(RoleType role);
		public static event GetRandomSpawnPoint OnGetRandomSpawnPoint;

		public static event SpawnerAPI.PlayerSpawningAtPoint OnPlayerSpawningAtPoint;

		private static bool Prefix(ref GameObject __result, RoleType classID)
		{
			if (!CustomPlayerSpawning.Configs.IsEnabled) return true;

			var spawnPoint = OnGetRandomSpawnPoint?.Invoke(classID);

			if (spawnPoint == null)
			{
				Log.Error("No SpawnPoint set.");
				return true;
			}

			OnPlayerSpawningAtPoint?.Invoke(spawnPoint, classID);

			__result = spawnPoint?.GameObject;
			return __result == null;
		}
	}
}