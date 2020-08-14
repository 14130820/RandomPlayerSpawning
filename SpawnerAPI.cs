namespace ArithFeather.CustomPlayerSpawning {
	public static class SpawnerAPI
	{
		/// <summary>
		/// Apply your custom settings on OnEnabled() or leave to use default game settings.
		/// </summary>
		/// <param name="settings"><see cref="SpawnSettings"/>"/></param>
		public static void ApplySettings(SpawnSettings settings) =>
			CustomPlayerSpawning.SpawnSettings = settings;

		/// <summary>
		/// This should be called once every time before you perform spawning on a specific role.
		/// </summary>
		/// <param name="role">The role of the player(s) you are spawning</param>
		/// <param name="numberOfSpawns">The number of players you are going to spawn.</param>
		public static void FilterSpawns(RoleType role, int numberOfSpawns = 1) =>
			Spawner.FilterSpawns(role, numberOfSpawns);

		/// <summary>
		/// Call this when you are done spawning to make sure other spawning mechanics work. (Like console teleport).
		/// </summary>
		public static void EndSpawning() => Spawner.EndTeamRespawn();
	}
}
