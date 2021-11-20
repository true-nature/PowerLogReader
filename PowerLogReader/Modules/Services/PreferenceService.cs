using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace PowerLogReader.Modules.Services
{
    public class PreferenceService : IPreferenceService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly string PreferenceFileName = "preference.xml";
        private string PreferenceFilePath { get; set; }

        public Preference Preference { get; set; } = new Preference();

        public PreferenceService()
        {
            Load();
        }

        /// <summary>
        /// 設定ファイル保存先パスの取得
        /// </summary>
        /// <returns></returns>
        private string GetAppDataPath()
        {
            var appDataPath = new StringBuilder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            var entryAssebly = Assembly.GetEntryAssembly();
            object[] attributes = entryAssebly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if (attributes.Length > 0)
            {
                var company = attributes[0] as AssemblyCompanyAttribute;
                appDataPath.Append($"{Path.DirectorySeparatorChar}{company.Company}");
            }
            appDataPath.Append($"{Path.DirectorySeparatorChar}{entryAssebly.GetName().Name}");
            return appDataPath.ToString();
        }

        /// <summary>
        /// 設定値読み込み
        /// </summary>
        private void Load()
        {
            var appDataPath = GetAppDataPath();
            Directory.CreateDirectory(appDataPath);
            PreferenceFilePath = $"{appDataPath}{Path.DirectorySeparatorChar}{PreferenceFileName}";
            if (File.Exists(PreferenceFilePath))
            {
                var serializer = new DataContractSerializer(typeof(Preference));
                using (var stream = new FileStream(PreferenceFilePath, FileMode.Open))
                using (var reader = XmlReader.Create(stream))
                {
                    Preference = serializer.ReadObject(reader) as Preference;
                }
            }
            else
            {
                Save();
            }
        }

        /// <summary>
        /// 設定値保存
        /// </summary>
        public void Save()
        {
            var serializer = new DataContractSerializer(typeof(Preference));
            using (var stream = new FileStream(PreferenceFilePath, FileMode.Create))
            {
                var xmlSettings = new XmlWriterSettings() { Indent = true, Encoding=new UTF8Encoding(false) };
                using(var writer = XmlWriter.Create(stream, xmlSettings))
                {
                    serializer.WriteObject(writer, Preference);
                }
            }
        }
    }
}
