namespace Interfaces
{
    public interface IConfigurationService
    {
        bool GetBool(string group, string setting);
        int GetInt(string group, string setting);
        float GetFloat(string group, string setting);
        string GetString(string group, string setting);

        string[] GetStringArray(string group, string setting);

        bool HasSetting(string group, string setting);
    }
}
