using Base.ScreenLocker;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
	public class ScreenLockerManager : ScreenLockerManagerBase
	{
		protected override bool DontDestroyOnLoad => false;

		protected override void InitManager(Canvas canvas, CanvasScaler canvasScaler, GraphicRaycaster graphicRaycaster)
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 10000;

			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(2048, 1536);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			canvasScaler.matchWidthOrHeight = 1;

			graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.All;
		}

		protected override void InitLocker(LockerType type, IScreenLocker locker)
		{
		}
	}
}