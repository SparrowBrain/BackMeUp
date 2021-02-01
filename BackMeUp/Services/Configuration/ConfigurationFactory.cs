﻿using System;
using NLog;

namespace BackMeUp.Services.Configuration
{
    public abstract class ConfigurationFactory<T>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected abstract string ConfigurationFile { get; }

        public T GetConfiguration()
        {
            var configurationReader = GetConfigurationReader();
            try
            {
                var configuration = configurationReader.Read(ConfigurationFile);
                return configuration;
            }
            catch (Exception ex)
            {
                if (_logger.IsWarnEnabled)
                {
                    _logger.WarnException("Could not read configuration in \"" + ConfigurationFile + "\"", ex);
                }
                return GetDefaultConfiguration();
            }
        }

        protected abstract IConfigurationReader<T> GetConfigurationReader();
        protected abstract T GetDefaultConfiguration();
    }
}