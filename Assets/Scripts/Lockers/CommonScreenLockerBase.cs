using DG.Tweening;
using Plugins.vcow.ScreenLocker;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lockers
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class CommonScreenLockerBase : ScreenLockerBase
	{
		private bool _isStarted;
		private CanvasGroup _canvasGroup;
		private Tween _tween;
		private float _alpha = 0;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.interactable = false;
		}

		protected virtual void Start()
		{
			_isStarted = true;
			_canvasGroup.alpha = _alpha;
			ValidateState();
		}

		protected override void OnDestroy()
		{
			_tween?.Kill();
			base.OnDestroy();
		}

		public override void Activate(object[] args = null, bool immediately = false)
		{
			Assert.IsFalse(State is ScreenLockerState.Active or ScreenLockerState.ToActive);
			State = immediately ? ScreenLockerState.Active : ScreenLockerState.ToActive;
			if (!ValidateState() && immediately)
			{
				_alpha = 1;
			}
		}

		public override void Deactivate(bool immediately = false)
		{
			Assert.IsFalse(State == ScreenLockerState.Inactive || State == ScreenLockerState.ToInactive);
			State = immediately ? ScreenLockerState.Inactive : ScreenLockerState.ToInactive;
			if (!ValidateState() && immediately)
			{
				_alpha = 0;
			}
		}

		public override void Force()
		{
			switch (State)
			{
				case ScreenLockerState.ToActive:
					State = ScreenLockerState.Active;
					ValidateState();
					break;
				case ScreenLockerState.ToInactive:
					State = ScreenLockerState.Inactive;
					ValidateState();
					break;
			}
		}

		private bool ValidateState()
		{
			if (!_isStarted) return false;

			_tween?.Kill();
			_tween = null;

			switch (State)
			{
				case ScreenLockerState.Active:
					_canvasGroup.alpha = 1;
					break;
				case ScreenLockerState.Inactive:
					_canvasGroup.alpha = 0;
					break;
				case ScreenLockerState.ToActive:
					_tween = _canvasGroup.DOFade(1, 1).OnComplete(() =>
					{
						_tween = null;
						_canvasGroup.interactable = true;
						State = ScreenLockerState.Active;
					});
					break;
				case ScreenLockerState.ToInactive:
					_canvasGroup.interactable = false;
					_tween = _canvasGroup.DOFade(0, 1).OnComplete(() =>
					{
						_tween = null;
						State = ScreenLockerState.Inactive;
					});
					break;
			}

			return true;
		}
	}
}