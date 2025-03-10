using UnityEngine;

namespace Lockers
{
	public class SceneScreenLocker : CommonScreenLockerBase
	{
		public const string Key = "SceneLoader";

		public override string LockerKey => Key;

		~SceneScreenLocker()
		{
			Debug.Log("SceneScreenLocker destroyed!");
		}
	}
}