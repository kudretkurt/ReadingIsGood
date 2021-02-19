using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ReadingIsGood.Shared
{
    public sealed class ApplicationConfiguration
    {
        private const string ConfigFileName = "configuration.json";
        private const string ConfigFilePathEnvironmentVariable = "configuration_path";

        private static readonly Lazy<ApplicationConfiguration> Lazy =
            new(() => new ApplicationConfiguration());

        private readonly string _configPath;

        private ApplicationConfiguration()
        {
            _configPath = GetConfigurationPath();

            var builder = new ConfigurationBuilder()
                .AddJsonFile(_configPath, false, true);

            Configuration = builder.Build();
        }

        public static ApplicationConfiguration Instance => Lazy.Value;
        private IConfiguration Configuration { get; }

        public T GetSection<T>(string configurationKey) where T : new()
        {
            if (string.IsNullOrEmpty(configurationKey)) throw new ArgumentNullException($"{nameof(configurationKey)}");

            try
            {
                var model = new T();

                var configurationSection = Configuration.GetSection(configurationKey);

                configurationSection.Bind(model, options => options.BindNonPublicProperties = true);

                return model;
            }
            catch (Exception e)
            {
                throw new Exception($" SectionName:{configurationKey} Error:{e.Message}", e);
            }
        }

        public T GetValue<T>(string configurationKey, object defaultValue = default) where T : IConvertible
        {
            T result;
            try
            {
                if (Configuration[configurationKey] == null && defaultValue == default)
                    throw new ArgumentNullException(
                        $"Configuration Value could not find for the give key:{configurationKey}. Configuration Location: {_configPath}");

                if (Configuration[configurationKey] == null && defaultValue != default)
                    return (T) Convert.ChangeType(defaultValue, typeof(T));

                if (typeof(T).IsEnum)
                    result = (T) Enum.Parse(typeof(T), Configuration[configurationKey]);
                else
                    result = (T) Convert.ChangeType(Configuration[configurationKey], typeof(T));

                if (string.IsNullOrEmpty(Convert.ToString(result, CultureInfo.InvariantCulture)))
                    throw new ArgumentNullException(nameof(configurationKey));
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException($"Could not convert!. Error:{e.Message}");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"GetValue Error:{e.Message}", e);
            }

            return result;
        }

        private string GetConfigurationPath()
        {
            var path = Environment.GetEnvironmentVariable(ConfigFilePathEnvironmentVariable,
                EnvironmentVariableTarget.Machine);
            return string.IsNullOrEmpty(path)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName)
                : path;
        }
    }
}