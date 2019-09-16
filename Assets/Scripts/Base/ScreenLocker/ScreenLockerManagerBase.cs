using System;
using System.Collections.Generic;
using System.Linq;
using Base.Activatable;
using Helpers.TouchHelper;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Base.ScreenLocker
{
	[Serializable]
	public class LockerRecord
	{
#pragma warning disable 649
		[SerializeField] private LockerType _type;
		[SerializeField] private GameObject _prefab;
#pragma warning restore 649
		public LockerType Type => _type;
		public GameObject Prefab => _prefab;
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
	public abstract class ScreenLockerManagerBase : MonoBehaviour, IScreenLockerManager
	{
		protected Dictionary<LockerType, GameObject> _screenPrefabs;
		protected abstract bool DontDestroyOnLoad { get; }

		private IScreenLocker _currentScreenLocker;
		private Action _completeCallback;

		private int _lockId;

#pragma warning disable 649
		[SerializeField] private LockerRecord[] _lockers = new LockerRecord[0];
#pragma warning restore 649

		protected virtual void Awake()
		{
			_screenPrefabs = _lockers.ToDictionary(record => record.Type, record => record.Prefab);

			if (DontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			InitManager(GetComponent<Canvas>(), GetComponent<CanvasScaler>(), GetComponent<GraphicRaycaster>());
		}

		protected abstract void InitManager(Canvas canvas, CanvasScaler canvasScaler,
			GraphicRaycaster graphicRaycaster);

		protected abstract void InitLocker(LockerType type, IScreenLocker locker);

		protected virtual void OnDestroy()
		{
			ReleaseHandlers(false);
		}

		private void OnActivateLockerHandler(ActivatableState state)
		{
			if (state != ActivatableState.Active) return;
			ReleaseHandlers(true);
		}

		private void OnDeactivateLockerHandler(ActivatableState state)
		{
			if (state != ActivatableState.Inactive) return;
			var mb = _currentScreenLocker as MonoBehaviour;
			if (mb) Destroy(mb.gameObject);
			IsLocked = false;
			ReleaseHandlers(true);
		}

		// 	IScreenLockerManager

		public bool IsLocked
		{
			get => _lockId > 0;
			private set
			{
				if (value == IsLocked) return;

				if (value)
				{
					Assert.IsTrue(_lockId == 0);
					_lockId = TouchHelper.Lock();
				}
				else
				{
					Assert.IsTrue(_lockId > 0);
					TouchHelper.Unlock(_lockId);
					_lockId = 0;
				}
			}
		}

		public void Lock(LockerType type, Action completeCallback)
		{
			ReleaseHandlers(true);

			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

			if (!_screenPrefabs.TryGetValue(type, out var prefab))
			{
				Debug.LogWarningFormat("There is no screen prefab for the {0} lock type.",
					typeof(LockerType).GetEnumName(type));
				IsLocked = false;
				completeCallback?.Invoke();
				return;
			}

			var locker = Instantiate(prefab, transform).GetComponent<IScreenLocker>();
			if (locker == null)
			{
				throw new Exception("Screen locker must implements IScreenLocker.");
			}

			InitLocker(type, locker);

			if (locker.IsActive())
			{
				Debug.LogWarningFormat("Screen locker for the {0} lock type is active in the initial time.",
					typeof(LockerType).GetEnumName(type));
				IsLocked = true;
				completeCallback?.Invoke();
				return;
			}

			_completeCallback = completeCallback;
			locker.ActivatableStateChangedEvent += OnActivateLockerHandler;

			locker.Activate(IsLocked);
			IsLocked = true;
		}

		public void Unlock(Action completeCallback)
		{
			ReleaseHandlers(true);

			IScreenLocker screenLocker = null;
			foreach (Transform child in transform)
			{
				var l = child.GetComponent<IScreenLocker>();
				if (l == null)
				{
					Destroy(child.gameObject);
				}
				else
				{
					if (screenLocker != null)
					{
						var mb = screenLocker as MonoBehaviour;
						if (mb) Destroy(mb.gameObject);
					}

					screenLocker = l;
				}
			}

			if (screenLocker == null || screenLocker.IsInactive())
			{
				IsLocked = false;
				completeCallback?.Invoke();
				return;
			}

			Assert.IsNull(_currentScreenLocker);
			_completeCallback = completeCallback;
			_currentScreenLocker = screenLocker;
			_currentScreenLocker.ActivatableStateChangedEvent += OnDeactivateLockerHandler;

			_currentScreenLocker.Deactivate();
		}

		// 	\IScreenLockerManager

		private void ReleaseHandlers(bool invokeCallback)
		{
			if (_currentScreenLocker != null)
			{
				_currentScreenLocker.ActivatableStateChangedEvent -= OnActivateLockerHandler;
				_currentScreenLocker.ActivatableStateChangedEvent -= OnDeactivateLockerHandler;
				_currentScreenLocker = null;
			}

			if (invokeCallback && _completeCallback != null)
			{
				// Предотвращение многократного рекурсивного вызова completeCallback.
				var completeCallback = _completeCallback;
				_completeCallback = null;

				completeCallback.Invoke();
			}
			else
			{
				_completeCallback = null;
			}
		}
	}
}