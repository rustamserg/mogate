using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Elizabeth
{
	public class Text : IBehavior
	{
		public Type Behavior { get { return typeof(Text); } }

		public SpriteFont Font;
		public string Message;

		public Text (SpriteFont font, string message = "")
		{
			Font = font;
			Message = message;
		}
	}
}

