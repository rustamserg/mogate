using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Xml.Serialization;
using System.IO;

namespace mogate
{
	public interface IGameState
	{
		int Level { get; }
		TimeSpan Playtime { get; }

		int PlayerHealth { get; set; }
		int MaxPlayerHealth { get; }

		int PlayerArmor { get; set; }
		int MaxPlayerArmor { get; }

		int PlayerTraps { get; set; }
		int PlayerAttack { get; set; }

		bool IsGameEnd { get; }
		bool IsLoaded { get; }
		bool CountPlaytime { get; set; }

		void NewGame();
		void NextLevel();
	}

	[Serializable]
	public class SaveData
	{
		public int Level;
		public long PlaytimeTicks;
		public int PlayerHealth;
		public int PlayerArmor;
		public int PlayerTraps;
		public int PlayerAttack;
	}

	public class GameState : GameComponent, IGameState
	{
		StorageDevice m_storageDevice;

		public int Level { get; private set; }
		public TimeSpan Playtime { get; private set; }

		public bool IsGameEnd { get; private set; }

		public int PlayerHealth { get; set; }
		public int MaxPlayerHealth { get; private set; }

		public int PlayerArmor { get; set; }
		public int MaxPlayerArmor { get; private set; }

		public int PlayerTraps { get; set; }
		public int PlayerAttack { get; set; }

		public bool IsLoaded { get; private set; }
		public bool CountPlaytime { get; set; }


		public GameState (Game game) : base(game)
		{
			IsLoaded = false;
			CountPlaytime = false;
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
			InitGame ();
			SaveGame ();
		}

		public void NextLevel()
		{
			Level = Level + 1;
		
			if (Level == Globals.MAX_LEVELS) {
				IsGameEnd = true;
				Level = 0;
			}
			SaveGame ();
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
			PlayerArmor = Globals.PLAYER_ARMOR * Globals.ARMOR_PACK;
			MaxPlayerArmor = Globals.PLAYER_ARMOR_MAX * Globals.ARMOR_PACK;
			PlayerTraps = Globals.PLAYER_TRAPS;
			PlayerAttack = Globals.PLAYER_ATTACK;
		}

		void LoadGame()
		{
			InitGame ();

			if (!Guide.IsVisible) {
				m_storageDevice = null;
				StorageDevice.BeginShowSelector (PlayerIndex.One, LoadFromDevice, null);
			}
		}

		void SaveGame()
		{
			if (!Guide.IsVisible) {
				m_storageDevice = null;
				StorageDevice.BeginShowSelector (PlayerIndex.One, SaveToDevice, null);
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
					PlaytimeTicks = this.Playtime.Ticks,
					PlayerHealth = this.PlayerHealth,
					PlayerArmor = this.PlayerArmor,
					PlayerTraps = this.PlayerTraps,
					PlayerAttack = this.PlayerAttack
				};
				serializer.Serialize (stream, save);
				stream.Close ();

				container.Dispose ();
				result.AsyncWaitHandle.Close ();
			}
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
					Playtime = TimeSpan.FromTicks(save.PlaytimeTicks);
					PlayerHealth = save.PlayerHealth;
					PlayerArmor = save.PlayerArmor;
					PlayerTraps = save.PlayerTraps;
					PlayerAttack = save.PlayerAttack;
				}
			}
			IsLoaded = true;
		}
	}
}

