using Elizabeth;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace mogate
{
    public class HallOfFameEntry
	{
		public string PlayerName;
		public int PlayerSpriteID;
		public long TotalPlaytime;
	}

	public enum GameProgressState { Win, Death, InGame };

	public interface IGameState
	{
		int Level { get; }
		TimeSpan Playtime { get; }

		string PlayerName { get; }
		int PlayerSpriteID { get; set; }
		int PlayerHealth { get; set; }
		int PlayerHealthMax { get; set; }
		int PlayerMoney { get; set; }
		int PlayerMoveSpeed { get; set; }
		int PlayerAttackSpeed { get; set; }
		int PlayerViewDistanceType { get; set; }
		int PlayerAntitodPotions { get; set; }
		int PlayerAntitodPotionsMax { get; set; }

		int PlayerWeaponID { get; set; }
		int PlayerArmorID { get; set; }

		float PlayerAttackMultiplier { get; set; }
		float PlayerMoneyMultiplier { get; set; }
		float PlayerPoisonChanceMultiplier { get; set; }
		float PlayerAttackDistanceMultiplier { get; set; }

		GameProgressState GameProgress { get; set; }
		bool CountPlaytime { get; set; }

		List<HallOfFameEntry> HallOfFame { get; }
		 
		bool IsLoaded { get; }
		void NewGame ();
		void ContinueGame ();

		void NextLevel ();
		void ApplyArchetype (Dictionary<string, float> archetype);
	}

	[DataContract]
	public class MogateSaveData
    {
        [DataMember]
		public int Level;
        [DataMember]
        public long PlaytimeTicks;
        [DataMember]
        public int PlayerHealth;
        [DataMember]
        public int PlayerHealthMax;
        [DataMember]
        public int PlayerAntitodPotions;
        [DataMember]
        public int PlayerAntitodPotionsMax;
        [DataMember]
        public int PlayerMoney;
        [DataMember]
        public int PlayerViewDistanceType;
        [DataMember]
        public int PlayerAttackSpeed;
        [DataMember]
        public int PlayerMoveSpeed;
        [DataMember]
        public int PlayerSpriteID;
        [DataMember]
        public int PlayerWeaponID;
        [DataMember]
        public int PlayerArmorID;

        [DataMember]
        public float PlayerAttackMultiplier;
        [DataMember]
        public float PlayerMoneyMultiplier;
        [DataMember]
        public float PlayerPoisonChanceMultiplier;
        [DataMember]
        public float PlayerAttackDistanceMultiplier;

        [DataMember]
        public string PlayerName;
        [DataMember]
        public List<HallOfFameEntry> HallOfFame;
	}

	public class GameState : GameComponent, IGameState
	{
		public bool IsLoaded { get; private set; }
		public int Level { get; private set; }
		public TimeSpan Playtime { get; private set; }

		public GameProgressState GameProgress { get; set; }

		public int PlayerSpriteID { get; set; }
		public int PlayerHealth { get; set; }
		public int PlayerHealthMax { get; set; }
		public int PlayerViewDistanceType { get; set; }
		public int PlayerMoney { get; set; }
		public int PlayerMoveSpeed { get; set; }
		public int PlayerAttackSpeed { get; set; }
		public int PlayerAntitodPotions { get; set; }
		public int PlayerAntitodPotionsMax { get; set; }

		public float PlayerAttackMultiplier { get; set; }
		public float PlayerMoneyMultiplier { get; set; }
		public float PlayerPoisonChanceMultiplier { get; set; }
		public float PlayerAttackDistanceMultiplier { get; set; }

		public int PlayerWeaponID { get; set; }
		public int PlayerArmorID { get; set; }
		public string PlayerName { get; private set; }

		public bool CountPlaytime { get; set; }

		public List<HallOfFameEntry> HallOfFame { get; private set; }

        private ICheckpoint<MogateSaveData> Checkpoint { get; }

        public GameState(Game game) : base(game)
        {
            CountPlaytime = false;
            HallOfFame = new List<HallOfFameEntry>();
            InitGame();
            IsLoaded = false;
            Checkpoint = new Checkpoint<MogateSaveData>();
        }

		public override void Update (GameTime gameTime)
		{
			if (!IsLoaded) {
				LoadGame ();
			}

			if (CountPlaytime) {
				Playtime += gameTime.ElapsedGameTime;
			}
			base.Update(gameTime);
		}

		public void NewGame()
		{
			var world = (IWorld)Game.Services.GetService(typeof(IWorld));
			world.GenerateLevels (Globals.MAX_LEVELS);

			InitGame ();
			SaveGame ();
		}

		public void ContinueGame()
		{
			var world = (IWorld)Game.Services.GetService(typeof(IWorld));
			world.GenerateLevels (Globals.MAX_LEVELS);
		}

		public void NextLevel()
		{
			Level = Level + 1;
		
			if (Level == Globals.MAX_LEVELS) {
				GameProgress = GameProgressState.Win;
				Level = 0;
				UpdateHallOfFame ();
			}
			SaveGame ();
		}

		public void ApplyArchetype(Dictionary<string, float> archetype)
		{
			PlayerHealth = (int)archetype ["health_packs"] * Globals.HEALTH_PACK;
			PlayerHealthMax = (int)archetype ["health_packs_max"] * Globals.HEALTH_PACK;
			PlayerMoveSpeed = (int)archetype ["move_duration_msec"];
			PlayerWeaponID = 0;
			PlayerArmorID = -1;
			PlayerMoney = 0;
			PlayerAntitodPotions = 0;
			PlayerSpriteID = (int)archetype ["sprite_index"];
			PlayerName = NameGenerator.Generate ();
			PlayerAttackSpeed = (int)archetype ["attack_duration_msec"];
			PlayerViewDistanceType = (int)archetype ["view_distance_type"];
			PlayerAntitodPotionsMax = (int)archetype ["antitod_potions_max"];

			PlayerMoneyMultiplier = archetype ["money_multiplier"];
			PlayerAttackMultiplier = archetype ["attack_multiplier"];
			PlayerPoisonChanceMultiplier = archetype ["poison_chance_multiplier"];
			PlayerAttackDistanceMultiplier = archetype ["attack_distance_multiplier"];
		}

		void UpdateHallOfFame()
		{
			HallOfFame.Add (new HallOfFameEntry {
				PlayerName = this.PlayerName,
				PlayerSpriteID = this.PlayerSpriteID,
				TotalPlaytime = this.Playtime.Ticks
			});
			HallOfFame = HallOfFame.OrderBy(o => o.TotalPlaytime).ToList();
			int count = HallOfFame.Count;
			if (count > Globals.HALL_OF_FAME_SIZE) {
				HallOfFame.RemoveRange (Globals.HALL_OF_FAME_SIZE, count - Globals.HALL_OF_FAME_SIZE);
			}
		}

		void InitGame()
		{
			Level = 0;
			GameProgress = GameProgressState.InGame;
			Playtime = TimeSpan.Zero;

			var arch = Archetypes.Players.First ();
			ApplyArchetype (arch);
		}

        void LoadGame()
        {
            var save = Checkpoint.Load("mogate.sav");
            if (save != null)
            {
                Level = save.Level;
                PlayerName = save.PlayerName;
                Playtime = TimeSpan.FromTicks(save.PlaytimeTicks);
                PlayerHealth = save.PlayerHealth;
                PlayerHealthMax = save.PlayerHealthMax;
                PlayerAntitodPotions = save.PlayerAntitodPotions;
                PlayerAntitodPotionsMax = save.PlayerAntitodPotionsMax;
                PlayerMoney = save.PlayerMoney;
                PlayerViewDistanceType = save.PlayerViewDistanceType;
                PlayerAttackDistanceMultiplier = save.PlayerAttackDistanceMultiplier;
                PlayerMoneyMultiplier = save.PlayerMoneyMultiplier;
                PlayerAttackMultiplier = save.PlayerAttackMultiplier;
                PlayerPoisonChanceMultiplier = save.PlayerPoisonChanceMultiplier;
                PlayerAttackSpeed = save.PlayerAttackSpeed;
                PlayerMoveSpeed = save.PlayerMoveSpeed;
                PlayerSpriteID = save.PlayerSpriteID;
                PlayerArmorID = save.PlayerArmorID;
                PlayerWeaponID = save.PlayerWeaponID;
                HallOfFame = save.HallOfFame;
            }
            IsLoaded = true;
        }

		void SaveGame()
		{
			var save = new MogateSaveData {
				Level = this.Level,
				PlayerName = this.PlayerName,
				PlaytimeTicks = this.Playtime.Ticks,
				PlayerHealth = this.PlayerHealth,
				PlayerHealthMax = this.PlayerHealthMax,
				PlayerAntitodPotions = this.PlayerAntitodPotions,
				PlayerAntitodPotionsMax = this.PlayerAntitodPotionsMax,
				PlayerMoney = this.PlayerMoney,
				PlayerViewDistanceType = this.PlayerViewDistanceType,
				PlayerAttackDistanceMultiplier = this.PlayerAttackDistanceMultiplier,
				PlayerMoneyMultiplier = this.PlayerMoneyMultiplier,
				PlayerAttackMultiplier = this.PlayerAttackMultiplier,
				PlayerPoisonChanceMultiplier = this.PlayerPoisonChanceMultiplier,
				PlayerAttackSpeed = this.PlayerAttackSpeed,
				PlayerMoveSpeed = this.PlayerMoveSpeed,
				PlayerSpriteID = this.PlayerSpriteID,
				PlayerArmorID= this.PlayerArmorID,
				PlayerWeaponID = this.PlayerWeaponID,
				HallOfFame = this.HallOfFame
			};
			Checkpoint.Save(save, "mogate.sav");
		}
	}
}

