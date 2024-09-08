using Dictianary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dictanary.Services
{
    public class JsonService
    {
        public async Task WriteJsonAsync(Word data, string filePath)
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.Combine(projectDirectory, @$"..\..\..\Data\{filePath}");
            Language lang;

            using (FileStream readStream = File.Open(relativePath, FileMode.Open, FileAccess.Read))
            {
                lang = await JsonSerializer.DeserializeAsync<Language>(readStream);

                lang.Words.Add(data);
            }

            using (FileStream writeStream = File.Open(relativePath, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(writeStream, lang, new JsonSerializerOptions { WriteIndented = true });
            }

        }

        public async Task WriteJsonAsync(string filePath, Language lang)
        {
            using (FileStream writeStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(writeStream, lang, new JsonSerializerOptions { WriteIndented = true });
            }
        }

        public List<string> GetAllLangs()
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.Combine(projectDirectory, @$"..\..\..\Data");
            List<string> langsFiles = Directory.GetFiles(relativePath).ToList();
            List<string> langs = new List<string>();
            foreach (var langsItem in langsFiles)
            {
                langs.Add(Path.GetFileName(langsItem));
            }
            return langs;
        }

        public async Task<string> CreateJsonFile(string name)
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.Combine(projectDirectory, @$"..\..\..\Data\{name}.json");

            string directoryPath = Path.GetDirectoryName(relativePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fs = File.Create(relativePath))
            {
                await JsonSerializer.SerializeAsync(fs, new Language { Name = name, Words = new List<Word>() });
            }

            return "File is created";
        }

        public async Task<Language> ReadJsonFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    return JsonSerializer.Deserialize<Language>(stream);
                }
            }
            else
            {
                return null!;
            }
        }

        public async Task<string> RemoveWordAsync(string filePath, Word wordName)
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.Combine(projectDirectory, @$"..\..\..\Data\{filePath}");

            var language = await ReadJsonFileAsync(relativePath);


            if (language != null) 
            { 
                Word word = language.Words.FirstOrDefault(x => x.Equals(wordName));

                if (word != null) 
                { 
                    language.Words.Remove(word);

                    await RewriteTheFileAsync(language, relativePath);

                    return "Word was removed";
                }
                return "Not found a word";
            }

            return "Not found a language";
        }

        public async Task<IEnumerable<Word>> FindWordByWordNameAsync(string filePath, string wordName) 
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.Combine(projectDirectory, @$"..\..\..\Data\{filePath}");

            var language = await ReadJsonFileAsync(relativePath);

            if (language == null) 
            {
                return null!;
            }

            foreach (var word in language.Words)
            {
                Console.WriteLine(word.WordName);
            }

            var words = language.Words.Where(x => x.WordName
                .ToLower()
                .Trim()
                .Contains(wordName.ToLower().Trim()));

            return words;
        }

        public async Task<IEnumerable<Word>> FindWordByTranslationAsync(string filePath, string wordName)
        {
            var language = await ReadJsonFileAsync(filePath);

            var words = language.Words.Where(x => x.WordName.ToLower().Contains(wordName));

            return words;
        }

        private async Task RewriteTheFileAsync<T>(T data, string path)
        {
            string json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(path, json);
        }
    }
}
