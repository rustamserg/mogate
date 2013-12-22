using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace mogate
{
	public enum HeroState
	{
		Idle,
		Moving
	};

	public interface IHero
	{
		Entity Player { get; }
	};

	public class Hero : GameComponent, IHero
	{
		Entity m_player;
		Point m_toMove;

		public Hero (Game game) : base(game)
		{
			m_player = new Entity ();
		}

		public override void Update(GameTime gameTime)
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
	
			if (gameState.State == EState.LevelCreated) {
				Init();
				gameState.State = EState.HeroCreated;
			}

			if (gameState.State == EState.GameStarted) {
				UpdateHero ();
				Player.Get<Execute> ().Update (gameTime);
			}

			base.Update (gameTime);
		}

		public Entity Player { get { return m_player; } }

		private void Init ()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			Player.Register (new State<HeroState> (HeroState.Idle));
			Player.Register (new Health (200));
			Player.Register (new Attack (10));
			Player.Register (new Attackable (OnAttacked));
			Player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			Player.Register (new Execute ());
			Player.Register (new Drawable (sprites.GetSprite ("hero_idle"),
				new Vector2(mapGrid.StairDown.X*Globals.CELL_WIDTH, mapGrid.StairDown.Y*Globals.CELL_HEIGHT)));

			m_toMove = mapGrid.StairDown;
			StartIdle (Player);
		}

		private void UpdateHero()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var effects = (IEffects)Game.Services.GetService (typeof(IEffects));

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

			if (Player.Get<State<HeroState>>().EState == HeroState.Idle) {

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
					spawn.Add (new MoveSpriteTo (Player, new Vector2 (mapPos.X * Globals.CELL_WIDTH, mapPos.Y * Globals.CELL_HEIGHT), 300));
					spawn.Add (new AnimSprite (Player, sprites.GetSprite("hero_move"), 300));
					seq.Add (spawn);
					seq.Add (new ActionEntity (Player, (_) => {
						Player.Get<Position> ().MapPos = mapPos;
					}));
					seq.Add (new ActionEntity (Player, OnEndMove));

					Player.Get<Execute> ().AddNew (seq, "movement");
					Player.Get<State<HeroState>> ().EState = HeroState.Moving;
				}
			}
		}

		private void DoAttack(Point attackTo)
		{
			var effects = (IEffects)Game.Services.GetService (typeof(IEffects));
			effects.SpawnEffect (attackTo, "items_sword", 100);

			var monsters = (IMonsters)Game.Services.GetService (typeof(IMonsters));

			var monster = monsters.GetMonsters ().FirstOrDefault (m => m.Get<Position> ().MapPos == attackTo);
			if (monster != default(Entity)) {
				Player.Get<Execute> ().Add (new AttackEntity (Player, monster));
			}
		}
		
		private void OnEndMove(Entity hero)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			var mt = mapGrid.GetID (Player.Get<Position>().MapPos.X, Player.Get<Position>().MapPos.Y);
			if (mt == MapGridTypes.ID.StairUp) {
				gameState.NextLevel ();
			}
			StartIdle (hero);
		}

		private void OnAttacked(Entity attacker)
		{
			var effects = (IEffects)Game.Services.GetService (typeof(IEffects));
			effects.AttachEffect (Player, "effects_damage", 400);
		}

		private void StartIdle(Entity hero)
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var loop = new Loop (new AnimSprite (Player, sprites.GetSprite("hero_idle"), 600));

			Player.Get<Execute> ().AddNew (loop, "movement");
			Player.Get<State<HeroState>>().EState = HeroState.Idle;
		}
	}
}

