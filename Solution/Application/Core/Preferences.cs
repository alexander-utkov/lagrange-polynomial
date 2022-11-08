using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;

namespace NumericalMethods.Core
{
    /// <summary>
    /// Предпочтения пользователя.
    /// </summary>
    /// <remarks>
    /// Средство доступа к данным об предпочтениях пользователя, сохраняемых на устройстве. Представляет собой одиночку.
    /// Экземпляр доступен через свойство <see cref="Instance"/>.
    /// </remarks>
    [DataContract]
    public sealed class Preferences : INotifyPropertyChanged
    {
        private Preferences()
        {
            PropertyChanged += Preferences_PropertyChanged;
        }

        public static Preferences Instance
        {
            get
            {
                if (m_preferences == null)
                {
                    if (File.Exists(m_file_path) == true)
                    {
                        m_preferences =  LoadPreferences();
                    }
                    else
                    {
                        m_preferences = new Preferences()
                        {
                            m_has_changes = true
                        };
                    }
                }
                return m_preferences;
            }
        }

        /// <summary>
        /// Событие, вызываемое при изменении значения любого свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Получает или задает язык пользовательского интерфейса.
        /// </summary>
        /// <remarks>
        /// Допустимые значения ограничены собственной локализацией приложения и локализацией HandyControls.
        /// </remarks>
        [DataMember]
        public string Language
        {
            get => m_language;
            set
            {
                if (value == "en" || value == "ru") m_language = value;
                else m_language = "en";

                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Language)));
            }
        }

        private string m_language = "ru";
        private bool m_has_changes = false;
        private static Preferences m_preferences = null;
        private static string m_file_path = "Preferences.xml";

        private void Preferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_has_changes = true;
        }

        /// <summary>
        /// Создает экземпляр из файла.
        /// </summary>
        /// <returns>Возвращает созданный экземпляр.</returns>
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

        /// <summary>
        /// Фиксирует изменения экземпляра в файле.
        /// </summary>
        public void CommitChanges()
        {
            lock (m_file_path)
            {
                if (m_has_changes == true)
                {
                    TextWriter writer = null;
                    try
                    {
                        var serializer = new XmlSerializer(typeof(Preferences));
                        writer = new StreamWriter(m_file_path, false);
                        serializer.Serialize(writer, this);
                    }
                    finally
                    {
                        if (writer != null)
                        {
                            writer.Close();
                        }
                    }
                }
                m_has_changes = false;
            }
        }
    }
}
