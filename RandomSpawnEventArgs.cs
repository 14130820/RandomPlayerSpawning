﻿using Exiled.Events.EventArgs;

namespace ArithFeather.RandomPlayerSpawning {
	public class RandomSpawnEventArgs : SpawningEventArgs {

		public RandomSpawnEventArgs(SpawningEventArgs argument) : base(argument.Player, argument.RoleType, argument.Position, argument.RotationY)
		{

		}

		public bool SpawnRandomly { get; set; } = false;
	}
}
