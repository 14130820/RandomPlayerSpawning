using ArithFeather.AriToolKit.PointEditor;
using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ArithFeather.AriToolKit;
using UnityEngine;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Random = UnityEngine.Random;
using ServerEvents = Exiled.Events.Handlers.Server;

namespace ArithFeather.CustomPlayerSpawning {
	public class Spawner {
		private const int SafeSpawnDistance = 30;
		private const int EnemySafeSpawnDistance = 60;



		private void CreateSpawns() {

		}

		// Getting Spawns

		// these cached values are used for storing the list of free rooms when a player spawns.
		private readonly List<byte> _cachedOccupiedRooms = new List<byte>();
		private readonly List<TeamPlayer> _cachedPlayerPos = new List<TeamPlayer>(40);

		private class RoomOccupied {
			public bool Occupied { get; set; }

			public readonly Room Room;

			public RoomOccupied(Room room) {
				Room = room;
				Occupied = occupied;
			}
		}

		private class TeamPlayer {
			public TeamPlayer(bool isEnemy, Vector3 position) {
				IsEnemy = isEnemy;
				Position = position;
			}

			public bool IsEnemy { get; }
			public Vector3 Position { get; }
		}

		/// <summary>
		/// Updates all the free spawn points using default max distances to other players.
		/// </summary>
		public void UpdateFreeSpawns() {
			var players = Player.List.ToList();
			var playerCount = players.Count;

			for (int i = 0; i < playerCount; i++) {
				var room = players[i].CurrentRoom.GetCustomRoom().Id;


			}

			CachePlayerDistances();

			UpdateFreeSpawns(SafeSpawnDistance, EnemySafeSpawnDistance);
		}

		private void UpdateFreeSpawns(int distanceToCheck, int enemyDistanceToCheck) {
			var loadedSpawnCount = _playerLoadedSpawns.Count;


			//for (var i = 0; i < loadedSpawnCount; i++) {
			//	var loadedSpawn = PlayerLoadedSpawns[i];
			//	//todo compare room pos to player pos instead of spawn, faster.	
			//	if (!loadedSpawn.IsFreePoint && !IsPlayerInRange(loadedSpawn, distanceToCheck, enemyDistanceToCheck)) {
			//		loadedSpawn.IsFreePoint = true;
			//		CachedFreeRooms.Add(loadedSpawn);
			//	}
			//}

			if (CachedFreeRooms.Count < 1) {
				distanceToCheck = Mathf.Clamp((distanceToCheck - 10), 0, 500);
				enemyDistanceToCheck = Mathf.Clamp((enemyDistanceToCheck - 10), 0, 500);
				UpdateFreeSpawns(distanceToCheck, enemyDistanceToCheck);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsPlayerInRange(PlayerFixedPoint spawn, int range, int enemyRange) {
			var distances = spawn.PlayerDistances;
			var distanceCount = distances.Count;
			for (int i = 0; i < distanceCount; i++) {
				var player = distances[i];
				if ((player.IsEnemy && player.Distance <= enemyRange) || player.Distance <= range) {
					return true;
				}
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CachePlayerDistances() {
			// Cache Player positions
			var players = Player.List.ToList();
			var playerCount = players.Count;
			_cachedPlayerPos.Clear();
			for (int i = 0; i < playerCount; i++) {
				var player = players[i];

				_cachedPlayerPos.Add(new TeamPlayer(player.Team == Team.SCP, player.Position));
			}

			// Cache Player Distances
			var loadedSpawnCount = _playerLoadedSpawns.Count;

			for (var i = 0; i < loadedSpawnCount; i++) {
				var loadedSpawn = _playerLoadedSpawns[i];
				loadedSpawn.IsFreePoint = false;
				var loadedFixedPointPos = loadedSpawn.Position;
				var distances = loadedSpawn.PlayerDistances;
				distances.Clear();

				for (int k = 0; k < playerCount; k++) {
					var cachedValue = _cachedPlayerPos[k];
					var distance = Vector3.Distance(cachedValue.Position, loadedFixedPointPos);

					if ((!cachedValue.IsEnemy || distance <= EnemySafeSpawnDistance) && (cachedValue.IsEnemy || distance <= SafeSpawnDistance)) {
						distances.Add(new PlayerFixedPoint.TeamDistance(cachedValue.IsEnemy, distance));
					}
				}
			}
		}

		#region Spawner

		public delegate void PlayerSpawning(RandomSpawnEventArgs ev);
		public event PlayerSpawning OnRandomPlayerSpawning;

		private void PlayerEvents_Spawning(Exiled.Events.EventArgs.SpawningEventArgs ev) {
			if (NoSpawns) return;

			var argument = new RandomSpawnEventArgs(ev);
			OnRandomPlayerSpawning?.Invoke(argument);

			if (argument.SpawnRandomly) {
				UpdateFreeSpawns();
				var freeRoomCount = CachedFreeRooms.Count;
				if (freeRoomCount > 0) {
					int randomN = Random.Range(0, freeRoomCount);
					var point = CachedFreeRooms[randomN];

					ev.Position = point.Position;
					ev.RotationY = Vector3.Angle(point.Rotation, Vector3.up);
					CachedFreeRooms.RemoveAt(randomN);
				}
			}
		}

		private void Spawner_OnRandomPlayerSpawning(RandomSpawnEventArgs ev) {
			switch (ev.Player.Team) {
				case Team.SCP:
				case Team.TUT:
					ev.SpawnRandomly = false;
					break;
				case Team.MTF:
				case Team.CHI:
				case Team.RSC:
				case Team.CDP:
				case Team.RIP:
					ev.SpawnRandomly = true;
					break;
			}
		}

		#endregion
	}
}