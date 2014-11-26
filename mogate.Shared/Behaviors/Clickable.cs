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

		public void HandleMouseInput(MouseState state)
		{
			if (!ClickArea.Contains (state.Position))
				return;

			if (state.LeftButton == ButtonState.Pressed) {
				if (LeftButtonPressed != null)
					LeftButtonPressed (state.Position);
			} else if (state.LeftButton == ButtonState.Released) {
				if (LeftButtonReleased != null)
					LeftButtonReleased (state.Position);
			}

			if (state.RightButton == ButtonState.Pressed) {
				if (RightButtonPressed != null)
					RightButtonPressed (state.Position);
			} else if (state.RightButton == ButtonState.Released) {
				if (RightButtonReleased != null)
					RightButtonReleased (state.Position);
			}
		}
	}
}

