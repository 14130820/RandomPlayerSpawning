using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Points;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning {
	internal static class CachedDistances {

		private static readonly Dictionary<uint, RoomWithPoints> CachedOccupiedRooms = new Dictionary<uint, RoomWithPoints>();
		private static readonly List<RoomWithPoints> CachedRoomPos = new List<RoomWithPoints>();
		private static readonly List<DistanceToRoom> CachedSpawnDistances = new List<DistanceToRoom>();
		private static readonly List<PlayerSpawnPoint> CachedFixedPoints = new List<PlayerSpawnPoint>();

		/// <summary>
		/// Will calculate the distance of the players to the spawn point's rooms.
		/// </summary>
		/// <returns>List of 'safe' spawn points.</returns>
		public static List<PlayerSpawnPoint> CalculateSpawns(IReadOnlyList<PlayerSpawnPoint> pointsToCheck, Dictionary<RoleType, float> unsafeData, int pointsRequired = 1) {
			CachedFixedPoints.Clear();

			if (pointsRequired < 1 || unsafeData.Count == 0 || pointsToCheck.Count < pointsRequired) return CachedFixedPoints;

			CachedOccupiedRooms.Clear();
			CachedRoomPos.Clear();
			CachedSpawnDistances.Clear();

			// Get unique rooms from points
			var pointCount = pointsToCheck.Count;
			for (int i = 0; i < pointCount; i++) {
				var point = pointsToCheck[i];
				var room = point.Room;
				var id = room.Id();

				if (!CachedOccupiedRooms.TryGetValue(id, out var roomWithPoints)) {
					roomWithPoints = new RoomWithPoints(room.Transform.position);
					roomWithPoints.Points.Add(point);

					CachedOccupiedRooms.Add(id, roomWithPoints);
					CachedRoomPos.Add(roomWithPoints);
				}

				roomWithPoints.Points.Add(point);
			}

			// Get players that have a distance modifier greater than 10 units.
			var players = Player.List.ToList();
			var playerCount = players.Count;
			for (int i = playerCount - 1; i >= 0; i--)
			{
				var playerRole = players[i].Role;
				if (playerRole == RoleType.Spectator || unsafeData[playerRole] < 10) players.RemoveAt(i);
			}
			playerCount = players.Count;

			// For each room, find the room distance to the closest player.
			var roomCount = CachedRoomPos.Count;

			for (int i = 0; i < roomCount; i++)
			{
				var room = CachedRoomPos[i];
				var roomPos = room.Position;

				float roomFreeDistance = 10000;

				for (int j = 0; j < playerCount; j++)
				{
					var player = players[j];
					var playerPos = player.Position;

					// subtract the safe distance to get a float representing how much distance is left before the room is unsafe.
					var freeDistance = (playerPos - roomPos).sqrMagnitude - unsafeData[player.Role];
					if (freeDistance < roomFreeDistance) roomFreeDistance = freeDistance;
				}

				CachedSpawnDistances.Add(new DistanceToRoom(roomFreeDistance, room.Points));
			}

			if (CachedSpawnDistances.Count == 0) return CachedFixedPoints;

			// Make sure we have enough points
			var distanceToRoomSize = CachedSpawnDistances.Count;
			var distanceModifier = 0;
			bool enoughPoints;

			do {
				CachedFixedPoints.Clear();

				for (int i = 0; i < distanceToRoomSize; i++) {
					var dtr = CachedSpawnDistances[i];

					if (dtr.FreeDistance + distanceModifier >= 0)
						CachedFixedPoints.AddRange(dtr.Points);
				}

				enoughPoints = CachedFixedPoints.Count >= pointsRequired;

				if (!enoughPoints) {
					distanceModifier += 10;
				}

			} while (!enoughPoints);

			return CachedFixedPoints;
		}

		private readonly struct RoomWithPoints {
			public readonly Vector3 Position;

			public RoomWithPoints(Vector3 position) {
				Position = position;
				Points = new List<PlayerSpawnPoint>();
			}

			public readonly List<PlayerSpawnPoint> Points;
		}

		private readonly struct DistanceToRoom {
			public readonly List<PlayerSpawnPoint> Points;
			public readonly float FreeDistance;

			public DistanceToRoom(float freeDistance, List<PlayerSpawnPoint> points)
			{
				FreeDistance = freeDistance;
				Points = points;
			}
		}
	}
}
