using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


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
		Random m_rand = new Random(DateTime.UtcNow.Millisecond);

		public MonstersLayer (Game game, string name, int z) : base(game, name, z)
		{
		}

		public override void OnActivated ()
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);
			var sprites = (ISpriteSheets)Game.Services.GetService (typeof(ISpriteSheets));

			RemoveAllEntities ();

			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					if (map.GetID (x, y) == MapGridTypes.ID.Tunnel) {
						if (m_rand.Next (100) < 10) {
							var me = CreateEntity ();
							me.Register (new State<MonsterState> (MonsterState.Idle));
							me.Register (new Position (x, y));
							me.Register (new Health (100));
							me.Register (new Attack (20));
							me.Register (new Attackable ((attacker) => OnAttacked(me, attacker)));
							me.Register (new Execute ());
							me.Register (new Drawable (sprites.GetSprite("monsters_mob"), new Vector2 (x * Globals.CELL_WIDTH, y * Globals.CELL_HEIGHT)));
						
							var updateLoop = new Loop (new ActionEntity(me, (_) => {
								UpdateMonster(me);
							}));
							me.Get<Execute> ().Add (updateLoop, "update_loop");
						}
					}
				}
			}
		}

		void UpdateMonster (Entity monster)
		{
			if (monster.Get<State<MonsterState>>().EState == MonsterState.Idle)
				UpdateIdleState (monster);
		}

		void UpdateIdleState (Entity monster)
		{
			Point newPos = TryChasePlayer(monster);

			if (monster.Get<Position>().MapPos != newPos)
				MonsterMove (monster, newPos);
		}

		Point TryChasePlayer (Entity monster)
		{
			var world = (IWorld)Game.Services.GetService (typeof(IWorld));
			var gameState = (IGameState)Game.Services.GetService (typeof(IGameState));

			var map = world.GetLevel(gameState.Level);

			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var gs = director.GetActiveScene ();
			var hero = (HeroLayer)gs.GetLayer ("hero");

			Point newPos = monster.Get<Position>().MapPos;
			Point curPos = monster.Get<Position>().MapPos;

			if (Utils.Dist (curPos, hero.Player.Get<Position> ().MapPos) < 6) {
				if (hero.Player.Get<Position> ().MapPos.X < monster.Get<Position> ().MapPos.X)
					newPos.X--;
				else if (hero.Player.Get<Position> ().MapPos.X > monster.Get<Position> ().MapPos.X)
					newPos.X++;
				else if (hero.Player.Get<Position> ().MapPos.Y < monster.Get<Position> ().MapPos.Y)
					newPos.Y--;
				else if (hero.Player.Get<Position> ().MapPos.Y > monster.Get<Position> ().MapPos.Y)
					newPos.Y++;

				if (map.GetID (newPos.X, newPos.Y) != MapGridTypes.ID.Tunnel)
					newPos = curPos;
			}
			return newPos;
		}

		void MonsterMove (Entity monster, Point newPos)
		{
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var gs = director.GetActiveScene ();
			var hero = (HeroLayer)gs.GetLayer ("hero");

			Point curPos = monster.Get<Position>().MapPos;

			if (newPos == hero.Player.Get<Position> ().MapPos) {
				var seq = new Sequence ();
				seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X*Globals.CELL_WIDTH, newPos.Y*Globals.CELL_HEIGHT), 500));
				seq.Add (new AttackEntity (monster, hero.Player));
				seq.Add (new MoveSpriteTo (monster, new Vector2(curPos.X*Globals.CELL_WIDTH, curPos.Y*Globals.CELL_HEIGHT), 500));
				seq.Add (new ActionEntity(monster, (_) => {
					monster.Get<State<MonsterState>>().EState = MonsterState.Idle;
				}));
				monster.Get<Execute> ().Add (seq);
				monster.Get<State<MonsterState>>().EState = MonsterState.Attacking;
			} else {
				var seq = new Sequence ();
				seq.Add (new MoveSpriteTo (monster, new Vector2(newPos.X*Globals.CELL_WIDTH, newPos.Y*Globals.CELL_HEIGHT), 600));
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
			var director = (IDirector)Game.Services.GetService (typeof(IDirector));
			var gs = director.GetActiveScene ();
			var effects = (EffectsLayer)gs.GetLayer ("effects");

			effects.AttachEffect (monster, "effects_damage", 400);

			if (monster.Get<Health> ().HP == 0)
				RemoveEntityByTag (monster.Tag);
		}
	}
}

