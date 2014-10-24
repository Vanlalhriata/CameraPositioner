using EasyConfig;
using System;
using System.Collections.Generic;

namespace CameraPositioner.LocalComponents
{
    public class ConfigurationManager : Interfaces.IConfigurationService
    {
        private ConfigFile configFile;

        public ConfigurationManager(ConfigFile configFileParam)
        {
            configFile = configFileParam;
        }

        #region IConfigurationService implementation

        public bool GetBool(string group, string setting)
        {
            bool result = false;

            try
            {
                result = GetSetting(group, setting).GetValueAsBool();
            }
            catch (FormatException e)
            {
                ThrowException(group, setting, "Boolean", e);
            }

            return result;
        }

        public int GetInt(string group, string setting)
        {
            int result = -1;

            try
            {
                result = GetSetting(group, setting).GetValueAsInt();
            }
            catch (FormatException e)
            {
                ThrowException(group, setting, "Integer", e);
            }

            return result;
        }

        public float GetFloat(string group, string setting)
        {
            float result = -1f;

            try
            {
                result = GetSetting(group, setting).GetValueAsFloat();
            }
            catch (FormatException e)
            {
                ThrowException(group, setting, "Float", e);
            }

            return result;
        }

        public string GetString(string group, string setting)
        {
            string result = String.Empty;

            try
            {
                result = GetSetting(group, setting).GetValueAsString();
            }
            catch (FormatException e)
            {
                ThrowException(group, setting, "String", e);
            }

            return result;
        }

        public string[] GetStringArray(string group, string setting)
        {
            List<string> result = new List<string>();

            try
            {
                result = new List<string>(GetSetting(group, setting).GetValueAsStringArray());
            }
            catch (FormatException e)
            {
                ThrowException(group, setting, "Array of String", e);
            }

            return result.ToArray();
        }

        public bool HasSetting(string group, string setting)
        {
            if (null == configFile)
                throw new Exception("Configuration file was not assigned");

            bool result = false;

            try
            {
                result = configFile.SettingGroups[group].Settings.ContainsKey(setting);
            }
            catch (FormatException e)
            {
                ThrowException(group, setting, "-", e);
            }

            return result;
        }

        #endregion IConfigurationService implementation

        #region Private methods

        private Setting GetSetting(string group, string setting)
        {
            if (null == configFile)
                throw new Exception("Configuration file was not assigned");

            Setting result = null;

            try
            {
                result = configFile.SettingGroups[group].Settings[setting];
            }
            catch (System.Collections.Generic.KeyNotFoundException e)
            {
                ThrowException(group, setting, "-", e);
            }

            return result;
        }

        private void ThrowException(string group, string setting, string expectedType, Exception e)
        {
            throw new Exception("Missing configuration group or setting, or unexpected setting type in configuration file"
                                + ". Group: " + group
                                + ". Setting: " + setting
                                + ". Expected type: " + expectedType,
                                e);
        }

        #endregion Private methods

    }
}
