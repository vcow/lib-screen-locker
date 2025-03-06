using InitialScene;
using UnityEngine;
using Zenject;

namespace GameScene
{
	[DisallowMultipleComponent]
	public class GameSceneInstaller : MonoInstaller<InitialSceneInstaller>
	{
		public override void InstallBindings()
		{
		}
	}
}