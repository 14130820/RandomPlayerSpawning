using Exiled.API.Interfaces;

namespace ArithFeather.CustomPlayerSpawning
{
	public class Config : IConfig {
		public bool IsEnabled { get; set; } = true;

		public bool UseDefaultSafeSpawns { get; set; } = false;
		public int DefaultSafeSpawnDistance { get; set; } = 30;
		public int DefaultEnemySafeSpawnDistance { get; set; } = 60;
	}
}
