using Dictianary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dictionary
{
    /// <summary>
    /// Interaction logic for ListWords.xaml
    /// </summary>
    public partial class ListWords : Window
    {
        public ObservableCollection<Word> Words { get; set; }

        public ListWords(IEnumerable<Word> words)
        {
            Words = new ObservableCollection<Word>(words); 
            InitializeComponent();
            this.DataContext = this;
        }

        public void UpdateWords(IEnumerable<Word> words)
        {
            Words.Clear();
            foreach (var word in words)
            {
                Words.Add(word);
            }

            this.DataContext = this;
        }
    }
}
