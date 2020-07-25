using Exiled.API.Features;

namespace ArithFeather.RandomPlayerSpawning {
	public class RandomPlayerSpawning : Plugin<Config> {

		public RandomPlayerSpawning() {
			Data.OnRandomPlayerSpawning += (a) => OnRandomPlayerSpawning?.Invoke(a);
		}

		public Spawner Data = new Spawner();

		public override string Author => "Arith";

		public override System.Version Version => new System.Version("1.0");

		public static event Spawner.PlayerSpawning OnRandomPlayerSpawning;

		public override void OnEnabled() {
			base.OnEnabled();
			Data.Enable();
		}

		public override void OnDisabled() {
			base.OnDisabled();
			Data.Disable();
		}
	}
}
