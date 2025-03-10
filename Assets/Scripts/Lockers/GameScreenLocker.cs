using UnityEngine;

namespace Lockers
{
	public class GameScreenLocker : CommonScreenLockerBase
	{
		public const string Key = "GameLoader";

		public override string LockerKey => Key;

		public override void Activate(object[] args = null, bool immediately = false)
		{
			base.Activate(args, true);
		}

		~GameScreenLocker()
		{
			Debug.Log("GameScreenLocker destroyed!");
		}
	}
}