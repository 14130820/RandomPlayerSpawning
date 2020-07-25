using ArithFeather.AriToolKit.PointEditor;
using Exiled.API.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace ArithFeather.RandomPlayerSpawning
{
	public class PlayerSpawnPoint : SpawnPoint
	{
		public PlayerSpawnPoint(string roomType, ZoneType zoneType, Vector3 position, Vector3 rotation) : base(roomType, zoneType, position, rotation) { }

		public List<TeamDistance> PlayerDistances = new List<TeamDistance>(40);
		public bool IsFreePoint;

		public class TeamDistance
		{
			public TeamDistance(bool isEnemy, float distance)
			{
				IsEnemy = isEnemy;
				Distance = distance;
			}

			public bool IsEnemy { get; }
			public float Distance { get; }
		}
	}
}