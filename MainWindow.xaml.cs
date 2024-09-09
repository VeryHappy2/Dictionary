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
            Langs = JsonService.GetAllLangs();
        }
        
        private async void CreateTranslation_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Translation.Text) || String.IsNullOrEmpty(WordName.Text)) 
            {
                CreateWordError.Text = "Translation or word name is empty";
            }

            await JsonService.WriteJsonAsync(new Word
                {
                    WordName = WordName.Text,
                    Translation = Translation.Text
                }, Lang.SelectedItem as string);
        }

        private async void FindByWord_Click(object sender, RoutedEventArgs e)
        {
            var result = await JsonService.FindWordByWordNameAsync(Lang.SelectedItem as string, WordName.Text);

            if (result == null) 
            {
                FindWordError.Text = "Not found or you didn't choose a langauge";
            }
            else
            {
                if (ListWordsWindow != null && ListWordsWindow.IsLoaded)
                {
                    ListWordsWindow.UpdateWords(result);
                }
                else
                {
                    ListWordsWindow = new ListWords(result);
                    ListWordsWindow.Show();
                }
            } 
        }

        private async void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            RemoveWordError.Text = await JsonService.RemoveWordAsync(Lang.SelectedItem as string, new Word { WordName = WordName.Text, Translation = Translation.Text });
        }
    }
}