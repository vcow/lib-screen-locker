using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.vcow.ScreenLocker
{
	public sealed class ScreenLockerManager : IScreenLockerManager, IDisposable
	{
		public delegate void InstantiateScreenLockerHook(ScreenLockerBase locker);

		private readonly Dictionary<string, ScreenLockerBase> _screenLockerPrefabs;
		private readonly Dictionary<string, ScreenLockerBase> _activeLockers = new();
		private readonly Dictionary<ScreenLockerBase, Action> _lockCompleteCallbacks = new();
		private readonly Dictionary<ScreenLockerBase, Action<string>> _unlockCompleteCallbacks = new();

		private bool? _isLocked;
		private InstantiateScreenLockerHook _instantiateScreenLockerHook;

		public ScreenLockerManager(IScreenLockerSettings settings, InstantiateScreenLockerHook instantiateScreenLockerHook = null)
		{
			_instantiateScreenLockerHook = instantiateScreenLockerHook;

			_screenLockerPrefabs = settings.ScreenLockers != null
				? settings.ScreenLockers.GroupBy(record => record.LockerKey)
					.Select(lockers =>
					{
						var locker = lockers.First();
#if DEBUG || UNITY_EDITOR
						var numLockers = lockers.Count();
						if (numLockers > 1)
						{
							Debug.LogErrorFormat("There are {0} lockers, specified for the {1} type.",
								numLockers, locker.LockerKey);
						}
#endif
						return locker;
					})
					.ToDictionary(locker => locker.LockerKey)
				: new Dictionary<string, ScreenLockerBase>();
		}

		void IDisposable.Dispose()
		{
			foreach (var activeLocker in _activeLockers.Values)
			{
				if (!activeLocker) continue;

				activeLocker.Force();

				activeLocker.StateChangedEvent -= OnLockerStateChanged;
				Object.Destroy(activeLocker.gameObject);
			}

			_activeLockers.Clear();
			_lockCompleteCallbacks.Clear();
			_unlockCompleteCallbacks.Clear();
		}

		private void OnLockerStateChanged(ScreenLockerBase locker, ScreenLockerState state)
		{
			switch (state)
			{
				case ScreenLockerState.Active: // locked
					locker.StateChangedEvent -= OnLockerStateChanged;

					if (_lockCompleteCallbacks.Remove(locker, out var lockCallback))
					{
						lockCallback.Invoke();
					}

					break;
				case ScreenLockerState.Inactive: // unlocked
					locker.StateChangedEvent -= OnLockerStateChanged;

					if (_activeLockers.TryGetValue(locker.LockerKey, out var activeLocker) &&
					    activeLocker == locker)
					{
						_activeLockers.Remove(locker.LockerKey);
					}

					if (_unlockCompleteCallbacks.Remove(locker, out var unlockCallback))
					{
						unlockCallback.Invoke(locker.LockerKey);
					}

					Object.Destroy(locker.gameObject);

					IsLocked = _activeLockers.Count > 0;
					break;
			}
		}

		public void SetScreenLocker(ScreenLockerBase screenLockerBasePrefab)
		{
			_screenLockerPrefabs[screenLockerBasePrefab.LockerKey] = screenLockerBasePrefab;
		}

		// 	IScreenLockerManager

		public bool IsLocked
		{
			get => _isLocked ?? false;
			private set
			{
				if (value == _isLocked) return;

				_isLocked = value;
				IsLockedChangedEvent?.Invoke(this, value);
			}
		}

		public event IsLockedChangedHandler IsLockedChangedEvent;

		public void Lock(string key, Action completeCallback = null, object[] args = null)
		{
			if (_activeLockers.TryGetValue(key, out var oldLocker))
			{
				oldLocker.Force();

				oldLocker.StateChangedEvent -= OnLockerStateChanged;
				Object.Destroy(oldLocker.gameObject);

				if (_activeLockers.Remove(key))
				{
					// This locker should have be removed from the active lockers in the OnLockerStateChanged handler,
					// if not then he isn't send the ActivatableStateChangedEvent during the Force() call.
					Debug.LogWarningFormat("The locker of type {0} hasn't change his activatable state during the " +
					                       "Force() action.", oldLocker.LockerKey);
				}

				if (_lockCompleteCallbacks.Remove(oldLocker, out var oldLockCallback))
				{
					oldLockCallback.Invoke();
				}

				if (_unlockCompleteCallbacks.Remove(oldLocker, out var oldUnlockCallback))
				{
					oldUnlockCallback.Invoke(oldLocker.LockerKey);
				}
			}

			if (!_screenLockerPrefabs.TryGetValue(key, out var prefab))
			{
				Debug.LogErrorFormat("There is no screen prefab for the {0} lock type.", key);
				IsLocked = _activeLockers.Count > 0;
				completeCallback?.Invoke();
				return;
			}

			var locker = Object.Instantiate(prefab);
			_instantiateScreenLockerHook?.Invoke(locker);
			Object.DontDestroyOnLoad(locker.gameObject);

			_activeLockers.Add(key, locker);

			IsLocked = true;

			if (locker.State == ScreenLockerState.Inactive)
			{
				if (completeCallback != null)
				{
					_lockCompleteCallbacks.Add(locker, completeCallback);
				}

				locker.StateChangedEvent += OnLockerStateChanged;
				locker.Activate(args: args);
			}
			else if (locker.State == ScreenLockerState.Active)
			{
				completeCallback?.Invoke();
			}
			else
			{
				Debug.LogErrorFormat("The locker {0} is in wrong initial state {1}.", locker.LockerKey, locker.State);
				completeCallback?.Invoke();
			}
		}

		public void Unlock(string key = null, Action<string> completeCallback = null)
		{
			var unlocked = new List<ScreenLockerBase>();
			if (!string.IsNullOrEmpty(key))
			{
				if (_activeLockers.TryGetValue(key, out var locker))
				{
					unlocked.Add(locker);
				}
			}
			else
			{
				unlocked.AddRange(_activeLockers.Values);
			}

			if (unlocked.Count <= 0)
			{
				completeCallback?.Invoke(string.Empty);
				return;
			}

			foreach (var locker in unlocked)
			{
				if (locker.State != ScreenLockerState.Active)
				{
					locker.Force();

					if (_lockCompleteCallbacks.TryGetValue(locker, out var lockCallback))
					{
						// This callback should be called and removed from the lock complete callbacks in the
						// OnLockerStateChanged handler, if not then locker isn't send ActivatableStateChanged event
						// during the Force() call.
						Debug.LogWarningFormat("The locker of type {0} hasn't change his activatable state during " +
						                       "the Force() action.", locker.LockerKey);
						_lockCompleteCallbacks.Remove(locker);
						lockCallback.Invoke();
					}
				}

				if (locker.State == ScreenLockerState.Active)
				{
					if (completeCallback != null)
					{
						_unlockCompleteCallbacks.Add(locker, completeCallback);
					}

					locker.StateChangedEvent += OnLockerStateChanged;
					locker.Deactivate();
				}
				else
				{
					Debug.LogErrorFormat("The locker {0} that is been unlocked wasn't switched to the Active state.",
						locker.LockerKey);

					completeCallback?.Invoke(locker.LockerKey);

					locker.StateChangedEvent -= OnLockerStateChanged;
					Object.Destroy(locker.gameObject);

					_activeLockers.Remove(locker.LockerKey);
					_lockCompleteCallbacks.Remove(locker);
					_unlockCompleteCallbacks.Remove(locker);
				}
			}

			IsLocked = _activeLockers.Count > 0;
		}

		// 	\IScreenLockerManager
	}
}