using System.Configuration;

namespace Common
{
    public class Config
    {
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}