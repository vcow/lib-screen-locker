using Lockers;
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
				_screenLockerManager.Unlock(completeCallback: Init);
			}
			else
			{
				Init();
			}
		}

		private void Init(string key = null)
		{
		}

		public void OnPlayGame()
		{
			Assert.IsFalse(_screenLockerManager.IsLocked);
			_screenLockerManager.Lock(SceneScreenLocker.Key, () =>
				_sceneLoader.LoadSceneAsync("GameScene"));
		}
	}
}