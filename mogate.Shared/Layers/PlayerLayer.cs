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
			player.Register (new Attack (Archetypes.Weapons[gameState.PlayerWeaponID]["attack"], gameState.PlayerWeaponID));
			player.Register (new PointLight (6));
			player.Register (new MoveSpeed (Globals.PLAYER_MOVE_SPEED));
			player.Register (new IFFSystem (Globals.IFF_PLAYER_ID, 2));
			player.Register (new Attackable (OnAttacked));
			player.Register (new Position (mapGrid.StairDown.X, mapGrid.StairDown.Y));
			player.Register (new Execute ());
			player.Register (new Poisonable (OnPoisoned));
			player.Register (new Sprite (sprites.GetSprite (string.Format("player_{0:D2}", gameState.PlayerSpriteID))));
			player.Register (new Drawable (new Vector2(mapGrid.StairDown.X * Globals.CELL_WIDTH, mapGrid.StairDown.Y * Globals.CELL_HEIGHT)));
			player.Register (new Clickable (new Rectangle (0, 0, Globals.CELL_WIDTH * Globals.WORLD_WIDTH, Globals.CELL_HEIGHT * Globals.WORLD_HEIGHT)));
			player.Register (new Consumable<ConsumableTypes> ());

			player.Get<Clickable> ().OnLeftButtonPressed += OnMoveToPosition;
			player.Get<Clickable> ().OnMoved += OnMoveToPosition;
			player.Get<Clickable> ().OnRightButtonPressed += OnAction;
			player.Get<Clickable> ().OnTouched += OnAction;

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
		}
			
		private void UpdatePlayer (Entity player)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

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
					//spawn.Add (new AnimSprite (player, sprites.GetSprite("hero_move"), player.Get<MoveSpeed>().Speed));
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

			//var effects = (EffectsLayer)Scene.GetLayer ("effects");

			var mapGrid = world.GetLevel(gameState.Level);
			var actionPos = mapGrid.ScreenToWorld (clickPos.X, clickPos.Y);

			if (mapGrid.GetID (actionPos.X, actionPos.Y) != MapGridTypes.ID.Blocked
				&& m_toMove != actionPos) {
				m_toMove = actionPos;
				//effects.SpawnEffect (m_toMove, "effects_marker", 200);
			}
		}

		private void OnAction(Point clickPos)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var mapGrid = world.GetLevel(gameState.Level);

			var actionPos = mapGrid.ScreenToWorld (clickPos.X, clickPos.Y);
			var player = GetEntityByTag("player");
			var mapPos = player.Get<Position>().MapPos;

			var actionPosID = mapGrid.GetID (actionPos.X, actionPos.Y);
			var playerPosID = mapGrid.GetID (mapPos.X, mapPos.Y);

			if (actionPosID == MapGridTypes.ID.Blocked || actionPosID != playerPosID)
				return;

			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			var items = (ItemsLayer)Scene.GetLayer ("items");
			var monsters = (MonstersLayer)Scene.GetLayer ("monsters");

			if (Utils.Dist(mapPos, actionPos) < 2) {
				effects.SpawnEffect (actionPos, "weapon_01", 200);

				var actionTargets = items.GetAllEntities ().ToList();
				actionTargets.AddRange (monsters.GetAllEntities ().Where (e => e.Has<Position> ()));
				var target = actionTargets.FirstOrDefault (m => m.Get<Position> ().MapPos == actionPos);
				if (target != default(Entity)) {
					if (target.Has<IFFSystem> ()) {
						if (player.Get<IFFSystem> ().IsFoe (target)) {
							player.Get<Execute> ().Add (new AttackEntity (player, target));
						}
					}
				}
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
			effects.AttachEffect (player, "damage_01", 400);

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

		private void OnPoisoned(Entity player, int damage)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			var hud = (HUDLayer)Scene.GetLayer ("hud");

			effects.AttachEffect (player, "damage_01", 400);

			string feedbackMsg = string.Format ("Poisoned: {0}", damage);
			hud.FeedbackMessage (feedbackMsg);
		}

		private void StartIdle()
		{
			var player = GetEntityByTag("player");
			player.Get<State<PlayerState>>().EState = PlayerState.Idle;
		}
	}
}

