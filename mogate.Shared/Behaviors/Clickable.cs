using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace mogate
{
	public class Clickable : IBehavior
	{
		public Type Behavior { get { return typeof(Clickable); } }

		public Rectangle ClickArea;

		public Action<Point> LeftButtonPressed;
		public Action<Point> LeftButtonReleased;

		public Action<Point> RightButtonPressed;
		public Action<Point> RightButtonReleased;

		public Action<Point> TouchPressed;
		public Action<Point> TouchReleased;


		public Clickable (Rectangle clickArea)
		{
			ClickArea = clickArea;
		}

		public void HandleMouseInput(MouseState state, Vector2 screenToWorldScale)
		{
			var worldPos = Vector2.Multiply (new Vector2 (state.Position.X, state.Position.Y), screenToWorldScale);

			if (!ClickArea.Contains (worldPos))
				return;

			var clickPos = new Point ((int)worldPos.X, (int)worldPos.Y);

			if (state.LeftButton == ButtonState.Pressed) {
				if (LeftButtonPressed != null)
					LeftButtonPressed (clickPos);
			} else if (state.LeftButton == ButtonState.Released) {
				if (LeftButtonReleased != null)
					LeftButtonReleased (clickPos);
			}

			if (state.RightButton == ButtonState.Pressed) {
				if (RightButtonPressed != null)
					RightButtonPressed (clickPos);
			} else if (state.RightButton == ButtonState.Released) {
				if (RightButtonReleased != null)
					RightButtonReleased (clickPos);
			}
		}

		public void HandleTouchInput(TouchCollection state, Vector2 screenToWorldScale)
		{
			foreach (var touch in state) {
				var worldPos = Vector2.Multiply (new Vector2 (touch.Position.X, touch.Position.Y), screenToWorldScale);

				if (!ClickArea.Contains (worldPos))
					continue;

				var touchPos = new Point ((int)worldPos.X, (int)worldPos.Y);
				if (touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved) {
					if (TouchPressed != null)
						TouchPressed (touchPos);
				} else if (touch.State == TouchLocationState.Released) {
					if (TouchReleased != null)
						TouchReleased (touchPos);
				}
			}
		}
	}
}

