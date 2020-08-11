using HarmonyLib;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning { 
	[HarmonyPatch(typeof(SpawnpointManager), "GetRandomPosition")]
	public static class GetRandomSpawnPointPatch {

		private static bool Prefix(ref GameObject __result, RoleType classID)
		{
			__result = DefaultSpawnEditor.GetRandomSpawnPoint(classID);
			return false;
		}
	}
}
