using Dictianary.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dictanary.Services
{
    public class JsonService
    {
        public async Task WriteJsonAsync(Word data, string filePath)
        {
            string relativePath = GetRealativePath(filePath);

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

        public ObservableCollection<string> GetAllLangs()
        {
            string relativePath = GetRealativePath();

            var langs = new ObservableCollection<string>(
                Directory.GetFiles(relativePath)
                .Select(Path.GetFileNameWithoutExtension)
            );

            return langs;
        }

        public async Task<string> CreateJsonFile(string name)
        {
            string relativePath = GetRealativePath($"{name}.json");

            if (name == null || File.Exists(name)) 
            {
                return "Langauge is exist or name of the langauge is null";
            }

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
            if (!File.Exists(filePath))
                return null!;

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                return JsonSerializer.Deserialize<Language>(stream);
            }
        }

        public async Task<string> RemoveWordAsync(string filePath, Word wordName)
        {
            string relativePath = GetRealativePath(filePath);

            var language = await ReadJsonFileAsync(relativePath);

            Word word = language?.Words.FirstOrDefault(x => x.Equals(wordName));

            if (word == null)
                return language == null ? "Not found a language" : "Not found a word";

            language!.Words.Remove(word);
            await RewriteFileAsync(language, relativePath);

            return "Not found a language";
        }

        public async Task<IEnumerable<Word>?> FindWordByWordNameAsync(string filePath, string wordName) 
        {
            string relativePath = GetRealativePath(filePath);

            var language = await ReadJsonFileAsync(relativePath);

            if (language == null) 
                return null!;

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

        private async Task RewriteFileAsync<T>(T data, string path)
        {
            string json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(path, json);
        }

        private string GetRealativePath(string filePath = "") 
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(projectDirectory, @$"..\..\..\Data\{filePath}");
        }
    }
}
