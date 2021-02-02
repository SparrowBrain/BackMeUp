using Microsoft.Win32;
using NLog;
using System;

namespace BackMeUp.Utils
{
    public interface IRegistryReader
    {
        string GetValue(string key, string value);
    }

    public class RegistryReader : IRegistryReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string GetValue(string key, string value)
        {
            try
            {
                using (var registryKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (registryKey == null)
                    {
                        Logger.Debug("Registry Key \"" + key + "\" not found.");
                        return null;
                    }

                    var valueObject = registryKey.GetValue(value);
                    if (valueObject == null)
                    {
                        Logger.Debug("Registry Value \"" + value + "\" not found in Registry Key \"" + key + "\".");
                        return null;
                    }

                    return valueObject.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failure getting registry key \"" + key + "\", value \"" + value + "\"");
                return null;
            }
        }
    }
}