using System;
using Microsoft.Xna.Framework;


namespace mogate
{
	public class HeroEntity : Entity
	{
		public HeroEntity ()
		{
			Register (new Health (2000));
		}
	};

	public interface IHero
	{
		void Init();
		HeroEntity Player { get; }
	};

	public class Hero : GameComponent, IHero
	{
		HeroEntity m_player;

		public Hero (Game game) : base(game)
		{
			m_player = new HeroEntity ();
		}

		public void Init ()
		{
			var mapGrid = (IMapGrid)Game.Services.GetService(typeof(IMapGrid));
			Player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
		}

		public HeroEntity Player { get { return m_player; } }
	}
}

