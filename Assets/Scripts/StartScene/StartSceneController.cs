using Plugins.vcow.ScreenLocker;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace StartScene
{
	[DisallowMultipleComponent]
	public class StartSceneController : MonoBehaviour
	{
		[Inject] private readonly IScreenLockerManager _screenLockerManager;
		[Inject] private readonly ZenjectSceneLoader _sceneLoader;

		private void Start()
		{
			if (_screenLockerManager.IsLocked)
			{
				_screenLockerManager.Unlock(Init);
			}
			else
			{
				Init();
			}
		}

		private void Init(LockerType lockerType = LockerType.Undefined)
		{
		}

		public void OnPlayGame()
		{
			Assert.IsFalse(_screenLockerManager.IsLocked);
			_screenLockerManager.Lock(LockerType.SceneLoader, () =>
				_sceneLoader.LoadSceneAsync("GameScene"));
		}
	}
}