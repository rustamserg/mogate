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
		Sprite2D m_armor;
		Point m_toMove;
		bool m_isLevelCompleted = false;

		public HeroLayer(Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated ()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			var player = CreateEntity ("player");

			player.Register (new State<HeroState> (HeroState.Idle));
			player.Register (new Health (gameState.PlayerHealth, gameState.MaxPlayerHealth,
											() => OnHealthChanged(player)));
			player.Register (new Attack (1));
			player.Register (new Armor (gameState.PlayerArmor, gameState.MaxPlayerArmor));
			player.Register (new PointLight (6));
			player.Register (new MoveSpeed (300));
			player.Register (new Attackable (OnAttacked));
			player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			player.Register (new Execute ());
			player.Register (new Drawable (sprites.GetSprite ("hero_idle"),
				new Vector2(mapGrid.StairDown.X * Globals.CELL_WIDTH, mapGrid.StairDown.Y * Globals.CELL_HEIGHT)));

			m_toMove = mapGrid.StairDown;
			StartIdle ();

			m_life = sprites.GetSprite ("items_life");
			m_armor = sprites.GetSprite ("items_shield");
		}

		public override void OnDeactivated ()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var player = GetEntityByTag("player");
			gameState.PlayerHealth = player.Get<Health> ().HP;
		}

		protected override void OnPostDraw (SpriteBatch spriteBatch, GameTime gameTime)
		{
			var player = GetEntityByTag("player");

			for (int i = 0; i < player.Get<Health> ().HP; i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_life.Texture, drawPos, m_life.GetFrameRect (0), Color.White);
			}
			for (int i = 0; i < player.Get<Armor> ().Value; i++) {
				var drawPos = new Vector2 (Globals.WORLD_WIDTH * Globals.CELL_WIDTH, 400 + i  * Globals.CELL_HEIGHT);
				spriteBatch.Draw (m_armor.Texture, drawPos, m_armor.GetFrameRect (0), Color.White);
			}
		}

		protected override void OnPostUpdate (GameTime gameTime)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			if (Keyboard.GetState ().IsKeyDown (Keys.Q))
				m_isLevelCompleted = true;

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			if (m_isLevelCompleted) {
				director.ActivateScene ("inter", TimeSpan.FromSeconds (1));
				return;
			}
				
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
	
			var mapGrid = world.GetLevel(gameState.Level);
			var player = GetEntityByTag("player");

			var ms = Mouse.GetState ();
			var mapPos = player.Get<Position>().MapPos;

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

			if (player.Get<State<HeroState>>().EState == HeroState.Idle) {

				if (mapPos.X < m_toMove.X && mapGrid.GetID (mapPos.X + 1, mapPos.Y) != MapGridTypes.ID.Blocked)
					mapPos.X++;
				if (mapPos.X > m_toMove.X && mapGrid.GetID (mapPos.X - 1, mapPos.Y) != MapGridTypes.ID.Blocked)
					mapPos.X--;
				if (mapPos.Y < m_toMove.Y && mapGrid.GetID (mapPos.X, mapPos.Y + 1) != MapGridTypes.ID.Blocked)
					mapPos.Y++;
				if (mapPos.Y > m_toMove.Y && mapGrid.GetID (mapPos.X, mapPos.Y - 1) != MapGridTypes.ID.Blocked)
					mapPos.Y--;

				if (mapGrid.GetID (mapPos.X, mapPos.Y) != MapGridTypes.ID.Blocked && mapPos != player.Get<Position> ().MapPos) {
					var seq = new Sequence ();
					var spawn = new Spawn ();
					spawn.Add (new MoveSpriteTo (player, new Vector2 (mapPos.X * Globals.CELL_WIDTH, mapPos.Y * Globals.CELL_HEIGHT), 300));
					spawn.Add (new AnimSprite (player, sprites.GetSprite("hero_move"), player.Get<MoveSpeed>().Speed));
					seq.Add (spawn);
					seq.Add (new ActionEntity (player, (_) => {
						player.Get<Position> ().MapPos = mapPos;
					}));
					seq.Add (new ActionEntity (player, OnEndMove));

					player.Get<Execute> ().AddNew (seq, "movement");
					player.Get<State<HeroState>> ().EState = HeroState.Moving;
				}
			}
		}

		private void DoAttack(Point attackTo)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			var monsters = (MonstersLayer)Scene.GetLayer ("monsters");
			var items = (ItemsLayer)Scene.GetLayer ("items");
			var player = GetEntityByTag("player");

			effects.SpawnEffect (attackTo, "items_sword", 100);

			var monster = monsters.GetAllEntities().FirstOrDefault (m => m.Get<Position> ().MapPos == attackTo);
			if (monster != default(Entity)) {
				player.Get<Execute> ().Add (new AttackEntity (player, monster));
			}
			var item = items.GetAllEntities ().FirstOrDefault (m => m.Get<Position> ().MapPos == attackTo);
			if (item != default(Entity)) {
				player.Get<Execute> ().Add (new AttackEntity (player, item));
			}
		}

		private void OnEndMove(Entity hero)
		{
			var player = GetEntityByTag ("player");
			var items = (ItemsLayer)Scene.GetLayer ("items");
			var maps = (MapGridLayer)Scene.GetLayer ("map");

			var toTrigger = items.GetAllEntities ().Where (m => m.Has<Triggerable>()).ToList();
			toTrigger.AddRange(maps.GetAllEntities ().Where (m => m.Has<Triggerable>()));

			foreach (var item in toTrigger) {
				player.Get<Execute> ().Add (new TriggerEntity (player, item));
			}
			StartIdle ();
		}

		private void OnAttacked(Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			var player = GetEntityByTag("player");
	
			effects.AttachEffect (player, "effects_damage", 400);
		}

		private void OnHealthChanged(Entity player)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));

			if (player.Get<Health> ().HP == 0)
				director.ActivateScene ("main");
		}

		private void StartIdle()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var player = GetEntityByTag("player");
			var loop = new Loop (new AnimSprite (player, sprites.GetSprite("hero_idle"), 600));

			player.Get<Execute> ().AddNew (loop, "movement");
			player.Get<State<HeroState>>().EState = HeroState.Idle;
		}
	}
}

