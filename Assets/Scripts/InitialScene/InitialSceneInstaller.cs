using UnityEngine;
using Zenject;

namespace InitialScene
{
	[DisallowMultipleComponent]
	public class InitialSceneInstaller : MonoInstaller<InitialSceneInstaller>
	{
		public override void InstallBindings()
		{
		}
	}
}