using Plugins.vcow.ScreenLocker;
using UnityEngine;

namespace Lockers
{
	public class WaitScreenLocker : CommonScreenLockerBase
	{
		public override LockerType LockerType => LockerType.BusyWait;

		~WaitScreenLocker()
		{
			Debug.Log("WaitScreenLocker destroyed!");
		}
	}
}