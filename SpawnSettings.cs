using System.Collections.Generic;

namespace ArithFeather.CustomPlayerSpawning
{
	/// <summary>
	/// Holds data to define how the game will handle spawning players.
	/// </summary>
	public class SpawnSettings
	{
		internal readonly Dictionary<RoleType, List<RoleType>> PopulatedSharedSpawnInfo = new Dictionary<RoleType, List<RoleType>>();
		internal readonly Dictionary<RoleType, Dictionary<RoleType, float>> PopulatedDistanceInfo = new Dictionary<RoleType, Dictionary<RoleType, float>>();

		public SpawnSettings()
		{
			// Populate Role Groups
			for (int i = 0; i < CustomPlayerSpawning.RoleTypeSize; i++)
			{
				var role = (RoleType)i;
				PopulatedSharedSpawnInfo.Add(role, new List<RoleType> { role });
			}
		}

		/// <summary>
		/// Define one of these for each team where you want the team to spawn a certain distance away from the enemy team or your own team.
		/// </summary>
		/// <param name="distanceCheckInfo"></param>
		public void DefineSafeSpawnDistances(DistanceCheckInfo distanceCheckInfo)
		{
			var myTeam = distanceCheckInfo.MyTeam;
			var counter = myTeam.Length;
			for (int i = 0; i < counter; i++)
			{
				var myRole = myTeam[i];
				if (!PopulatedDistanceInfo.ContainsKey(myRole))
					PopulatedDistanceInfo.Add(myRole, distanceCheckInfo.TeamInfo);
			}
		}

		/// <summary>
		/// Define any Roles that share the same spawns.
		/// </summary>
		public void DefineSharedSpawns(params RoleType[] rolesSharingSpawns)
		{
			var counter = rolesSharingSpawns.Length;
			for (int i = 0; i < counter; i++)
			{
				var role = rolesSharingSpawns[i];

				for (int j = 0; j < counter; j++)
				{
					var roleToAdd = rolesSharingSpawns[j];
					var roleGroup = PopulatedSharedSpawnInfo[role];

					if (!roleGroup.Contains(roleToAdd))
						roleGroup.Add(roleToAdd);
				}
			}
		}

		/// <summary>
		/// Gets the default spawn settings file. This is closest to base game.
		/// </summary>
		public static SpawnSettings GetDefaultSpawnSettings()
		{
			var spawnSettings = new SpawnSettings();
			spawnSettings.DefineSharedSpawns(RoleType.Scp93953, RoleType.Scp93989);
			spawnSettings.DefineSharedSpawns(RoleType.NtfScientist, RoleType.NtfCadet, RoleType.NtfCommander,
				RoleType.NtfLieutenant);

			var configs = CustomPlayerSpawning.Configs;

			if (configs.UseDefaultSafeSpawns)
			{
				spawnSettings.DefineSafeSpawnDistances(new DistanceCheckInfo(
					new[] { RoleType.ChaosInsurgency, RoleType.ClassD },
					configs.DefaultSafeSpawnDistance, configs.DefaultEnemySafeSpawnDistance));

				spawnSettings.DefineSafeSpawnDistances(new DistanceCheckInfo(
					new[]
					{
						RoleType.NtfCadet, RoleType.NtfCommander, RoleType.NtfLieutenant, RoleType.NtfScientist,
						RoleType.Scientist, RoleType.FacilityGuard
					},
					configs.DefaultSafeSpawnDistance, configs.DefaultEnemySafeSpawnDistance));

				spawnSettings.DefineSafeSpawnDistances(new DistanceCheckInfo(
					new[]
					{
						RoleType.Scp93953, RoleType.Scp049, RoleType.Scp096, RoleType.Scp106, RoleType.Scp93989,
						RoleType.Scp173
					},
					configs.DefaultSafeSpawnDistance, configs.DefaultEnemySafeSpawnDistance));
			}

			return spawnSettings;
		}
	}
}