using System.Collections.Generic;

namespace ArithFeather.CustomPlayerSpawning
{
	/// <summary>
	/// Create one of these for each team where you want the team to spawn a certain distance away from the enemy team or your own team.
	/// </summary>
	public class DistanceCheckInfo
	{
		public readonly RoleType[] MyTeam;
		public readonly Dictionary<RoleType, float> TeamInfo = new Dictionary<RoleType, float>();

		public DistanceCheckInfo(RoleType[] myTeam, bool onlyAvoidEnemy, float avoidDistance)
		{
			MyTeam = myTeam;

			if (onlyAvoidEnemy)
			{
				FillDefaultAvoidValues(0);
				DefineOtherTeam(avoidDistance);
			}
			else
			{
				FillDefaultAvoidValues(avoidDistance);
			}
		}

		public DistanceCheckInfo(RoleType[] myTeam, float avoidOtherDistance, float avoidEnemyDistance)
		{
			MyTeam = myTeam;
			FillDefaultAvoidValues(avoidOtherDistance);

			DefineOtherTeam(avoidEnemyDistance);
		}

		private void DefineOtherTeam(float avoidEnemyDistance)
		{
			for (int i = 0; i < CustomPlayerSpawning.RoleTypeSize; i++)
			{
				var role = (RoleType)i;

				if (!MyTeam.Contains(role))
				{
					TeamInfo[role] = avoidEnemyDistance;
				}
			}
		}

		private void FillDefaultAvoidValues(float avoidNonEnemyDistance)
		{
			for (int i = 0; i < CustomPlayerSpawning.RoleTypeSize; i++)
			{
				TeamInfo.Add((RoleType)i, avoidNonEnemyDistance);
			}
		}
	}
}