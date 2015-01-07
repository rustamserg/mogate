using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;


namespace mogate
{
	public class MonstersLayer : Layer
	{
		enum MonsterType { Monster, Boss };

		public MonstersLayer (Game game, string name, Scene scene, int z) : base(game, name, scene, z)
		{
		}

		public override void OnActivated ()
		{
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			int spawnDelay = Globals.MONSTER_SPAWN_DELAY_MSEC [gameState.Level];

			var spawner = CreateEntity ();
			spawner.Register (new Execute ());

			var seq = new Sequence ();
			seq.Add (new ActionEntity (spawner, SpawnBosses));
			seq.Add (new Loop (new ActionEntity (spawner, SpawnMonsters), spawnDelay));
			spawner.Get<Execute> ().Add (seq);
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
						var distToEnemy = Utils.DirectionDist (monsterPos, enemyPos, lookDir);
						if (distToEnemy < monster.Get<Perception> ().AlertDistance) {
							monster.Get<LookDirection> ().Direction = lookDir;
							monster.Get<Patrol> ().Steps = Math.Min (monster.Get<Patrol> ().MaxSteps,
								monster.Get<Perception> ().AlertDistance - distToEnemy + 1);
							if (monster.Has<DirectLight> ()) {
								monster.Get<DirectLight> ().LightColor = Color.Red;
							}
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

				var distToEnemy = Utils.DirectionDist (monsterPos, enemyPos, monster.Get<LookDirection> ().Direction);
				if (distToEnemy < monster.Get<Perception> ().AlertDistance) {
					monster.Get<Patrol> ().Steps += monster.Get<Perception> ().AlertDistance - distToEnemy + 1;
					if (monster.Has<DirectLight> ()) {
						monster.Get<DirectLight> ().LightColor = Color.Red;
					}
					break;
				} else {
					if (monster.Has<DirectLight> ()) {
						monster.Get<DirectLight> ().LightColor = Color.White;
					}
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
					var seq = new Sequence ();
					seq.Add (new Delay (Utils.ThrowDice(monster.Get<AttackSpeed>().Speed)));
					seq.Add (new AttackEntity (monster, foe));
					seq.Add (new TryPoisonEntity (monster, foe));
					seq.Add (new Delay (monster.Get<AttackSpeed>().Speed));
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
			if (monster.Get<Health> ().HP == 0) {
				RemoveEntityByTag (monster.Tag);
			
				var items = (ItemsLayer)Scene.GetLayer ("items");
				var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

				items.DropLoot (monster.Get<Position> ().MapPos, Archetypes.MonsterLoot, Globals.MONSTER_DROP_LOOT_WEIGHT[gameState.Level]);
			}
		}

		void SpawnMonsters(Entity spawner)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var map = world.GetLevel(gameState.Level);

			int maxWeight = Globals.MONSTER_SPAWN_WEIGHT [gameState.Level];
			int monsters = GetAllEntities ().Where (e => e.Has<State<MonsterType>> () && e.Get<State<MonsterType>> ().EState == MonsterType.Monster).Count();

			if (monsters < Globals.MONSTER_POPULATION [gameState.Level]) {
				var tunnels = map.GetTunnels ();
				var pos = tunnels [Utils.ThrowDice (tunnels.Count)];

				Archetypes.Monsters.Shuffle();

				foreach (var arch in Archetypes.Monsters) {
					var w = Utils.ThrowDice (maxWeight);
					var spriteName = string.Format ("monster_{0:D2}", arch ["sprite_index"]);
					if (arch ["spawn_weight"] >= w && arch["spawn_weight"] <= maxWeight) {
						var me = CreateEntity ();
						me.Register (new State<MonsterType> (MonsterType.Monster));
						me.Register (new Position (pos.X, pos.Y));
						me.Register (new Health (arch["health"], () => OnHealthChanged(me)));
						me.Register (new Attack (arch["attack"]));
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

						if (arch ["poison_chance"] > 0)
							me.Register (new Poison (arch["poison_damage"], arch["poison_chance"], arch["poison_effect_delay_msec"]));

						if (arch ["critical_chance"] > 0)
							me.Register (new CriticalHit (arch ["critical_chance"], arch ["critical_damage"]));
		
						if (arch ["visible"] == 1)
							me.Register (new DirectLight (Color.White));

						me.Get<Execute> ().Add (new ActionEntity (me, (_) => {
							StartPatrol (me);
						}), "patrol_loop");

						break;
					}
				}
			}
		}

		void SpawnBosses(Entity spawner)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			var map = world.GetLevel(gameState.Level);
			int maxWeight = Globals.BOSSES_SPAWN_WEIGHT [gameState.Level];
		
			foreach (var room in map.GetRooms()) {
				Archetypes.Bosses.Shuffle ();

				foreach (var arch in Archetypes.Bosses) {
					var w = Utils.ThrowDice (maxWeight);
					var spriteName = string.Format ("boss_{0:D2}", arch ["sprite_index"]);
					var pos = new Point (room.Pos.X + Utils.Rand.Next (room.Width), room.Pos.Y + Utils.Rand.Next (room.Height));

					if (arch ["spawn_weight"] >= w && arch["spawn_weight"] <= maxWeight) {
						var boss = CreateEntity ();
						boss.Register (new State<MonsterType> (MonsterType.Boss));
						boss.Register (new Position (pos.X, pos.Y));
						boss.Register (new Health (arch["health"], () => OnHealthChanged(boss)));
						boss.Register (new Attack (arch["attack"]));
						boss.Register (new MoveSpeed (arch["move_duration_msec"]));
						boss.Register (new AttackSpeed (arch["attack_duration_msec"]));
						boss.Register (new Attackable ((attacker, _) => OnAttacked(boss, attacker)));
						boss.Register (new Execute ());
						boss.Register (new Patrol (arch["patrol_min_steps"], arch["patrol_max_steps"]));
						boss.Register (new IFFSystem (Globals.IFF_MONSTER_ID));
						boss.Register (new LookDirection (Utils.Direction.Down));
						boss.Register (new Perception (arch["perception"]));
						boss.Register (new AllowedMapArea(MapGridTypes.ID.Room));
						boss.Register (new Sprite (sprites.GetSprite (spriteName)));
						boss.Register (new Drawable (new Vector2 (pos.X * Globals.CELL_WIDTH, pos.Y * Globals.CELL_HEIGHT)));

						if (arch ["poison_chance"] > 0)
							boss.Register (new Poison (arch["poison_damage"], arch["poison_chance"], arch["poison_effect_delay_msec"]));

						if (arch ["critical_chance"] > 0)
							boss.Register (new CriticalHit (arch ["critical_chance"], arch ["critical_damage"]));

						if (arch ["visible"] == 1)
							boss.Register (new DirectLight (Color.White));

						boss.Get<Execute> ().Add (new ActionEntity (boss, (_) => {
							StartPatrol (boss);
						}), "patrol_loop");

						break;
					}
				}
			}
		}
	}
}

