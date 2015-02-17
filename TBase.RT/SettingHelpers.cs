using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBase.RT
{
    public class SettingException : Exception
    {
        public SettingException() { }
        public SettingException(string message) : base(message) { }
        public SettingException(string message, Exception inner) : base(message, inner) { }
    }
    public static class SettingHelpers
    {

        public static void SetSetting<T>(string settingName, T setValue)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(settingName))
            {
                settings.Values.Remove(settingName);
            }
            settings.Values.Add(settingName, setValue);
        }

        public static T GetSetting<T>(string settingName ,bool isThrowException = false)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            T chackValue = default(T);
            if (settings.Values.ContainsKey(settingName))
            {
                chackValue = (T)settings.Values[settingName];
            }
            else
            {
                if (isThrowException)
                {
                    throw new SettingException("Can not Find " + settingName + " value");
                }
            }
            return chackValue;
        }

        public static bool GetSetting(string settingName, string onValue)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string chackValue = "";
            if (settings.Values.ContainsKey(settingName))
            {
                chackValue = (string)settings.Values[settingName];
            }
            else
            {
                return false;
            }
            return chackValue.Equals(onValue) ? true : false;
        }

    }
}
