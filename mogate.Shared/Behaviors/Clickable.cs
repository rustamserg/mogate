using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

		public Clickable (Rectangle clickArea)
		{
			ClickArea = clickArea;
		}

		public void HandleMouseInput(MouseState state, Vector2 screenToWordScale)
		{
			var worldPos = Vector2.Multiply (new Vector2 (state.Position.X, state.Position.Y), screenToWordScale);

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
	}
}

