using UnityEngine;

namespace Lockers
{
	public class WaitScreenLocker : CommonScreenLockerBase
	{
		public const string Key = "BusyWait";

		public override string LockerKey => Key;

		~WaitScreenLocker()
		{
			Debug.Log("WaitScreenLocker destroyed!");
		}
	}
}