using System;
using Microsoft.Xna.Framework.Input;

namespace Elizabeth
{
	public static class KeyboardUtils
	{
		private static bool m_isDown = false;
		private static Keys m_isDownKey;

		public static bool IsKeyPressed(Keys key)
		{
			if (!m_isDown && Keyboard.GetState ().IsKeyDown (key)) {
				m_isDownKey = key;
				m_isDown = true;
			}

			if (m_isDown && key == m_isDownKey && Keyboard.GetState ().IsKeyUp (key)) {
				m_isDown = false;
				return true;
			}
			return false;
		}
	}
}

