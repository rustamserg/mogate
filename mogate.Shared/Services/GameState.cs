using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace mogate
{
	public class HallOfFameEntry
	{
		public string PlayerName;
		public int PlayerSpriteID;
		public long TotalPlaytime;
	}

	public enum SaveDataState { Unsync, Loading, Saving, Ready };

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

		bool IsGameEnd { get; }
		SaveDataState DataState { get; }
		bool CountPlaytime { get; set; }

		List<HallOfFameEntry> HallOfFame { get; }

		void NewGame();
		void NextLevel();
		void ApplyArchetype (Dictionary<string, float> archetype);
	}

	[Serializable]
	public class SaveData
	{
		public int Level;
		public long PlaytimeTicks;
		public int PlayerHealth;
		public int PlayerHealthMax;
		public int PlayerAntitodPotions;
		public int PlayerAntitodPotionsMax;
		public int PlayerMoney;
		public int PlayerViewDistanceType;
		public int PlayerAttackSpeed;
		public int PlayerMoveSpeed;
		public int PlayerSpriteID;
		public int PlayerWeaponID;
		public int PlayerArmorID;

		public float PlayerAttackMultiplier;
		public float PlayerMoneyMultiplier;
		public float PlayerPoisonChanceMultiplier;
		public float PlayerAttackDistanceMultiplier;

		public string PlayerName;
		public List<HallOfFameEntry> HallOfFame;
	}

	public class GameState : GameComponent, IGameState
	{
		StorageDevice m_storageDevice;

		public int Level { get; private set; }
		public TimeSpan Playtime { get; private set; }

		public bool IsGameEnd { get; private set; }

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

		public SaveDataState DataState { get; private set; }
		public bool CountPlaytime { get; set; }

		public List<HallOfFameEntry> HallOfFame { get; private set; }


		public GameState (Game game) : base(game)
		{
			DataState = SaveDataState.Unsync;
			CountPlaytime = false;
			HallOfFame = new List<HallOfFameEntry> ();
		}

		public override void Update (GameTime gameTime)
		{
			if (DataState == SaveDataState.Unsync) {
				LoadGame ();
			}
			if (CountPlaytime) {
				Playtime += gameTime.ElapsedGameTime;
			}
			base.Update(gameTime);
		}

		public void NewGame()
		{
			InitGame ();
			SaveGame ();
		}

		public void NextLevel()
		{
			Level = Level + 1;
		
			if (Level == Globals.MAX_LEVELS) {
				IsGameEnd = true;
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
			var world = (IWorld)Game.Services.GetService(typeof(IWorld));
			world.GenerateLevels (Globals.MAX_LEVELS);

			Level = 0;
			IsGameEnd = false;
			Playtime = TimeSpan.Zero;

			var arch = Archetypes.Players.First ();
			ApplyArchetype (arch);
		}

		void LoadGame()
		{
			InitGame ();

			if (!Guide.IsVisible) {
				m_storageDevice = null;
				DataState = SaveDataState.Loading;
				StorageDevice.BeginShowSelector (PlayerIndex.One, LoadFromDevice, null);
			} else {
				DataState = SaveDataState.Ready;
			}
		}

		void SaveGame()
		{
			if (!Guide.IsVisible) {
				m_storageDevice = null;
				DataState = SaveDataState.Saving;
				StorageDevice.BeginShowSelector (PlayerIndex.One, SaveToDevice, null);
			}else {
				DataState = SaveDataState.Ready;
			}
		}

		void SaveToDevice(IAsyncResult result)
		{
			m_storageDevice = StorageDevice.EndShowSelector (result);

			if (m_storageDevice != null && m_storageDevice.IsConnected) {
				var res = m_storageDevice.BeginOpenContainer ("mogate", null, null);
				result.AsyncWaitHandle.WaitOne ();
				var container = m_storageDevice.EndOpenContainer (res);

				if (container.FileExists ("mogate.sav"))
					container.DeleteFile ("mogate.sav");

				var stream = container.CreateFile ("mogate.sav");
				var serializer = new XmlSerializer (typeof(SaveData));

				var save = new SaveData {
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

				serializer.Serialize (stream, save);
				stream.Close ();

				container.Dispose ();
				result.AsyncWaitHandle.Close ();
			}
			DataState = SaveDataState.Ready;
		}

		void LoadFromDevice(IAsyncResult result)
		{
			m_storageDevice = StorageDevice.EndShowSelector (result);
			var res = m_storageDevice.BeginOpenContainer ("mogate", null, null);
			result.AsyncWaitHandle.WaitOne ();
			var container = m_storageDevice.EndOpenContainer (res);
			result.AsyncWaitHandle.Close ();

			if (container.FileExists ("mogate.sav")) {
				SaveData save = null;
				var stream = container.OpenFile("mogate.sav", FileMode.Open);

				try {
					var serializer = new XmlSerializer(typeof(SaveData));
					save = (SaveData)serializer.Deserialize(stream);
				} catch (SystemException) {}

				stream.Close ();
				container.Dispose ();

				if (save != null) {
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
			}
			DataState = SaveDataState.Ready;
		}
	}
}

