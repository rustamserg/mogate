using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace mogate
{
	public enum HeroState
	{
		Idle,
		Moving
	};

	public class HeroLayer : Layer
	{
		Sprite2D m_life;
		Point m_toMove;
		Entity m_player;
		bool m_isLevelCompleted = false;

		public HeroLayer(Game game, string name, int z) : base(game, name, z)
		{
		}

		public Entity Player { get { return m_player; } }

		public override void OnActivated ()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			RemoveAllEntities ();
			m_player = CreateEntity ();

			m_player.Register (new State<HeroState> (HeroState.Idle));
			m_player.Register (new Health (200));
			m_player.Register (new Attack (10));
			m_player.Register (new Attackable (OnAttacked));
			m_player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			m_player.Register (new Execute ());
			m_player.Register (new Drawable (sprites.GetSprite ("hero_idle"),
				new Vector2(mapGrid.StairDown.X*Globals.CELL_WIDTH, mapGrid.StairDown.Y*Globals.CELL_HEIGHT)));

			m_toMove = mapGrid.StairDown;
			StartIdle ();

			m_life = sprites.GetSprite ("items_life");
		}

		protected override void OnPostDraw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			for (int i = 0; i < (int)(m_player.Get<Health> ().HP / 20); i++) {
				var drawPos = new Vector2 (i * Globals.CELL_WIDTH, Globals.WORLD_HEIGHT * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_life.Texture, drawPos, m_life.GetFrameRect (0), Color.White);
			}
		}

		protected override void OnPostUpdate (GameTime gameTime)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			if (m_isLevelCompleted) {
				director.ActivateScene ("inter", TimeSpan.FromSeconds (1));
				return;
			}

			var gs = director.GetActiveScene ();
			var effects = (EffectsLayer)gs.GetLayer ("effects");
	
			var mapGrid = world.GetLevel(gameState.Level);

			var ms = Mouse.GetState ();
			var mapPos = m_player.Get<Position>().MapPos;

			if (ms.LeftButton == ButtonState.Pressed) {
				var clickPos = mapGrid.ScreenToWorld (ms.X, ms.Y);
				if (mapGrid.GetID (clickPos.X, clickPos.Y) != MapGridTypes.ID.Blocked) {
					m_toMove = clickPos;
					effects.SpawnEffect (m_toMove, "effects_marker", 200);
				}
			}

			if (ms.RightButton == ButtonState.Pressed) {
				var clickPos = mapGrid.ScreenToWorld (ms.X, ms.Y);
				if (mapGrid.GetID (clickPos.X, clickPos.Y) != MapGridTypes.ID.Blocked) {
					if (Utils.Dist (clickPos, mapPos) < 2) {
						DoAttack (clickPos);
					}
				}
			}

			if (m_player.Get<State<HeroState>>().EState == HeroState.Idle) {

				if (mapPos.X < m_toMove.X && mapGrid.GetID (mapPos.X + 1, mapPos.Y) != MapGridTypes.ID.Blocked)
					mapPos.X++;
				if (mapPos.X > m_toMove.X && mapGrid.GetID (mapPos.X - 1, mapPos.Y) != MapGridTypes.ID.Blocked)
					mapPos.X--;
				if (mapPos.Y < m_toMove.Y && mapGrid.GetID (mapPos.X, mapPos.Y + 1) != MapGridTypes.ID.Blocked)
					mapPos.Y++;
				if (mapPos.Y > m_toMove.Y && mapGrid.GetID (mapPos.X, mapPos.Y - 1) != MapGridTypes.ID.Blocked)
					mapPos.Y--;

				if (mapGrid.GetID (mapPos.X, mapPos.Y) != MapGridTypes.ID.Blocked && mapPos != m_player.Get<Position> ().MapPos) {
					var seq = new Sequence ();
					var spawn = new Spawn ();
					spawn.Add (new MoveSpriteTo (m_player, new Vector2 (mapPos.X * Globals.CELL_WIDTH, mapPos.Y * Globals.CELL_HEIGHT), 300));
					spawn.Add (new AnimSprite (m_player, sprites.GetSprite("hero_move"), 300));
					seq.Add (spawn);
					seq.Add (new ActionEntity (m_player, (_) => {
						m_player.Get<Position> ().MapPos = mapPos;
					}));
					seq.Add (new ActionEntity (m_player, OnEndMove));

					m_player.Get<Execute> ().AddNew (seq, "movement");
					m_player.Get<State<HeroState>> ().EState = HeroState.Moving;
				}
			}
		}

		private void DoAttack(Point attackTo)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var gs = director.GetActiveScene ();
			var effects = (EffectsLayer)gs.GetLayer ("effects");
			var monsters = (MonstersLayer)gs.GetLayer ("monsters");

			effects.SpawnEffect (attackTo, "items_sword", 100);

			var monster = monsters.GetAllEntities().FirstOrDefault (m => m.Get<Position> ().MapPos == attackTo);
			if (monster != default(Entity)) {
				m_player.Get<Execute> ().Add (new AttackEntity (m_player, monster));
			}
		}

		private void OnEndMove(Entity hero)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			var mt = mapGrid.GetID (m_player.Get<Position>().MapPos.X, m_player.Get<Position>().MapPos.Y);
			if (mt == MapGridTypes.ID.StairUp) {
				m_isLevelCompleted = true;
			}
			StartIdle ();
		}

		private void OnAttacked(Entity attacker)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var gs = director.GetActiveScene ();
			var effects = (EffectsLayer)gs.GetLayer ("effects");
	
			effects.AttachEffect (m_player, "effects_damage", 400);
		}

		private void StartIdle()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var loop = new Loop (new AnimSprite (m_player, sprites.GetSprite("hero_idle"), 600));

			m_player.Get<Execute> ().AddNew (loop, "movement");
			m_player.Get<State<HeroState>>().EState = HeroState.Idle;
		}
	}
}

