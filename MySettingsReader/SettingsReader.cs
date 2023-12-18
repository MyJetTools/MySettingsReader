using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using DotNetCoreDecorators;
using MyYamlParser;

namespace MySettingsReader
{
    public static class SettingsReader
    {
        public static bool DisableTraceConsoleLog = false;
        
        private const string SettingsUrlEnvVariable = "SETTINGS_URL";

        public static byte[] ReadingFromEnvVariable()
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
                using var client = new HttpClient();
                return client.GetByteArrayAsync(settingsUrl).GetAwaiter().GetResult();
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
            Console.WriteLine("Read settings yaml ...");
            
            var yaml = ReadingFromEnvVariable() ?? ReadFromFileInHome(homeFileName);

            if (yaml == null)
            {
                Console.WriteLine();
                throw new Exception("No settings found");
            }

            var notUsed = new List<string>();

            try
            {
                var result = MyYamlDeserializer.Deserialize<T>(yaml, notUsed);

                if (notUsed.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    throw new Exception("The line is not initialized: " + notUsed[0]);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot parse settings yaml: {ex}");
                if (!DisableTraceConsoleLog)
                {
                    Console.WriteLine("Yaml:");
                    Console.WriteLine(yaml);
                    Console.WriteLine();
                }

                throw new Exception("Cannot parse settings yaml", ex);
            }
        }


    }
}