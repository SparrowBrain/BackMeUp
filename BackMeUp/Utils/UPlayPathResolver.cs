using NLog;
using System;
using System.IO;

namespace BackMeUp.Utils
{
    public interface IUPlayPathResolver
    {
        string GetUPlayInstallationDirectory();
    }

    public class UPlayPathResolver : IUPlayPathResolver
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string GetUPlayInstallationDirectory()
        {
            var registryReader = GetRegistryReader();
            var uPlayInstallationDirectory = registryReader.GetValue(
                @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Uplay",
                "InstallLocation");

            if (!string.IsNullOrEmpty(uPlayInstallationDirectory))
            {
                return uPlayInstallationDirectory;
            }

            Logger.Warn("Failure to read uPlay installation directory from registry. Falling back to best guess.");

            uPlayInstallationDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    @"Ubisoft\Ubisoft Game Launcher");

            return uPlayInstallationDirectory;
        }

        protected virtual IRegistryReader GetRegistryReader()
        {
            return new RegistryReader();
        }
    }
}