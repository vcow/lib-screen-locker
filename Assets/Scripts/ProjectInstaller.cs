using Plugins.vcow.ScreenLocker;
using Plugins.vcow.TouchHelper;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

[DisallowMultipleComponent]
public class ProjectInstaller : MonoInstaller<ProjectInstaller>
{
	private int? _lock;

	public override void InstallBindings()
	{
		Container.BindInterfacesTo<ScreenLockerManager>().FromMethod(ScreenLockerFabric).AsSingle();
	}

	private ScreenLockerManager ScreenLockerFabric(InjectContext injectContext)
	{
		var screenLockerManager = new ScreenLockerManager(Container.Resolve<IScreenLockerSettings>(), InstantiateScreenLockerHook);
		screenLockerManager.IsLockedChangedEvent += OnScreenLockedChangedEvent;
		return screenLockerManager;
	}

	void OnScreenLockedChangedEvent(IScreenLockerManager screenLockerManager, bool isLocked)
	{
		if (isLocked)
		{
			Assert.IsFalse(_lock.HasValue);
			_lock = TouchHelper.Lock();
		}
		else if (_lock.HasValue)
		{
			TouchHelper.Unlock(_lock.Value);
			_lock = null;
		}
	}

	private void InstantiateScreenLockerHook(ScreenLockerBase locker)
	{
		Container.Inject(locker);
	}
}