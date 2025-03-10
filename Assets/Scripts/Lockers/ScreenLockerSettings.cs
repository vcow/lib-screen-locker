using System.Collections.Generic;
using Plugins.vcow.ScreenLocker;
using UnityEngine;
using Zenject;

namespace Lockers
{
	[CreateAssetMenu(fileName = "ScreenLockerSettings", menuName = "Screen Locker Settings")]
	public class ScreenLockerSettings : ScriptableObjectInstaller<ScreenLockerSettings>, IScreenLockerSettings
	{
		[SerializeField] private ScreenLockerBase[] _screenLockers;

		public override void InstallBindings()
		{
			Container.Bind<IScreenLockerSettings>().FromInstance(this).AsSingle();
		}

		IReadOnlyList<ScreenLockerBase> IScreenLockerSettings.ScreenLockers => _screenLockers;
	}
}