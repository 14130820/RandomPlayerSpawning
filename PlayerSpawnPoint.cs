using Exiled.API.Features;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning
{
	public class PlayerSpawnPoint
	{
		public GameObject GameObject;
		public Room Room;

		public PlayerSpawnPoint(GameObject gameObject, Room room)
		{
			GameObject = gameObject;
			Room = room;
		}
	}
}
