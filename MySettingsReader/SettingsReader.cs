using System;
using System.Collections.Generic;
using System.IO;
using DotNetCoreDecorators;
using Flurl.Http;
using MyYamlParser;

namespace MySettingsReader
{
    public static class SettingsReader
    {
        private const string SettingsUrlEnvVariable = "SETTINGS_URL";

        private static byte[] ReadingFromEnvVariable()
        {

            var settingsUrl = Environment.GetEnvironmentVariable(SettingsUrlEnvVariable);
            if (string.IsNullOrEmpty(settingsUrl))
            {
                Console.WriteLine(
                    $"Environment variable {SettingsUrlEnvVariable} is empty. Skipping reading settings from it");
                return null;
            }

            try
            {
                return settingsUrl.GetBytesAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read settings from: " + settingsUrl + "; " + e.Message);
                return null;
            }

        }

        private static byte[] ReadFromFileInHome(string homeFileName)
        {
            var settingsFile =
                Environment.GetEnvironmentVariable("HOME").AddLastSymbolIfNotExists(Path.DirectorySeparatorChar) +
                homeFileName;

            try
            {
                return File.ReadAllBytes(settingsFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could noy read settings from " + settingsFile + ". " + e.Message);
                return null;
            }
        }


        public static T GetSettings<T>(string homeFileName) where T : class, new()
        {

            var yaml = ReadingFromEnvVariable() ?? ReadFromFileInHome(homeFileName);

            if (yaml == null)
            {
                Console.WriteLine();
                throw new Exception("No settings found");
            }

            var notUsed = new List<string>();

            var result = MyYamlDeserializer.Deserialize<T>(yaml, notUsed);

            if (notUsed.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                throw new Exception("The line is not initialized: " + notUsed[0]);
            }

            return result;
        }


    }
}