using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace mogate
{
	public enum PlayerState
	{
		Idle,
		Moving
	};

	public class PlayerLayer : Layer
	{
		Point m_toMove;
		bool m_isLevelCompleted = false;

		public PlayerLayer(Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated ()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var mapGrid = world.GetLevel(gameState.Level);

			var player = CreateEntity ("player");

			player.Register (new State<PlayerState> (PlayerState.Idle));
			player.Register (new Health (gameState.PlayerHealth, gameState.MaxPlayerHealth,
											() => OnHealthChanged(player)));
			player.Register (new Attack (gameState.PlayerAttack));
			player.Register (new Armor (gameState.PlayerArmor, gameState.MaxPlayerArmor));
			player.Register (new PointLight (6));
			player.Register (new MoveSpeed (Globals.PLAYER_MOVE_SPEED));
			player.Register (new IFFSystem (Globals.IFF_PLAYER_ID, 2));
			player.Register (new Attackable (OnAttacked));
			player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			player.Register (new Execute ());
			player.Register (new Consumable<ConsumableItems> ());
			player.Register (new Sprite (sprites.GetSprite ("hero_idle")));
			player.Register (new Drawable (new Vector2(mapGrid.StairDown.X * Globals.CELL_WIDTH, mapGrid.StairDown.Y * Globals.CELL_HEIGHT)));
			player.Register (new Clickable (new Rectangle (0, 0, Globals.CELL_WIDTH * Globals.WORLD_WIDTH, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT)));

			player.Get<Consumable<ConsumableItems>> ().Refill (ConsumableItems.Trap, gameState.PlayerTraps);
			player.Get<Clickable> ().LeftButtonPressed += OnMoveToPosition;
			player.Get<Clickable> ().OnTouch += OnMoveToPosition;
			player.Get<Clickable> ().RightButtonPressed += OnAction;
			player.Get<Clickable> ().OnDoubleTap += OnAction;

			player.Get<Execute> ().Add (new Loop (new ActionEntity (player, (_) => {
				UpdatePlayer (player);
			})), "player_update_loop");

			m_toMove = mapGrid.StairDown;
			StartIdle ();
		}

		public override void OnDeactivated ()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var player = GetEntityByTag("player");
			gameState.PlayerHealth = player.Get<Health> ().HP;
			gameState.PlayerArmor = player.Get<Armor> ().Value;
			gameState.PlayerTraps = player.Get<Consumable<ConsumableItems>> ().Amount (ConsumableItems.Trap);
		}
			
		private void UpdatePlayer (Entity player)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			if (KeyboardUtils.IsKeyPressed(Keys.Q))
				m_isLevelCompleted = true;

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			if (m_isLevelCompleted) {
				director.ActivateScene ("inter", TimeSpan.FromSeconds (1));
				return;
			}
					
			var mapGrid = world.GetLevel(gameState.Level);
			var mapPos = player.Get<Position>().MapPos;

			if (player.Get<State<PlayerState>>().EState == PlayerState.Idle) {

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
					player.Get<State<PlayerState>> ().EState = PlayerState.Moving;
				}
			}
		}

		private void OnMoveToPosition(Point clickPos)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var effects = (EffectsLayer)Scene.GetLayer ("effects");

			var mapGrid = world.GetLevel(gameState.Level);
			var actionPos = mapGrid.ScreenToWorld (clickPos.X, clickPos.Y);

			if (mapGrid.GetID (actionPos.X, actionPos.Y) != MapGridTypes.ID.Blocked
				&& m_toMove != actionPos) {
				m_toMove = actionPos;
				effects.SpawnEffect (m_toMove, "effects_marker", 200);
			}
		}

		private void OnAction(Point clickPos)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var mapGrid = world.GetLevel(gameState.Level);

			var actionPos = mapGrid.ScreenToWorld (clickPos.X, clickPos.Y);
			if (mapGrid.GetID (actionPos.X, actionPos.Y) == MapGridTypes.ID.Blocked)
				return;
				
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			var items = (ItemsLayer)Scene.GetLayer ("items");
			var player = GetEntityByTag("player");
			var mapPos = player.Get<Position>().MapPos;

			var item = items.GetAllEntities ().FirstOrDefault (m => m.Get<Position> ().MapPos == actionPos);
			if (item != default(Entity) && Utils.Dist(mapPos, actionPos) < 2) {
				if (item.Has<IFFSystem> ()) {
					if (player.Get<IFFSystem> ().IsFoe (item)) {
						effects.SpawnEffect (actionPos, "items_sword", 100);
						player.Get<Execute> ().Add (new AttackEntity (player, item));
					}
				}
			} else if (item == default(Entity)) {
				if (player.Get<Consumable<ConsumableItems>>().TryConsume(ConsumableItems.Trap, 1))
					items.AddTrap (actionPos);
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

		private void OnAttacked(Entity attacker, int damage)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			var hud = (HUDLayer)Scene.GetLayer ("hud");

			var player = GetEntityByTag("player");
			effects.AttachEffect (player, "effects_damage", 400);

			string feedbackMsg = string.Format ("Damaged: {0}", damage);
			hud.FeedbackMessage (feedbackMsg);
		}

		private void OnHealthChanged(Entity player)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var state = (IGameState)Game.Services.GetService (typeof(IGameState));

			if (player.Get<Health> ().HP == 0) {
				state.NewGame ();
				director.ActivateScene ("main");
			}
		}

		private void StartIdle()
		{
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));
			var player = GetEntityByTag("player");
			var loop = new Loop (new AnimSprite (player, sprites.GetSprite("hero_idle"), 600));

			player.Get<Execute> ().AddNew (loop, "movement");
			player.Get<State<PlayerState>>().EState = PlayerState.Idle;
		}
	}
}

