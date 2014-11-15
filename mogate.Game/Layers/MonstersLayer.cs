using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace mogate
{
	public enum MonsterState
	{
		Idle,
		Chasing,
		Attacking
	};

	public class MonstersLayer : Layer
	{
		public MonstersLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated ()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID (x, y) == MapGridTypes.ID.Tunnel) {
						if (Utils.Rand.Next (100) < Globals.MONSTER_PROB[gameState.Level]) {
							var me = CreateEntity ();
							me.Register (new State<MonsterState> (MonsterState.Idle));
							me.Register (new Position (x, y));
							me.Register (new Health (2, 2));
							me.Register (new Attack (1));
							me.Register (new MoveSpeed (600));
							me.Register (new AttackSpeed (500));
							me.Register (new Attackable ((attacker) => OnAttacked(me, attacker)));
							me.Register (new Execute ());
							me.Register (new Patrol ());
							me.Register (new Drawable (sprites.GetSprite("monsters_mob"), new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));

							/*var updateLoop = new Loop (new ActionEntity(me, (_) => {
								UpdateMonster(me);
							}));
							me.Get<Execute> ().Add (updateLoop, "update_loop");*/
							me.Get<Execute> ().Add (new ActionEntity (me, (_) => {
								StartPatrol (me);
							}), "patrol_loop");
						}
					}
				}
			}

			if (gameState.Level == Globals.MAX_LEVELS - 1) {
				var boss = CreateEntity ();
				var bossRoom = map.GetRooms ().First ();
				var pos = new Point (bossRoom.Pos.X + Utils.Rand.Next (bossRoom.Width), bossRoom.Pos.Y + Utils.Rand.Next (bossRoom.Height));

				boss.Register (new State<MonsterState> (MonsterState.Idle));
				boss.Register (new Position (pos.X, pos.Y));
				boss.Register (new Health (10, 10));
				boss.Register (new Attack (2));
				boss.Register (new MoveSpeed (400));
				boss.Register (new AttackSpeed (300));
				boss.Register (new Attackable ((attacker) => OnAttacked(boss, attacker)));
				boss.Register (new Execute ());
				boss.Register (new Drawable (sprites.GetSprite("monsters_boss"), new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

				var updateLoop = new Loop (new ActionEntity(boss, (_) => {
					UpdateBoss(boss);
				}));
				boss.Get<Execute> ().Add (updateLoop, "update_loop");
			}
		}

		void StartPatrol(Entity monster)
		{
			monster.Get<Patrol>().Direction = Utils.RandomEnumValue<Utils.Direction> ();
			monster.Get<Patrol>().Steps = Utils.Rand.Next (10) + 3;
			monster.Get<Execute> ().Add (new ActionEntity (monster, (_) => {
				DoPatrol (monster);
			}), "patrol_loop");
		}

		void DoPatrol(Entity monster)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);

			var newPos = monster.Get<Position> ().MapPos;
			if (monster.Get<Patrol> ().Direction == Utils.Direction.Down)
				newPos.Y++;
			else if (monster.Get<Patrol> ().Direction == Utils.Direction.Up)
				newPos.Y--;
			else if (monster.Get<Patrol> ().Direction == Utils.Direction.Right)
				newPos.X++;
			else if (monster.Get<Patrol> ().Direction == Utils.Direction.Left)
				newPos.X--;

			if (map.GetID (newPos.X, newPos.Y) != MapGridTypes.ID.Tunnel) {
				monster.Get<Execute> ().Add (new ActionEntity (monster, (_) => {
					StartPatrol (monster);
				}), "patrol_loop");
			} else {
				var seq = new Sequence ();
				seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X*Globals.CELL_WIDTH, newPos.Y*Globals.CELL_HEIGHT), monster.Get<MoveSpeed>().Speed));
				seq.Add (new ActionEntity (monster, (_) => {
					monster.Get<Position> ().MapPos = newPos;
				}));
				seq.Add (new ActionEntity (monster, (_) => {
					monster.Get<Patrol> ().Steps--;
				})); 
				if (monster.Get<Patrol> ().Steps > 1) {
					seq.Add (new ActionEntity (monster, (_) => {
						DoPatrol (monster);
					}));
				} else {
					seq.Add (new ActionEntity (monster, (_) => {
						StartPatrol (monster);
					}));
				}
				monster.Get<Execute> ().Add (seq, "patrol_loop");
			}
		}

		void UpdateBoss (Entity boss)
		{
			if (boss.Get<State<MonsterState>> ().EState == MonsterState.Idle) {
				var newPos = ProtectArtefact (boss);

				if (boss.Get<Position> ().MapPos != newPos)
					MonsterMove (boss, newPos);
			}
		}

		void UpdateMonster (Entity monster)
		{
			if (monster.Get<State<MonsterState>> ().EState == MonsterState.Idle) {
				var newPos = TryChasePlayer (monster);

				if (monster.Get<Position> ().MapPos != newPos)
					MonsterMove (monster, newPos);
			}
		}

		Point TryChasePlayer (Entity monster)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);

			var player = Scene.GetLayer ("hero").GetEntityByTag("player");

			Point newPos = monster.Get<Position>().MapPos;
			Point curPos = monster.Get<Position>().MapPos;

			if (Utils.Dist (curPos, player.Get<Position> ().MapPos) < 6) {
				if (player.Get<Position> ().MapPos.X < monster.Get<Position> ().MapPos.X)
					newPos.X--;
				else if (player.Get<Position> ().MapPos.X > monster.Get<Position> ().MapPos.X)
					newPos.X++;
				else if (player.Get<Position> ().MapPos.Y < monster.Get<Position> ().MapPos.Y)
					newPos.Y--;
				else if (player.Get<Position> ().MapPos.Y > monster.Get<Position> ().MapPos.Y)
					newPos.Y++;

				if (map.GetID (newPos.X, newPos.Y) != MapGridTypes.ID.Tunnel)
					newPos = curPos;
			}
			return newPos;
		}

		Point ProtectArtefact (Entity boss)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);

			var player = Scene.GetLayer ("hero").GetEntityByTag("player");

			Point newPos = boss.Get<Position>().MapPos;
			Point curPos = boss.Get<Position>().MapPos;

			if (Utils.Dist (curPos, player.Get<Position> ().MapPos) < 10) {
				if (player.Get<Position> ().MapPos.X < boss.Get<Position> ().MapPos.X)
					newPos.X--;
				else if (player.Get<Position> ().MapPos.X > boss.Get<Position> ().MapPos.X)
					newPos.X++;
				else if (player.Get<Position> ().MapPos.Y < boss.Get<Position> ().MapPos.Y)
					newPos.Y--;
				else if (player.Get<Position> ().MapPos.Y > boss.Get<Position> ().MapPos.Y)
					newPos.Y++;

				if (map.GetID (newPos.X, newPos.Y) != MapGridTypes.ID.Room)
					newPos = curPos;
			}
			return newPos;
		}


		void MonsterMove (Entity monster, Point newPos)
		{
			var player = Scene.GetLayer ("hero").GetEntityByTag("player");

			Point curPos = monster.Get<Position>().MapPos;

			if (newPos == player.Get<Position> ().MapPos) {
				var seq = new Sequence ();
				seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X*Globals.CELL_WIDTH, newPos.Y*Globals.CELL_HEIGHT), monster.Get<AttackSpeed>().Speed));
				seq.Add (new AttackEntity (monster, player));
				seq.Add (new MoveSpriteTo (monster, new Vector2(curPos.X*Globals.CELL_WIDTH, curPos.Y*Globals.CELL_HEIGHT), monster.Get<AttackSpeed>().Speed));
				seq.Add (new ActionEntity(monster, (_) => {
					monster.Get<State<MonsterState>>().EState = MonsterState.Idle;
				}));
				monster.Get<Execute> ().Add (seq);
				monster.Get<State<MonsterState>>().EState = MonsterState.Attacking;
			} else {
				var seq = new Sequence ();
				seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X*Globals.CELL_WIDTH, newPos.Y*Globals.CELL_HEIGHT), monster.Get<MoveSpeed>().Speed));
				seq.Add (new ActionEntity (monster, (_) => {
					monster.Get<Position> ().MapPos = newPos;
				}));
				seq.Add (new ActionEntity(monster, (_) => {
					monster.Get<State<MonsterState>>().EState = MonsterState.Idle;
				}));
				monster.Get<Execute> ().Add (seq);
				monster.Get<State<MonsterState>>().EState = MonsterState.Chasing;
			}
		}


		void OnAttacked (Entity monster, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");

			effects.AttachEffect (monster, "effects_damage", 400);

			if (monster.Get<Health> ().HP == 0)
				RemoveEntityByTag (monster.Tag);
		}
	}
}

