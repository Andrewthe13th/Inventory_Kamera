using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace InventoryKamera.Properties
{
    /// <summary>
    /// Simple JSON SettingsProvider with only support of <see cref="SettingsSerializeAs.String"/> serialized properties
    /// with <see cref="UserScopedSettingAttribute"/>
    /// </summary>
    /// <exception cref="ConfigurationErrorsException"></exception>
    public class JsonUserSettingsProvider : SettingsProvider
    {
        public static string SettingsFileName => "settings.json";

        public static string SettingsDirectory => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Path.GetFileNameWithoutExtension(Application.ExecutablePath));

        private static string SettingsFile => Path.Combine(SettingsDirectory, SettingsFileName);

        public override string ApplicationName
        {
            get => Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            set { }
        }

        public override string Name => "JsonUserSettingsProvider";

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(Name, config);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection properties)
        {
            var values = new SettingsPropertyValueCollection();

            var settingsJson = new JObject();

            Directory.CreateDirectory(SettingsDirectory);

            if (File.Exists(SettingsFile))
            {
                try
                {
                    settingsJson = JObject.Parse(File.ReadAllText(SettingsFile));
                }
                catch (JsonReaderException)
                {
                    // Using default setting cause file isn't valid JSON
                }
            }

            foreach (SettingsProperty property in properties)
            {
                bool isUserSetting =
                    property.Attributes[typeof(UserScopedSettingAttribute)] is UserScopedSettingAttribute;
                if (!isUserSetting)
                {
                    throw new ConfigurationErrorsException("Application scoped properties aren't supported");
                }

                string propertyName = property.Name;
                var value = new SettingsPropertyValue(property) {IsDirty = false};

                JToken settingToken = settingsJson[propertyName];
                if (settingToken != null)
                {
                    if (property.SerializeAs != SettingsSerializeAs.String)
                    {
                        throw new ConfigurationErrorsException(
                            "Propeties with serialization otherwise String aren't supported");
                    }

                    value.SerializedValue = settingToken.ToString();
                }
                else
                {
                    value.SerializedValue = property.DefaultValue;
                }

                values.Add(value);
            }

            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            var settingsJson = new JObject();

            foreach (SettingsPropertyValue value in values)
            {
                bool isUserSetting =
                    value.Property.Attributes[typeof(UserScopedSettingAttribute)] is UserScopedSettingAttribute;

                if (!isUserSetting)
                {
                    throw new ConfigurationErrorsException("Application scoped properties aren't supported");
                }

                if (value.Property.SerializeAs != SettingsSerializeAs.String)
                {
                    throw new ConfigurationErrorsException(
                        "Propeties with serialization otherwise String aren't supported");
                }

                settingsJson[value.Name] = new JValue((string) value.SerializedValue);
            }

            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(settingsJson, Formatting.Indented));
        }
    }
}