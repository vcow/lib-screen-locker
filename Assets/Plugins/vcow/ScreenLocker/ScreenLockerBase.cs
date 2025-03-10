using UnityEngine;

namespace Plugins.vcow.ScreenLocker
{
	/// <summary>
	/// Activatqble states.
	/// </summary>
	public enum ScreenLockerState
	{
		Inactive,
		Active,
		ToActive,
		ToInactive
	}

	/// <summary>
	/// Delegate for state change event.
	/// </summary>
	public delegate void ScreenLockerStateChangedHandler(ScreenLockerBase locker, ScreenLockerState state);

	/// <summary>
	/// The base class of the Screen Locker component, which should be added to the screen locker window prefab.
	/// </summary>
	public abstract class ScreenLockerBase : MonoBehaviour
	{
		private ScreenLockerState _state = ScreenLockerState.Inactive;
		public event ScreenLockerStateChangedHandler StateChangedEvent;

		/// <summary>
		/// Start activate locker transition.
		/// </summary>
		/// <param name="args">Additional arguments for the locker.</param>
		/// <param name="immediately">Activate locker immediately.</param>
		public abstract void Activate(object[] args = null, bool immediately = false);

		/// <summary>
		/// Start deactivate locker transition.
		/// </summary>
		/// <param name="immediately">Deactivate locker immediately.</param>
		public abstract void Deactivate(bool immediately = false);

		/// <summary>
		/// The type of the locker screen.
		/// </summary>
		public abstract string LockerKey { get; }

		/// <summary>
		/// This method immediately finished any transition process (if locker is in the ToActive or ToInactive state).
		/// </summary>
		public abstract void Force();

		public ScreenLockerState State
		{
			get => _state;
			protected set
			{
				if (value == _state) return;
				_state = value;
				StateChangedEvent?.Invoke(this, _state);
			}
		}

		protected virtual void OnDestroy()
		{
			StateChangedEvent = null;
		}
	}
}