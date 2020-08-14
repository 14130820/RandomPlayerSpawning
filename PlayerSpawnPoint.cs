using ArithFeather.AriToolKit.Components;
using UnityEngine;

namespace ArithFeather.CustomPlayerSpawning {
	public class PlayerSpawnPoint
	{
		public readonly GameObject GameObject;
		public readonly CustomRoom Room;

		public PlayerSpawnPoint(GameObject gameObject, CustomRoom room)
		{
			GameObject = gameObject;
			Room = room;
		}
	}
}
