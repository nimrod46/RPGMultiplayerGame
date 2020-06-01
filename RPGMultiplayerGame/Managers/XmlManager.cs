using System;
using System.IO;
using System.Xml.Serialization;

namespace RPGMultiplayerGame.Managers
{

    class XmlManager<T>
    {
        public Type Type;

        public XmlManager()
        {
            Type = typeof(T);
        }

        public T Load(string path)
        {
            T instance;
            try
            {
                using TextReader reader = new StreamReader(path);
                XmlSerializer xml = new XmlSerializer(Type);
                instance = (T)xml.Deserialize(reader);
            }
            catch (Exception)
            {
                throw;
            }
            return instance;
        }

        public void Save(string path, T obj)
        {
            using TextWriter writer = new StreamWriter(path);
            XmlSerializer xml = new XmlSerializer(Type);
            xml.Serialize(writer, obj);
        }
    }
}
