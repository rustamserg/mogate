using System;
using Microsoft.Xna.Framework;


namespace mogate
{
	public interface IHero
	{
		void Init();
		Point Position { get; set; }
	};

	public class Hero : GameComponent, IHero
	{
		public Hero (Game game) : base(game)
		{
		}

		public void Init ()
		{
			var mapGrid = (IMapGrid)Game.Services.GetService(typeof(IMapGrid));
			Position = mapGrid.StairDown;
		}

		public Point Position { get; set; }
	}
}

