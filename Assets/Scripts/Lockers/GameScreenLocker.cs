using Plugins.vcow.ScreenLocker;
using UnityEngine;

namespace Lockers
{
	public class GameScreenLocker : CommonScreenLockerBase
	{
		public override LockerType LockerType => LockerType.GameLoader;

		public override void Activate(bool immediately = false)
		{
			base.Activate(true);
		}

		~GameScreenLocker()
		{
			Debug.Log("GameScreenLocker destroyed!");
		}
	}
}