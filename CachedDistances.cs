using System.Collections.Generic;
using System.Linq;
using ArithFeather.Points;
using ArithFeather.Points.Tools;
using Exiled.API.Features;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning
{
	internal static class CachedDistances
	{
		private static readonly List<DistanceToPoint> CachedSpawnDistances = new List<DistanceToPoint>();
		private static readonly List<PlayerSpawnPoint> CachedFixedPoints = new List<PlayerSpawnPoint>();

		/// <summary>
		/// Will calculate the distance of the players to the spawn point's rooms.
		/// </summary>
		/// <returns>List of 'safe' spawn points.</returns>
		public static List<PlayerSpawnPoint> CalculateSpawns(IReadOnlyList<PlayerSpawnPoint> pointsToCheck, Dictionary<RoleType, float> unsafeData, int pointsRequired = 1)
		{

			if (pointsRequired < 1 || unsafeData.Count == 0 || pointsToCheck.Count < pointsRequired) return pointsToCheck.ToList();

			CachedSpawnDistances.Clear();

			// Get players that have a distance modifier greater than 10 units.
			var players = Player.List.ToList();
			var playerCount = players.Count;
			for (int i = playerCount - 1; i >= 0; i--)
			{
				var playerRole = players[i].Role;
				if (playerRole == RoleType.Spectator || unsafeData[playerRole] < 10) players.RemoveAt(i);
			}
			playerCount = players.Count;

			// For each point, find the closest player
			var pointCount = pointsToCheck.Count;

			for (int i = 0; i < pointCount; i++)
			{
				var point = pointsToCheck[i];
				var pointPos = point.GameObject.transform.position;

				float pointFreeDistance = 10000;

				for (int j = 0; j < playerCount; j++)
				{
					var player = players[j];
					var playerPos = player.Position;

					// subtract the safe distance to get a float representing how much distance is left before the room is unsafe.
					var freeDistance = (playerPos - pointPos).sqrMagnitude - unsafeData[player.Role];
					if (freeDistance < pointFreeDistance) pointFreeDistance = freeDistance;
				}

				CachedSpawnDistances.Add(new DistanceToPoint(pointFreeDistance, point));
			}

			if (CachedSpawnDistances.Count == 0) return pointsToCheck.ToList();

			// Make sure we have enough points
			var distanceToRoomSize = CachedSpawnDistances.Count;
			var distanceModifier = -10;
			bool enoughPoints;

			do
			{
				distanceModifier += 10;
				CachedFixedPoints.Clear();

				for (int i = 0; i < distanceToRoomSize; i++)
				{
					var dtr = CachedSpawnDistances[i];

					if (dtr.FreeDistance + distanceModifier >= 0)
						CachedFixedPoints.Add(dtr.Point);
				}

				enoughPoints = CachedFixedPoints.Count >= pointsRequired;

			} while (!enoughPoints);

			return CachedFixedPoints;
		}

		private readonly struct DistanceToPoint
		{
			public readonly PlayerSpawnPoint Point;
			public readonly float FreeDistance;

			public DistanceToPoint(float freeDistance, PlayerSpawnPoint point)
			{
				FreeDistance = freeDistance;
				Point = point;
			}
		}
	}
}
