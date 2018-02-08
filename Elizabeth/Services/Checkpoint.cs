using System;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.GamerServices;
using System.Xml.Serialization;
using System.IO;

namespace Elizabeth
{
	public enum CheckpointState { Unsync, Loading, Saving, Ready };

	public interface ICheckpoint<T>
	{
		CheckpointState State { get; }
		T LastCheckpoint { get; }
		void SaveCheckpoint(T checkpoint);
	}

	public class Checkpoint<T> : GameComponent, ICheckpoint<T>
	{
		//private StorageDevice m_storageDevice;

		public CheckpointState State { get; private set; }
		public T LastCheckpoint { get; private set; }

		public Checkpoint (Game game) : base(game)
		{
			State = CheckpointState.Unsync;
			LastCheckpoint = default(T);
		}

		public override void Update (GameTime gameTime)
		{
			if (State == CheckpointState.Unsync) {
				LoadCheckpoint ();
			}
			base.Update(gameTime);
		}

		public void SaveCheckpoint(T data)
		{
			/*if (!Guide.IsVisible) {
				m_storageDevice = null;
				State = CheckpointState.Saving;
				StorageDevice.BeginShowSelector (PlayerIndex.One, SaveToDevice, data);
			}else*/ {
				State = CheckpointState.Ready;
			}
		}

		void LoadCheckpoint()
		{
			/*if (!Guide.IsVisible) {
				m_storageDevice = null;
				State = CheckpointState.Loading;
				StorageDevice.BeginShowSelector (PlayerIndex.One, LoadFromDevice, null);
			} else*/ {
				State = CheckpointState.Ready;
			}
		}

		void SaveToDevice(IAsyncResult result)
		{
			/*m_storageDevice = StorageDevice.EndShowSelector (result);
			var save = (T)result.AsyncState;

			if (m_storageDevice != null && m_storageDevice.IsConnected) {
				var res = m_storageDevice.BeginOpenContainer ("mogate", null, null);
				result.AsyncWaitHandle.WaitOne ();
				var container = m_storageDevice.EndOpenContainer (res);

				if (container.FileExists ("mogate.sav"))
					container.DeleteFile ("mogate.sav");

				var stream = container.CreateFile ("mogate.sav");
				var serializer = new XmlSerializer (typeof(T));
				serializer.Serialize (stream, save);
				stream.Close ();

				container.Dispose ();
				result.AsyncWaitHandle.Close ();
			}*/
			State = CheckpointState.Ready;
		}

		void LoadFromDevice(IAsyncResult result)
		{
			/*m_storageDevice = StorageDevice.EndShowSelector (result);
			var res = m_storageDevice.BeginOpenContainer ("mogate", null, null);
			result.AsyncWaitHandle.WaitOne ();
			var container = m_storageDevice.EndOpenContainer (res);
			result.AsyncWaitHandle.Close ();

			if (container.FileExists ("mogate.sav")) {
				T save = default(T);
				var stream = container.OpenFile("mogate.sav", FileMode.Open);

				try {
					var serializer = new XmlSerializer(typeof(T));
					save = (T)serializer.Deserialize(stream);
				} catch (SystemException) {}

				stream.Close ();
				container.Dispose ();
				LastCheckpoint = save;
			}*/
			State = CheckpointState.Ready;
		}
	}
}