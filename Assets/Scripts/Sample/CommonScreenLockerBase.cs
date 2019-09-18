using System.Collections;
using Base.Activatable;
using Base.ScreenLocker;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sample
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class CommonScreenLockerBase : ScreenLocker<CommonScreenLockerBase>
	{
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

		public override void Activate(bool immediately = false)
		{
			Assert.IsFalse(this.IsActiveOrActivated());
			ActivatableState = immediately ? ActivatableState.Active : ActivatableState.ToActive;
			ValidateState();
		}

		public override void Deactivate(bool immediately = false)
		{
			Assert.IsFalse(this.IsInactiveOrDeactivated());
			ActivatableState = immediately ? ActivatableState.Inactive : ActivatableState.ToInactive;
			ValidateState();
		}

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
					ActivatableState = ActivatableState.Active;
					break;
				}

				if (newAlpha < 0)
				{
					Assert.IsTrue(ActivatableState == ActivatableState.ToInactive);
					_canvasGroup.alpha = 0;
					ActivatableState = ActivatableState.Inactive;
					break;
				}

				_canvasGroup.alpha = newAlpha;
				yield return null;
			}

			_fadeRoutine = null;
		}
	}
}