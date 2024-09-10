using Dictianary.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dictanary.Services
{
    public class JsonService
    {
        public async Task<string> WriteJsonAsync(Word word, string filePath)
        {
            string relativePath = GetRealativePath(filePath);

            Language lang = await ReadJsonFileAsync(filePath);

            if (lang == null)
                return "Language isn't exist";

            lang.Words.Add(word);

            using (FileStream writeStream = File.Open(relativePath, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(writeStream, lang, new JsonSerializerOptions { WriteIndented = true });
            }
            return "The word was created";
        }

        public ObservableCollection<string> GetAllLangs()
        {
            string relativePath = GetRealativePath();

            string directoryPath = Path.GetDirectoryName(relativePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var langs = new ObservableCollection<string>(
                Directory.GetFiles(relativePath)
                .Select(Path.GetFileNameWithoutExtension)
            );

            return langs;
        }

        public async Task<string> CreateJsonFile(string name)
        {
            string relativePath = GetRealativePath($"{name}.json");

            if (name == null) 
                return File.Exists(name) ? "Langauge is exist" : "The field of langauge is empty";

            string directoryPath = Path.GetDirectoryName(relativePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            await CreateLangaugeFileAsync(relativePath, new Language { Name = name, Words = new List<Word>() });

            return "File is created";
        }

        public async Task<Language> ReadJsonFileAsync(string filePath)
        {
            var relativePath = GetRealativePath(filePath);

            if (!File.Exists(relativePath))
                return null!;

            using (FileStream stream = File.Open(relativePath, FileMode.Open, FileAccess.Read))
            {
                return JsonSerializer.Deserialize<Language>(stream);
            }
        }

        public async Task<string> RemoveWordAsync(string filePath, Word wordName)
        {
            string relativePath = GetRealativePath(filePath);

            var language = await ReadJsonFileAsync(filePath);

            Word word = language?.Words.FirstOrDefault(x => x.Equals(wordName));

            if (word == null)
                return language == null ? "Not found a language" : "Not found a word";

            language!.Words.Remove(word);
            await RewriteFileAsync(language, relativePath);

            return "Success";
        }

        public async Task<IEnumerable<Word>?> FindWordByWordNameAsync(string filePath, string wordName) 
        {
            var language = await ReadJsonFileAsync(filePath);

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

        private async Task CreateLangaugeFileAsync(string filePath, Language language)
        {
            using (FileStream fs = File.Create(filePath))
            {
                await JsonSerializer.SerializeAsync(fs, language);
            }
        }

        private string GetRealativePath(string filePath = "") 
        {
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(projectDirectory, @$"..\..\..\Data\{filePath}");
        }
    }
}
