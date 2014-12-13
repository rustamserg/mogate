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
		public long TotalPlaytime;
	}

	public enum SaveDataState { Unsync, Loading, Saving, Ready };

	public interface IGameState
	{
		int Level { get; }
		TimeSpan Playtime { get; }

		string PlayerName { get; }
		int PlayerHealth { get; set; }
		int MaxPlayerHealth { get; }

		int PlayerTraps { get; set; }
		int PlayerAttack { get; set; }

		bool IsGameEnd { get; }
		SaveDataState DataState { get; }
		bool CountPlaytime { get; set; }

		List<HallOfFameEntry> HallOfFame { get; }

		void NewGame();
		void NextLevel();
	}

	[Serializable]
	public class SaveData
	{
		public int Level;
		public long PlaytimeTicks;
		public int PlayerHealth;
		public int PlayerTraps;
		public int PlayerAttack;
		public string PlayerName;
		public List<HallOfFameEntry> HallOfFame;
	}

	public class GameState : GameComponent, IGameState
	{
		StorageDevice m_storageDevice;

		public int Level { get; private set; }
		public TimeSpan Playtime { get; private set; }

		public bool IsGameEnd { get; private set; }

		public int PlayerHealth { get; set; }
		public int MaxPlayerHealth { get; private set; }

		public int PlayerTraps { get; set; }
		public int PlayerAttack { get; set; }
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

		void UpdateHallOfFame()
		{
			HallOfFame.Add (new HallOfFameEntry {
				PlayerName = this.PlayerName,
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

			PlayerHealth = Globals.PLAYER_HEALTH * Globals.HEALTH_PACK;
			MaxPlayerHealth = Globals.PLAYER_HEALTH_MAX * Globals.HEALTH_PACK;
			PlayerTraps = 0;
			PlayerAttack = Globals.PLAYER_ATTACK;
			PlayerName = NameGenerator.Generate ();
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
					PlayerTraps = this.PlayerTraps,
					PlayerAttack = this.PlayerAttack,
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
					PlayerTraps = save.PlayerTraps;
					PlayerAttack = save.PlayerAttack;
					HallOfFame = save.HallOfFame;
				}
			}
			DataState = SaveDataState.Ready;
		}
	}
}

