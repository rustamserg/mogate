using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;


namespace mogate
{
	public interface IItems
	{
		void Init();
		IEnumerable<Point> GetItems();
	};

	public class Items : GameComponent, IItems
	{
		List<Point> m_itemsPos = new List<Point>();
		Random m_rand = new Random(DateTime.UtcNow.Millisecond);


		public Items(Game game) : base(game)
		{
		}

		public void Init()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			m_itemsPos.Clear ();
			foreach (var room in mapGrid.GetRooms()) {
				m_itemsPos.Add(new Point(room.Pos.X + m_rand.Next(room.Width), room.Pos.Y + m_rand.Next (room.Height)));
			}
		}

		public IEnumerable<Point> GetItems ()
		{
			return new List<Point> (m_itemsPos);
		}
	}
}

