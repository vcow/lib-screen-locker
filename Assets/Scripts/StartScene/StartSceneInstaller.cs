using InitialScene;
using UnityEngine;
using Zenject;

namespace StartScene
{
	[DisallowMultipleComponent]
	public class StartSceneInstaller : MonoInstaller<InitialSceneInstaller>
	{
		public override void InstallBindings()
		{
		}
	}
}