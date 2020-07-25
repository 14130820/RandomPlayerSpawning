using Exiled.API.Interfaces;

namespace ArithFeather.RandomPlayerSpawning
{
	public class Config : IConfig {
		public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// Randomly spawn everything except SCP.
		/// </summary>
		public static bool UseDefaultSpawnSettings { get; set; } = true;
	}
}
