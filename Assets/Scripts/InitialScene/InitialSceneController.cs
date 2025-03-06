using System.Collections;
using Plugins.vcow.ScreenLocker;
using UnityEngine;
using Zenject;

namespace InitialScene
{
	[DisallowMultipleComponent]
	public class InitialSceneController : MonoBehaviour
	{
		[Inject] private readonly IScreenLockerManager _screenLockerManager;
		[Inject] private readonly ZenjectSceneLoader _sceneLoader;

		private void Start()
		{
			_screenLockerManager.Lock(LockerType.GameLoader, () => StartCoroutine(Init()));
		}

		private IEnumerator Init()
		{
			yield return new WaitForSeconds(3);
			yield return _sceneLoader.LoadSceneAsync("StartScene");
		}
	}
}