using System;
using System.Collections.Generic;
using System.IO;
using ArithFeather.AriToolKit;
using ArithFeather.AriToolKit.Components;
using ArithFeather.AriToolKit.PointEditor;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;
using SpawnPoint = ArithFeather.AriToolKit.PointEditor.Point;
using ServerEvents = Exiled.Events.Handlers.Server;

namespace ArithFeather.CustomPlayerSpawning {
	public class CustomPlayerSpawning : Plugin<Config> {
		private const string PlayerFixedPointFileName = "CustomPlayerSpawns";

		public static Config Configs;
		public static readonly int RoleTypeSize = Enum.GetNames(typeof(RoleType)).Length;
		private static readonly string PointDataFilePath = Path.Combine(PointIO.FolderPath, PlayerFixedPointFileName) + ".txt";

		private bool _spawnFileExists;
		private PointList _pointList;

		// Loaded every game
		public static readonly Dictionary<RoleType, IReadOnlyList<PlayerSpawnPoint>> RoleGameObjectDictionary = new Dictionary<RoleType, IReadOnlyList<PlayerSpawnPoint>>();
		public static SpawnSettings SpawnSettings { get; set; }

		public override string Author => "Arith";
		public override Version Version => new Version("2.03");
		public override PluginPriority Priority => PluginPriority.Lowest; // Make sure this team spawn event is tested last

		private readonly Harmony _harmony = new Harmony("RandomPlayerSpawning");

		public override void OnEnabled()
		{
			Configs = Config;
			base.OnEnabled();

			_harmony.PatchAll();

			PointAPI.OnLoadSpawnPoints += OrganizeSpawns;
			ServerEvents.ReloadedConfigs += ReloadConfig;
			ServerEvents.RespawningTeam += Spawner.ServerEvents_RespawningTeam;
			GetRandomSpawnPointPatch.OnGetRandomSpawnPoint += Spawner.GetRandomSpawnPointPatch_OnGetRandomSpawnPoint;
			EndOfTeamSpawnPatch.OnEndTeamRespawn += Spawner.EndTeamRespawn;

			ReloadConfig();
		}

		public override void OnDisabled() {
			PointAPI.OnLoadSpawnPoints -= OrganizeSpawns;
			ServerEvents.ReloadedConfigs -= ReloadConfig;
			ServerEvents.RespawningTeam -= Spawner.ServerEvents_RespawningTeam;
			GetRandomSpawnPointPatch.OnGetRandomSpawnPoint -= Spawner.GetRandomSpawnPointPatch_OnGetRandomSpawnPoint;
			EndOfTeamSpawnPatch.OnEndTeamRespawn -= Spawner.EndTeamRespawn;

			base.OnDisabled();
		}

		private void ReloadConfig() {
			_pointList = PointAPI.GetPointList(PlayerFixedPointFileName);
			_spawnFileExists = FileManager.FileExists(PointDataFilePath);
		}

		private void OrganizeSpawns() {
			if (!_spawnFileExists) CreateDefaultSpawnPointFile();
			if (SpawnSettings == null) SpawnSettings = GetDefaultSpawnSettings();

			RoleGameObjectDictionary.Clear();

			// For each role, get who they share spawns with.
			for (int i = 0; i < RoleTypeSize; i++) {
				var role = (RoleType)i;

				// Make sure the role isn't already populated
				if (RoleGameObjectDictionary.ContainsKey(role)) continue;

				var sharedRoles = SpawnSettings.PopulatedSharedSpawnInfo[role];
				var sharedRolesSize = sharedRoles.Count;

				var sharedSpawnPoints = new List<PlayerSpawnPoint>();

				// Create a list of GameObjects using those shared spawn points.
				for (int j = 0; j < sharedRolesSize; j++) {
					var sharedRole = sharedRoles[j];
					if (_pointList.IdGroupedFixedPoints.TryGetValue(((int)sharedRole).ToString(), out var spawnList)) {

						var spawnCount = spawnList.Count;

						for (int k = 0; k < spawnCount; k++) {
							var spawn = spawnList[k];

							var go = new GameObject(spawn.Id);
							go.transform.position = spawn.Position;
							go.transform.rotation = spawn.Rotation;

							sharedSpawnPoints.Add(new PlayerSpawnPoint(go, spawn.CustomRoom));
						}
					}
				}

				if (sharedSpawnPoints.Count == 0) continue;

				// Assign the shared roles the Game Objects.
				for (int j = 0; j < sharedRolesSize; j++) {
					var sharedRole = sharedRoles[j];

					RoleGameObjectDictionary.Add(sharedRole, sharedSpawnPoints);
				}
			}
		}

