using System.Collections.Generic;

namespace Plugins.vcow.ScreenLocker
{
	public interface IScreenLockerSettings
	{
		IReadOnlyList<ScreenLockerBase> ScreenLockers { get; }
	}
}