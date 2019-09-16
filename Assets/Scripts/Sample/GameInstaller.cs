using Base.ScreenLocker;
using Zenject;

namespace Sample
{
	public class GameInstaller : MonoInstaller<GameInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<IScreenLockerManager>().FromComponentInNewPrefabResource(@"ScreenLocker").AsSingle();
		}
	}
}