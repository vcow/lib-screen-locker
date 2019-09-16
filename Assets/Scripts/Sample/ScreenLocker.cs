using System;
using System.Collections;
using Base.Activatable;
using Base.ScreenLocker;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sample
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class ScreenLocker : MonoBehaviour, IScreenLocker
	{
		private ActivatableState _activatableState = ActivatableState.Inactive;
		private bool _isStarted;
		private CanvasGroup _canvasGroup;
		private Coroutine _fadeRoutine;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.alpha = 0;
		}

		private void Start()
		{
			_isStarted = true;
			ValidateState();
		}

		private void OnDestroy()
		{
			if (_fadeRoutine != null)
			{
				StopCoroutine(_fadeRoutine);
			}
		}

		public void Activate(bool immediately = false)
		{
			Assert.IsFalse(this.IsActiveOrActivated());
			ActivatableState = immediately ? ActivatableState.Active : ActivatableState.ToActive;
			ValidateState();
		}

		public void Deactivate(bool immediately = false)
		{
			Assert.IsFalse(this.IsInactiveOrDeactivated());
			ActivatableState = immediately ? ActivatableState.Inactive : ActivatableState.ToInactive;
			ValidateState();
		}

		public ActivatableState ActivatableState
		{
			get => _activatableState;
			private set
			{
				if (value == _activatableState) return;
				_activatableState = value;
				ActivatableStateChangedEvent?.Invoke(_activatableState);
			}
		}

		public event Action<ActivatableState> ActivatableStateChangedEvent;

		private void ValidateState()
		{
			if (!_isStarted) return;

			if (_fadeRoutine != null)
			{
				StopCoroutine(_fadeRoutine);
				_fadeRoutine = null;
			}

			switch (ActivatableState)
			{
				case ActivatableState.Active:
					_canvasGroup.alpha = 1;
					break;
				case ActivatableState.Inactive:
					_canvasGroup.alpha = 0;
					break;
				case ActivatableState.ToActive:
					_fadeRoutine = StartCoroutine(FadeRoutine(0.01f));
					break;
				case ActivatableState.ToInactive:
					_fadeRoutine = StartCoroutine(FadeRoutine(-0.01f));
					break;
			}
		}

		private IEnumerator FadeRoutine(float increment)
		{
			for (;;)
			{
				var newAlpha = _canvasGroup.alpha + increment;
				if (newAlpha > 1)
				{
					Assert.IsTrue(ActivatableState == ActivatableState.ToActive);
					_canvasGroup.alpha = 1;
					ActivatableStateChangedEvent?.Invoke(ActivatableState.Active);
					break;
				}

				if (newAlpha < 0)
				{
					Assert.IsTrue(ActivatableState == ActivatableState.ToInactive);
					_canvasGroup.alpha = 0;
					ActivatableStateChangedEvent?.Invoke(ActivatableState.Inactive);
					break;
				}

				_canvasGroup.alpha = newAlpha;
				yield return null;
			}

			_fadeRoutine = null;
		}
	}
}