using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;

namespace NumericalMethods.Core
{
    [DataContract]
    public sealed class Preferences : INotifyPropertyChanged
    {
        private Preferences()
        {
            Language = "ru";

            m_has_changes = true;
        }

        public static Preferences Instance
        {
            get
            {
                if (m_preferences == null)
                {
                    m_preferences = File.Exists(m_file_path) ? LoadPreferences() : new Preferences();
                }
                return m_preferences;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public string Language
        {
            get => m_language;
            set
            {
                m_language = value;
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Language)));
            }
        }

        private string m_language;
        private bool m_has_changes;
        private static Preferences m_preferences;
        private static string m_file_path = "Preferences.xml";

        private static Preferences LoadPreferences()
        {
            lock (m_file_path)
            {
                TextReader reader = null;
                try
                {
                    var serializer = new XmlSerializer(typeof(Preferences));
                    reader = new StreamReader(m_file_path);
                    return (Preferences)serializer.Deserialize(reader);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
        }

        public bool CommitChanges()
        {
            if (m_preferences == null)
            {
                return false;
            }

            lock (m_file_path)
            {
                if (m_preferences.m_has_changes == true)
                {
                    TextWriter writer = null;
                    try
                    {
                        var serializer = new XmlSerializer(typeof(Preferences));
                        writer = new StreamWriter(m_file_path, false);
                        serializer.Serialize(writer, m_preferences);
                    }
                    finally
                    {
                        if (writer != null)
                        {
                            writer.Close();
                        }
                    }
                }
                m_preferences.m_has_changes = false;
            }
            return true;
        }
    }
}
