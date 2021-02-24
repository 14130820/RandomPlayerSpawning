using ArithFeather.CustomPlayerSpawning.Patches;

namespace ArithFeather.CustomPlayerSpawning
{
	public static class SpawnerAPI
	{
		static SpawnerAPI()
		{
			GetRandomSpawnPointPatch.OnPlayerSpawningAtPoint += (a, b) => OnPlayerSpawningAtPoint?.Invoke(a,b);
		}

		/// <summary>
		/// Apply your custom settings on OnEnabled() or ignore to use default game settings.
		/// </summary>
		/// <param name="settings"><see cref="SpawnSettings"/>"/></param>
		public static void ApplySettings(SpawnSettings settings) =>
			CustomPlayerSpawning.SpawnSettings = settings;

		/// <summary>
		/// This should be called once every time before you perform spawning on a specific role.
		/// It makes sure there are enough available spawn points for your spawns, using the <see cref="SpawnSettings"/>.
		/// </summary>
		/// <param name="role">The role of the player(s) you are spawning</param>
		/// <param name="numberOfSpawns">The number of players you are going to spawn.</param>
		public static void FilterSpawns(RoleType role, int numberOfSpawns = 1) =>
			Spawner.FilterSpawns(role, numberOfSpawns);

		/// <summary>
		/// Call this when you are done spawning to make sure other spawning mechanics work. (Like console teleport).
		/// </summary>
		public static void EndSpawning() => Spawner.EndTeamRespawn();

		public delegate void PlayerSpawningAtPoint(PlayerSpawnPoint playerSpawnPoint, RoleType role);

		/// <summary>
		/// Invokes right before the player spawns, giving you the spawn point information.
		/// </summary>
		public static event PlayerSpawningAtPoint OnPlayerSpawningAtPoint;

		/// <summary>
		/// This will attempt to get a random spawn point.
		/// </summary>
		public static PlayerSpawnPoint GetRandomSpawnPoint(RoleType role) =>
			Spawner.GetRandomSpawnPointPatch_OnGetRandomSpawnPoint(role);
	}
}
