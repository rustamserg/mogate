using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;


namespace mogate
{
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
						if (Utils.DropChance(Globals.MONSTER_PROB[gameState.Level])) {
							var me = CreateEntity ();
							me.Register (new Position (x, y));
							me.Register (new Health (Globals.MONSTER_HEALTH[gameState.Level], () => OnHealthChanged(me)));
							me.Register (new Attack (Globals.MONSTER_ATTACK[gameState.Level]));
							me.Register (new MoveSpeed (Globals.MONSTER_MOVE_SPEED[gameState.Level]));
							me.Register (new AttackSpeed (Globals.MONSTER_ATTACK_SPEED[gameState.Level]));
							me.Register (new Attackable ((attacker) => OnAttacked(me, attacker)));
							me.Register (new Execute ());
							me.Register (new Patrol (3, 5));
							me.Register (new IFFSystem (Globals.IFF_MONSTER_ID, 0));
							me.Register (new LookDirection (Utils.Direction.Down));
							me.Register (new Perception (Globals.MONSTER_PERCEPTION[gameState.Level]));
							me.Register (new AllowedMapArea(MapGridTypes.ID.Tunnel));
							me.Register (new Drawable (sprites.GetSprite("monsters_mob"), new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));

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

				boss.Register (new Position (pos.X, pos.Y));
				boss.Register (new Health (Globals.BOSS_HEALTH, () => OnHealthChanged(boss)));
				boss.Register (new Attack (Globals.BOSS_ATTACK));
				boss.Register (new MoveSpeed (Globals.BOSS_MOVE_SPEED));
				boss.Register (new AttackSpeed (Globals.BOSS_ATTACK_SPEED));
				boss.Register (new Attackable ((attacker) => OnAttacked(boss, attacker)));
				boss.Register (new Execute ());
				boss.Register (new Patrol (1, 3));
				boss.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
				boss.Register (new LookDirection (Utils.Direction.Down));
				boss.Register (new Perception (Globals.BOSS_PERCEPTION));
				boss.Register (new AllowedMapArea(MapGridTypes.ID.Room));
				boss.Register (new Drawable (sprites.GetSprite("monsters_boss"), new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

				boss.Get<Execute> ().Add (new ActionEntity (boss, (_) => {
					StartPatrol (boss);
				}), "patrol_loop");
			}
		}

		void StartPatrol(Entity monster)
		{
			var targets = Scene.GetLayer ("player").GetAllEntities ().ToList ();
			targets.AddRange (Scene.GetLayer ("items").GetAllEntities ());
			targets.AddRange (GetAllEntities ());

			if (monster.Get<Patrol> ().Steps == 0) {
				if (!TryFindEnemyAround (monster, targets)) {
					PatrolRandomDirection (monster);
				}
			} else {
				TryFindEnemyOnRoute (monster, targets);
			}

			monster.Get<Execute> ().Add (new ActionEntity (monster, (_) => {
				DoPatrol (monster, targets);
			}), "patrol_loop");
		}


		bool TryFindEnemyAround(Entity monster, IEnumerable<Entity> targets)
		{
			if (!monster.Has<IFFSystem> ())
				return false;

			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var mapGrid = world.GetLevel(gameState.Level);

			foreach (var enemy in monster.Get<IFFSystem> ().GetFoes (targets)) {
				var enemyPos = enemy.Get<Position> ().MapPos;
				var monsterPos = monster.Get<Position> ().MapPos;

				var mapLine = mapGrid.GetLine (monsterPos, enemyPos);

				if (!mapLine.Any (e => e.Type != monster.Get<AllowedMapArea> ().Area)) {
					Utils.Direction lookDir;
					if (Utils.FindDirection (monsterPos, enemyPos, out lookDir)) {
						var distToEnemy = (int)Utils.DirectionDist (monsterPos, enemyPos, lookDir);
						if (distToEnemy < monster.Get<Perception> ().AlertDistance) {
							monster.Get<LookDirection> ().Direction = lookDir;
							monster.Get<Patrol> ().Steps = Math.Min (monster.Get<Patrol> ().MaxSteps,
								monster.Get<Perception> ().AlertDistance - distToEnemy + 1);
							return true;
						}
					}
				}
			}
			return false;
		}

		void TryFindEnemyOnRoute(Entity monster, IEnumerable<Entity> targets)
		{
			if (!monster.Has<IFFSystem> ())
				return;

			foreach (var enemy in monster.Get<IFFSystem> ().GetFoes (targets)) {
				var enemyPos = enemy.Get<Position> ().MapPos;
				var monsterPos = monster.Get<Position> ().MapPos;

				var distToEnemy = (int)Utils.DirectionDist (monsterPos, enemyPos, monster.Get<LookDirection> ().Direction);
				if (distToEnemy < monster.Get<Perception> ().AlertDistance) {
					monster.Get<Patrol> ().Steps += monster.Get<Perception> ().AlertDistance - distToEnemy + 1;
					break;
				}
			}
		}

		void PatrolRandomDirection(Entity monster)
		{
			monster.Get<LookDirection> ().Direction = Utils.RandomEnumValue<Utils.Direction> ();
			monster.Get<Patrol> ().Steps = Utils.Rand.Next (monster.Get<Patrol> ().MaxSteps - monster.Get<Patrol> ().MinSteps + 1) + monster.Get<Patrol> ().MinSteps;
		}

		void DoPatrol(Entity monster, IEnumerable<Entity> targets)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var map = world.GetLevel(gameState.Level);

			var foes = new List<Entity> ();
			if (monster.Has<IFFSystem> ())
				foes = monster.Get<IFFSystem> ().GetFoes (targets).ToList();

			var newPos = monster.Get<Position> ().MapPos;
			if (monster.Get<LookDirection> ().Direction == Utils.Direction.Down)
				newPos.Y++;
			else if (monster.Get<LookDirection> ().Direction == Utils.Direction.Up)
				newPos.Y--;
			else if (monster.Get<LookDirection> ().Direction == Utils.Direction.Right)
				newPos.X++;
			else if (monster.Get<LookDirection> ().Direction == Utils.Direction.Left)
				newPos.X--;

			if (map.GetID (newPos.X, newPos.Y) != monster.Get<AllowedMapArea>().Area) {
				monster.Get<Patrol> ().Steps = 0;
				monster.Get<Execute> ().Add (new ActionEntity (monster, (_) => {
					StartPatrol (monster);
				}), "patrol_loop");
			} else {
				var foe = foes.FirstOrDefault (e => e.Get<Position> ().MapPos == newPos);
				if (foe != null) {
					var stepBackPos = monster.Get<Position> ().MapPos;
					var seq = new Sequence ();
					seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X * Globals.CELL_WIDTH, newPos.Y * Globals.CELL_HEIGHT), monster.Get<AttackSpeed>().Speed));
					seq.Add (new AttackEntity (monster, foe));
					seq.Add (new MoveSpriteTo (monster, new Vector2(stepBackPos.X * Globals.CELL_WIDTH, stepBackPos.Y * Globals.CELL_HEIGHT), monster.Get<AttackSpeed>().Speed));
					seq.Add (new ActionEntity (monster, (_) => {
						StartPatrol (monster);
					}));
					monster.Get<Execute> ().Add (seq, "patrol_loop");
				} else {
					var seq = new Sequence ();
					seq.Add (new MoveSpriteTo (monster, new Vector2 (newPos.X * Globals.CELL_WIDTH, newPos.Y * Globals.CELL_HEIGHT), monster.Get<MoveSpeed> ().Speed));
					seq.Add (new ActionEntity (monster, (_) => {
						monster.Get<Position> ().MapPos = newPos;
					}));
					seq.Add (new ActionEntity (monster, (_) => {
						monster.Get<Patrol> ().Steps--;
					})); 
					seq.Add (new ActionEntity (monster, (_) => {
						StartPatrol (monster);
					}));
					monster.Get<Execute> ().Add (seq, "patrol_loop");
				}
			}
		}

		void OnAttacked (Entity monster, Entity attacker)
		{
			var effects = (EffectsLayer)Scene.GetLayer ("effects");
			effects.AttachEffect (monster, "effects_damage", 400);

		}

		void OnHealthChanged(Entity monster)
		{
			if (monster.Get<Health> ().HP == 0)
				RemoveEntityByTag (monster.Tag);
		}
	}
}

