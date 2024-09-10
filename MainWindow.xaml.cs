using Dictanary.Services;
using Dictianary.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dictionary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Langs {  get; set; }

        private JsonService JsonService = new JsonService();
        private ListWords ListWordsWindow {  get; set; }
        public MainWindow()
        {
            Langs = JsonService.GetAllLangs();
            InitializeComponent();
            this.DataContext = this;
        }

        private async void CreateLang_Click(object sender, RoutedEventArgs e)
        {
            ResultLang.Text = await JsonService.CreateJsonFile(lang.Text);
            Langs.Add(lang.Text);
        }
        
        private async void CreateTranslation_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Translation.Text) || String.IsNullOrEmpty(WordName.Text)) 
            {
                CreateWordError.Text = "Translation or word name is empty";
            }

            CreateWordError.Text = await JsonService.WriteJsonAsync(new Word
                {
                    WordName = WordName.Text,
                    Translation = Translation.Text
                }, $"{Lang.SelectedItem as string}.json");
        }

        private async void FindByWord_Click(object sender, RoutedEventArgs e)
        {
            var result = await JsonService.FindWordByWordNameAsync($"{Lang.SelectedItem as string}.json", WordName.Text);

            if (result == null) 
            {
                FindWordError.Text = "Not found or you didn't choose a language";
                return;
            }

            if (ListWordsWindow != null && ListWordsWindow.IsLoaded)
            {
                ListWordsWindow.UpdateWords(result);
                return;
            }

            ShowListWordsWindow(result);
        }

        private async void GetAllWords_Click(object sender, RoutedEventArgs e) 
        {
            var result = await JsonService.ReadJsonFileAsync($"{Lang.SelectedItem as string}.json");

            if (result == null || result.Words == null)
            {
                GetAllWords.Text = "Language hasn't any words";
                return;
            }

            if (ListWordsWindow != null && ListWordsWindow.IsLoaded)
            {
                ListWordsWindow.UpdateWords(result.Words);
                return;
            }

            ShowListWordsWindow(result.Words);
        }

        private async void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            RemoveWordError.Text = await JsonService.RemoveWordAsync(
                $"{Lang.SelectedItem as string}.json", 
                new Word 
                { 
                    WordName = WordName.Text, 
                    Translation = Translation.Text 
                });
        }

        private void ShowListWordsWindow(IEnumerable<Word> words)
        {
            ListWordsWindow = new ListWords(words);
            ListWordsWindow.Show();
        }
    }
}