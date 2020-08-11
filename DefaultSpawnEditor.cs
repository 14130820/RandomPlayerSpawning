using System;
using System.Collections.Generic;
using System.IO;
using ArithFeather.AriToolKit;
using ArithFeather.AriToolKit.Components;
using ArithFeather.AriToolKit.PointEditor;
using Exiled.API.Features;
using UnityEngine;
using Random = UnityEngine.Random;
using SpawnPoint = ArithFeather.AriToolKit.PointEditor.Point;

namespace ArithFeather.CustomPlayerSpawning {
	internal static class DefaultSpawnEditor {
		private const string PlayerFixedPointFileName = "RandomPlayerSpawnData";
		private static readonly string PointDataFilePath = Path.Combine(PointIO.FolderPath, PlayerFixedPointFileName) + ".txt";

		private static readonly Dictionary<RoleType, List<GameObject>> RoleOrganizedSpawnPoints = new Dictionary<RoleType, List<GameObject>>();

		private static bool _spawnFileExists;
		private static PointList _pointList;

		public static void Reload() {
			_pointList = PointAPI.GetPointList(PlayerFixedPointFileName);
			_spawnFileExists = FileManager.FileExists(PointDataFilePath);
		}

		public static void CheckFile() {
			if (!_spawnFileExists) CreateDefaultSpawnPointFile();
		}

		public static void CreateNewSpawns() {
			CheckFile();

			RoleOrganizedSpawnPoints.Clear();

			var groupedSpawns = _pointList.IdGroupedFixedPoints;

			foreach (var pair in groupedSpawns) {
				var key = pair.Key;

				RoleType roleType;
				if (int.TryParse(key, out var roleIndex) && roleIndex >= 0 && roleIndex < 18) {
					roleType = (RoleType)roleIndex;
				} else {
					Log.Error($"Could not convert {key} to a RoleType.");
					continue;
				}

				var spawns = pair.Value;
				var spawnCount = spawns.Count;
				var objects = new List<GameObject>(spawnCount);

				for (int i = 0; i < spawnCount; i++) {
					var spawn = spawns[i];

					var go = new GameObject(spawn.Id);
					go.transform.position = spawn.Position;
					go.transform.rotation = spawn.Rotation;

					objects.Add(go);
				}

				if (RoleOrganizedSpawnPoints.ContainsKey(roleType)) {
					// Replace data if key exists, allows for default game logic or new spawn point lookups.
					RoleOrganizedSpawnPoints[roleType] = objects;
				} else {
					RoleOrganizedSpawnPoints.Add(roleType, objects);

					// Try to add default game data.
					switch (roleType) {
						case RoleType.Scp93953:
							TryRegisterRole(RoleType.Scp93989);
							break;
						case RoleType.NtfCadet:
							TryRegisterRole(RoleType.NtfScientist);
							TryRegisterRole(RoleType.NtfLieutenant);
							TryRegisterRole(RoleType.NtfCommander);
							break;
					}


					void TryRegisterRole(RoleType role) {
						if (!RoleOrganizedSpawnPoints.ContainsKey(role))
							RoleOrganizedSpawnPoints.Add(role, objects);
					}
				}
			}
		}

		public static void CreateDefaultSpawnPointFile() {
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
					_pointList.RawPoints.Add(new SpawnPoint(i.ToString(), room.FixedName,
						room.Room.Zone, position, rotation));
				}
			}

			PointIO.Save(_pointList, PointDataFilePath);
			_pointList.FixData();
		}

		public static GameObject GetRandomSpawnPoint(RoleType role) {
			var objects = RoleOrganizedSpawnPoints[role];
			return objects[Random.Range(0, objects.Count)];
		}
	}
}