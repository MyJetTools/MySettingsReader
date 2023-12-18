// See https://aka.ms/new-console-template for more information


using System.Text;
using MySettingsReader;

var bytes = SettingsReader.ReadingFromEnvVariable();


Console.WriteLine($"{Encoding.UTF8.GetString(bytes)}");