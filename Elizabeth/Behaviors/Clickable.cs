using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;

namespace Elizabeth
{
	//TODO: remove pressed/released and make only click and tap events
	public class Clickable : IBehavior
	{
		public Type Behavior { get { return typeof(Clickable); } }

		public Rectangle ClickArea;

		public Action<Point, Entity> OnLeftButtonPressed;
		public Action<Point, Entity> OnRightButtonPressed;

		public Action<Point, Entity> OnTouched;
		public Action<Point, Entity> OnMoved;

		private int m_lastTouchID;
		private Vector2 m_lastTouchPos;
		private bool m_isTouchMoved;
		private Entity m_entity;

		public Clickable (Rectangle clickArea, Entity entity = null)
		{
			ClickArea = clickArea;
			m_entity = entity;
		}

		public void HandleMouseInput(MouseState state, Vector2 screenToWorldScale)
		{
			var worldPos = Vector2.Multiply (new Vector2 (state.Position.X, state.Position.Y), screenToWorldScale);

			if (!ClickArea.Contains (worldPos))
				return;

			var clickPos = new Point ((int)worldPos.X, (int)worldPos.Y);

			if (state.LeftButton == ButtonState.Pressed) {
				if (OnLeftButtonPressed != null)
					OnLeftButtonPressed (clickPos, m_entity);
			}
			if (state.RightButton == ButtonState.Pressed) {
				if (OnRightButtonPressed != null)
					OnRightButtonPressed (clickPos, m_entity);
			}
		}

		public void HandleTouchInput(TouchCollection touches)
		{
			foreach (var touch in touches) {
				var touchPos = new Point ((int)touch.Position.X, (int)touch.Position.Y);

				switch (touch.State) {
				case TouchLocationState.Pressed:
					m_lastTouchID = touch.Id;
					m_lastTouchPos = touch.Position;
					m_isTouchMoved = false;
					break;
				case TouchLocationState.Released:
					if (touch.Id == m_lastTouchID) {
						if (!m_isTouchMoved && ClickArea.Contains (touchPos)) {
							if (OnTouched != null) {
								OnTouched (touchPos, m_entity);
							}
						}
						m_isTouchMoved = false;
						m_lastTouchID = 0;
						m_lastTouchPos = Vector2.Zero;
					}
					break;
				case TouchLocationState.Moved:
					if (touch.Id == m_lastTouchID && touch.Position != m_lastTouchPos) {
						m_lastTouchPos = touch.Position;
						m_isTouchMoved = true;
						if (ClickArea.Contains (touchPos)) {
							if (OnMoved != null) {
								OnMoved (touchPos, m_entity);
							}
						}
					}
					break;
				}
			}
		}
	}
}

