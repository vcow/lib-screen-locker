using Base.ScreenLocker;
using UnityEngine;
using Zenject;

namespace Sample
{
	public class SampleSceneBehaviour : MonoBehaviour
	{
#pragma warning disable 649
		[Inject] private readonly LazyInject<IScreenLockerManager> _screenLockerManager;
#pragma warning restore 649

		public void OnLoadGame()
		{
			_screenLockerManager.Value.Lock(LockerType.GameLoader, () => _screenLockerManager.Value.Unlock(null));
		}

		public void OnLoadScene()
		{
			_screenLockerManager.Value.Lock(LockerType.SceneLoader, () => _screenLockerManager.Value.Unlock(null));
		}

		public void OnWait()
		{
			_screenLockerManager.Value.Lock(LockerType.BusyWait, () => _screenLockerManager.Value.Unlock(null));
		}
	}
}