		private SpawnSettings GetDefaultSpawnSettings() {
			var spawnSettings = new SpawnSettings();
			spawnSettings.DefineSharedSpawns(RoleType.Scp93953, RoleType.Scp93989);
			spawnSettings.DefineSharedSpawns(RoleType.NtfScientist, RoleType.NtfCadet, RoleType.NtfCommander, RoleType.NtfLieutenant);

			if (Config.UseDefaultSafeSpawns)
			{
				spawnSettings.DefineSafeSpawnDistances(new DistanceCheckInfo(
					new[] { RoleType.ChaosInsurgency, RoleType.ClassD },
					Config.DefaultSafeSpawnDistance, Config.DefaultEnemySafeSpawnDistance));

				spawnSettings.DefineSafeSpawnDistances(new DistanceCheckInfo(
					new[] { RoleType.NtfCadet, RoleType.NtfCommander, RoleType.NtfLieutenant, RoleType.NtfScientist, RoleType.Scientist, RoleType.FacilityGuard },
					Config.DefaultSafeSpawnDistance, Config.DefaultEnemySafeSpawnDistance));

				spawnSettings.DefineSafeSpawnDistances(new DistanceCheckInfo(
					new[] { RoleType.Scp93953, RoleType.Scp049, RoleType.Scp096, RoleType.Scp106, RoleType.Scp93989, RoleType.Scp173 },
					Config.DefaultSafeSpawnDistance, Config.DefaultEnemySafeSpawnDistance));
			}

			return spawnSettings;
		}

		private void CreateDefaultSpawnPointFile() {
			Log.Warn("Creating default CustomPlayerSpawns file.");
			var roleSize = Enum.GetNames(typeof(RoleType)).Length - 1;

			for (int i = 0; i < roleSize; i++) {
				var roleType = (RoleType)i;

				GameObject[] spawns = GetDefaultSpawnPoints(roleType);

				GameObject[] GetDefaultSpawnPoints(RoleType role) {
					switch (role) {
						case RoleType.Scp106: return GameObject.FindGameObjectsWithTag("SP_106");
						case RoleType.Scp049: return GameObject.FindGameObjectsWithTag("SP_049");
						case RoleType.Scp079: return GameObject.FindGameObjectsWithTag("SP_079");
						case RoleType.Scp096: return GameObject.FindGameObjectsWithTag("SCP_096");
						case RoleType.Scp173: return GameObject.FindGameObjectsWithTag("SP_173");
						case RoleType.Scp93953: return GameObject.FindGameObjectsWithTag("SCP_939");
						case RoleType.FacilityGuard: return GameObject.FindGameObjectsWithTag("SP_GUARD");
						case RoleType.NtfCadet: return GameObject.FindGameObjectsWithTag("SP_MTF");
						case RoleType.ChaosInsurgency: return GameObject.FindGameObjectsWithTag("SP_CI");
						case RoleType.Scientist: return GameObject.FindGameObjectsWithTag("SP_RSC");
						case RoleType.ClassD: return GameObject.FindGameObjectsWithTag("SP_CDP");
						case RoleType.Tutorial: return new[] { GameObject.Find("TUT Spawn") };
						default: return null;
					}
				}

				if (spawns == null) continue;

				var spawnCount = spawns.Length;

				for (int j = 0; j < spawnCount; j++) {
					var spawnPoint = spawns[j];
					var room = spawnPoint.GetComponentInParent<CustomRoom>();

					if (room == null) {
						room = Rooms.CustomRooms[Rooms.CustomRooms.Count - 1];
					}

					var spawnTransform = spawnPoint.transform;
					var roomTransform = room.transform;
					var position = roomTransform.InverseTransformPoint(spawnTransform.position);
					var rotation = roomTransform.InverseTransformDirection(spawnTransform.eulerAngles);
					_pointList.RawPoints.Add(new SpawnPoint(i.ToString(), room.Room.Type,
						room.Room.Zone, position, rotation));
				}
			}

			PointIO.Save(_pointList, PointDataFilePath);
			_pointList.FixData();
			_spawnFileExists = true;
		}
	}
}
