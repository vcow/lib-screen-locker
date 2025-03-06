using System.Collections.Generic;

namespace Plugins.vcow.ScreenLocker
{
	public interface IScreenLockerSettings
	{
		IReadOnlyList<BaseScreenLocker> ScreenLockers { get; }
	}
}