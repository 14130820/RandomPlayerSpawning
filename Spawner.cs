using System.Collections.Generic;
using System.Linq;
using Respawning;
using Random = UnityEngine.Random;

namespace ArithFeather.CustomPlayerSpawning
{
	internal static class Spawner
	{
		private static Dictionary<RoleType, IReadOnlyList<PlayerSpawnPoint>> RoleGameObjectDictionary =>
			CustomPlayerSpawning.RoleGameObjectDictionary;

		private static Dictionary<RoleType, Dictionary<RoleType, float>> DistanceInfo =>
			CustomPlayerSpawning.SpawnSettings.PopulatedDistanceInfo;

		private static List<PlayerSpawnPoint> _filteredSpawns = new List<PlayerSpawnPoint>();
		private static int _filteredSpawnIndex;
		private static bool _useFilteredSpawns;

		public static void EndTeamRespawn() => _useFilteredSpawns = false;

		public static PlayerSpawnPoint GetRandomSpawnPointPatch_OnGetRandomSpawnPoint(RoleType role)
		{
			// Try to get a filtered spawn
			if (_useFilteredSpawns)
			{
				_useFilteredSpawns = false;
				if (TryGetRandomFilteredSpawn(out var spawn)) return spawn;
			}

			// Try to get a normal spawn
			if (RoleGameObjectDictionary.TryGetValue(role, out var spawns) && spawns.Count != 0) return spawns[Random.Range(0, spawns.Count)];

			return new PlayerSpawnPoint(null, null);
		}

		private static bool TryGetRandomFilteredSpawn(out PlayerSpawnPoint filteredSpawn)
		{
			var filteredSpawnsCount = _filteredSpawns.Count;

			if (filteredSpawnsCount != 0)
			{
				if (_filteredSpawnIndex == _filteredSpawns.Count)
					_filteredSpawnIndex = 0;

				filteredSpawn = _filteredSpawns[_filteredSpawnIndex];
				_filteredSpawnIndex++;
				return true;
			}

			filteredSpawn = new PlayerSpawnPoint(null, null);
			return false;
		}

		public static void ServerEvents_RespawningTeam(Exiled.Events.EventArgs.RespawningTeamEventArgs ev)
		{
			// Default game logic 
			var spawningPlayers = ev.Players;
			var spawningPlayersCount = spawningPlayers.Count;

			if (spawningPlayersCount == 0) return;

			FilterSpawns(ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency
				? RoleType.ChaosInsurgency
				: RoleType.NtfCadet,
				spawningPlayersCount);
		}

		/// <summary>
		/// Populates <see cref="_filteredSpawns"/> based on <see cref="SpawnSettings"/>
		/// </summary>
		/// <param name="role"></param>
		/// <param name="numberOfPoints"></param>
		public static void FilterSpawns(RoleType role, int numberOfPoints = 1)
		{
			// Get spawns for role
			var spawns = RoleGameObjectDictionary[role];
			var spawnCount = spawns.Count;

			if (spawnCount == 0) return;

			// Filter spawns based on distances
			_filteredSpawns = DistanceInfo.TryGetValue(role, out var roleDistances) ?
				CachedDistances.CalculateSpawns(spawns, roleDistances, numberOfPoints) : spawns.ToList();

			CustomPlayerSpawning.UnityShuffle(_filteredSpawns);
			_filteredSpawnIndex = 0;
			_useFilteredSpawns = true;
		}
	}
}