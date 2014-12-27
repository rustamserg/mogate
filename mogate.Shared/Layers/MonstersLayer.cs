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

			var tunnels = map.GetTunnels ();
			int maxWeight = Globals.MONSTER_SPAWN_WEIGHT [gameState.Level];

			for (int i = 0; i < Globals.MONSTER_POPULATION [gameState.Level]; i++) {
				var pos = tunnels [Utils.ThrowDice (tunnels.Count)];

				foreach (var arch in Archetypes.Monsters) {
					var w = Utils.Rand.Next (maxWeight);
					var spriteName = string.Format ("monster_{0:D2}", arch ["sprite_index"]);
					if (arch ["spawn_weight"] >= w) {
						var me = CreateEntity ();
						me.Register (new Position (pos.X, pos.Y));
						me.Register (new Health (arch["health"], () => OnHealthChanged(me)));
						me.Register (new Attack (arch["attack"]));
						me.Register (new Poison (arch["poison_damage"], arch["poison_chance"], arch["poison_effect_delay_msec"]));
						me.Register (new MoveSpeed (arch["move_duration_msec"]));
						me.Register (new AttackSpeed (arch["attack_duration_msec"]));
						me.Register (new Attackable ((attacker, _) => OnAttacked(me, attacker)));
						me.Register (new Execute ());
						me.Register (new Patrol (arch["patrol_min_steps"], arch["patrol_max_steps"]));
						me.Register (new IFFSystem (Globals.IFF_MONSTER_ID, 0));
						me.Register (new LookDirection (Utils.Direction.Down));
						me.Register (new Perception (arch["perception"]));
						me.Register (new AllowedMapArea(MapGridTypes.ID.Tunnel));
						me.Register (new Sprite (sprites.GetSprite (spriteName)));
						me.Register (new Drawable (new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

						me.Get<Execute> ().Add (new ActionEntity (me, (_) => {
							StartPatrol (me);
						}), "patrol_loop");

						break;
					}
				}
			}

			if (gameState.Level == Globals.MAX_LEVELS - 1) {
				var bossArch = Archetypes.Bosses [0];
				var spriteName = string.Format ("boss_{0:D2}", bossArch ["sprite_index"]);
				var boss = CreateEntity ();
				var bossRoom = map.GetRooms ().First ();
				var pos = new Point (bossRoom.Pos.X + Utils.Rand.Next (bossRoom.Width), bossRoom.Pos.Y + Utils.Rand.Next (bossRoom.Height));

				boss.Register (new Position (pos.X, pos.Y));
				boss.Register (new Health (bossArch["health"], () => OnHealthChanged(boss)));
				boss.Register (new Attack (bossArch["attack"]));
				boss.Register (new Poison (bossArch["poison_damage"], bossArch["poison_chance"], bossArch["poison_effect_delay_msec"]));
				boss.Register (new MoveSpeed (bossArch["move_duration_msec"]));
				boss.Register (new AttackSpeed (bossArch["attack_duration_msec"]));
				boss.Register (new Attackable ((attacker, _) => OnAttacked(boss, attacker)));
				boss.Register (new Execute ());
				boss.Register (new Patrol (bossArch["patrol_min_steps"], bossArch["patrol_max_steps"]));
				boss.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
				boss.Register (new LookDirection (Utils.Direction.Down));
				boss.Register (new Perception (bossArch["perception"]));
				boss.Register (new AllowedMapArea(MapGridTypes.ID.Room));
				boss.Register (new Sprite (sprites.GetSprite (spriteName)));
				boss.Register (new Drawable (new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

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
					seq.Add (new TryPoisonEntity (monster, foe));
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
			effects.AttachEffect (monster, "damage_01", 400);
		}

		void OnHealthChanged(Entity monster)
		{
			if (monster.Get<Health> ().HP == 0)
				RemoveEntityByTag (monster.Tag);
		}
	}
}

