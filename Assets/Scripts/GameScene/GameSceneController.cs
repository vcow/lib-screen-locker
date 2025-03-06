using System.Collections;
using Plugins.vcow.ScreenLocker;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public class GameSceneController : MonoBehaviour
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

		public void OnGoBack()
		{
			Assert.IsFalse(_screenLockerManager.IsLocked);
			_screenLockerManager.Lock(LockerType.SceneLoader, () =>
				_sceneLoader.LoadSceneAsync("StartScene"));
		}

		public void OnWaitButton()
		{
			Assert.IsFalse(_screenLockerManager.IsLocked);
			_screenLockerManager.Lock(LockerType.BusyWait, () => StartCoroutine(Unlock(3)));
		}

		private IEnumerator Unlock(float delaySec)
		{
			Assert.IsTrue(delaySec > 0f);
			yield return new WaitForSeconds(delaySec);
			_screenLockerManager.Unlock(null);
		}
	}
}