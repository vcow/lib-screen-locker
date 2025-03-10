using System;

namespace Plugins.vcow.ScreenLocker
{
	public delegate void IsLockedChangedHandler(IScreenLockerManager screenLockerManager, bool isLocked);

	/// <summary>
	/// The Screen Locker Manager interface.
	/// </summary>
	public interface IScreenLockerManager
	{
		/// <summary>
		/// The flag indicates that some kind of blocking is enabled.
		/// </summary>
		bool IsLocked { get; }

		/// <summary>
		/// Lock state change event.
		/// </summary>
		event IsLockedChangedHandler IsLockedChangedEvent;

		/// <summary>
		/// Enable a blocking of the specified type.
		/// </summary>
		/// <param name="key">The type of blocking.</param>
		/// <param name="completeCallback">A callback, which call when blocking is finished.</param>
		/// <param name="args">Additional args to send to the locker.</param>
		void Lock(string key, Action completeCallback = null, object[] args = null);

		/// <summary>
		/// Disable a current blocking.
		/// </summary>
		/// <param name="key">A type of the blocking to disable, if null all blocks are disabled.</param>
		/// <param name="completeCallback">A callback, which call when unblocking is finished.</param>
		void Unlock(string key = null, Action<string> completeCallback = null);

		/// <summary>
		/// Set a screen locker for the specified type.
		/// </summary>
		/// <param name="screenLockerBasePrefab">A prefab of the screen locker window.</param>
		void SetScreenLocker(ScreenLockerBase screenLockerBasePrefab);
	}
}