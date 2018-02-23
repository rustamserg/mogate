using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;

namespace Elizabeth
{
    public interface ICheckpoint<T>
	{
        T Load(string filename);
		void Save(T checkpoint, string filename);
	}

	public class Checkpoint<T> : ICheckpoint<T>
	{
		public void Save(T data, string filename)
		{
            var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists(filename))
            {
                isoStore.DeleteFile(filename);
            }

            using (var isoStream = new IsolatedStorageFileStream(filename, FileMode.CreateNew, isoStore))
            {
                var ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(isoStream, data);
            }
		}

        public T Load(string filename)
        {
            var isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists(filename))
            {
                using (var isoStream = new IsolatedStorageFileStream(filename, FileMode.Open, isoStore))
                {
                    using (var reader = XmlDictionaryReader.CreateTextReader(isoStream, new XmlDictionaryReaderQuotas()))
                    {
                        var ser = new DataContractSerializer(typeof(T));
                        return (T)ser.ReadObject(reader, true);
                    }
                }
            }
            return default(T);
        }
	}
}