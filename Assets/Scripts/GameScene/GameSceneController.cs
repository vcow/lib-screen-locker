using System.Collections;
using System.Collections.Generic;
using Lockers;
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
				_screenLockerManager.Unlock(completeCallback: Init);
			}
			else
			{
				Init();
			}
		}

		private void Init(IReadOnlyList<string> keys = null)
		{
		}

		public void OnGoBack()
		{
			Assert.IsFalse(_screenLockerManager.IsLocked);
			_screenLockerManager.Lock(SceneScreenLocker.Key, () => _sceneLoader.LoadSceneAsync("StartScene"));
		}

		public void OnWaitButton()
		{
			Assert.IsFalse(_screenLockerManager.IsLocked);
			_screenLockerManager.Lock("BusyWait", () => StartCoroutine(Unlock(3)));
		}

		private IEnumerator Unlock(float delaySec)
		{
			Assert.IsTrue(delaySec > 0f);
			yield return new WaitForSeconds(delaySec);
			_screenLockerManager.Unlock();
		}
	}
}