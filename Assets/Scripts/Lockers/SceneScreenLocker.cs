using Plugins.vcow.ScreenLocker;
using UnityEngine;

namespace Lockers
{
	public class SceneScreenLocker : CommonScreenLockerBase
	{
		public override LockerType LockerType => LockerType.SceneLoader;

		~SceneScreenLocker()
		{
			Debug.Log("SceneScreenLocker destroyed!");
		}
	}
}