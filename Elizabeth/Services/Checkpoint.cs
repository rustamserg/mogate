using Microsoft.Xna.Framework;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;

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
            var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists("mogate.sav"))
            {
                isoStore.DeleteFile("mogate.sav");
            }

            using (var isoStream = new IsolatedStorageFileStream("mogate.sav", FileMode.CreateNew, isoStore))
            {
                var ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(isoStream, data);
            }
            State = CheckpointState.Ready;
		}

        void LoadCheckpoint()
        {
            var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists("mogate.sav"))
            {
                using (var isoStream = new IsolatedStorageFileStream("mogate.sav", FileMode.Open, isoStore))
                {
                    using (var reader = XmlDictionaryReader.CreateTextReader(isoStream, new XmlDictionaryReaderQuotas()))
                    {
                        var ser = new DataContractSerializer(typeof(T));
                        LastCheckpoint = (T)ser.ReadObject(reader, true);
                    }
                }
            }
            State = CheckpointState.Ready;
        }
	}
}