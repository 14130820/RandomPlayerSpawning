using Exiled.API.Features;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning
{
	public class PlayerSpawnPoint
	{
		public readonly GameObject GameObject;
		public readonly Room Room;

		public PlayerSpawnPoint(GameObject gameObject, Room room)
		{
			GameObject = gameObject;
			Room = room;
		}
	}
}
