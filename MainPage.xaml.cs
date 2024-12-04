using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Learn_a_language
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Sõna> WordList { get; set; } = new ObservableCollection<Sõna>();

        private Sõna _newWord;

        public Sõna NewWord
        {
            get => _newWord;
            set
            {
                _newWord = value;
                OnPropertyChanged(nameof(NewWord)); // Notify UI when NewWord changes
            }
        }

        private Sõna selectedWord; // Holds the selected word for editing

        public Command SaveWordCommand { get; }

        private string filePath;

        public MainPage()
        {
            InitializeComponent();

            NewWord = new Sõna { Kategooria = "õppimisel" };


            SaveWordCommand = new Command(SaveWord);

            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "words.txt");

            BindingContext = this;

            LoadWordsFromFileAsync();
        }

        private void EditWordClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var word = button?.BindingContext as Sõna;

            if (word != null)
            {
                selectedWord = word;

                // Populate input fields with the selected word's data
                NewWord = new Sõna
                {
                    Nimetus = word.Nimetus,
                    Tõlge = word.Tõlge,
                    Selgitus = word.Selgitus,
                    Kategooria = word.Kategooria
                };
            }
        }

        private void SaveWord()
        {
            if (selectedWord != null)
            {
                // Modify the existing word
                selectedWord.Nimetus = NewWord.Nimetus;
                selectedWord.Tõlge = NewWord.Tõlge;
                selectedWord.Selgitus = NewWord.Selgitus;
                selectedWord.Kategooria = NewWord.Kategooria;

                // Remove and add the modified word to refresh the UI
                var index = WordList.IndexOf(selectedWord);
                if (index >= 0)
                {
                    WordList.RemoveAt(index);
                    WordList.Insert(index, selectedWord);  // Reinsert the modified word
                }

                selectedWord = null; // Reset selected word
            }
            else
            {
                // Add new word
                WordList.Add(new Sõna
                {
                    Nimetus = NewWord.Nimetus,
                    Tõlge = NewWord.Tõlge,
                    Selgitus = NewWord.Selgitus,
                    Kategooria = NewWord.Kategooria
                });
            }

            // Clear input fields by resetting NewWord properties
            NewWord.Nimetus = string.Empty;
            NewWord.Tõlge = string.Empty;
            NewWord.Selgitus = string.Empty;
            NewWord.Kategooria = "õppimisel";  // Resetting to a default value

            SaveWordsToFileAsync();
        }


        private void DeleteWordClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var word = button?.BindingContext as Sõna;

            if (word != null && WordList.Contains(word))
            {
                WordList.Remove(word);
                SaveWordsToFileAsync();
            }
        }

        public async Task SaveWordsToFileAsync()
        {
            var lines = WordList.Select(w => $"{w.Nimetus}|{w.Tõlge}|{w.Selgitus}|{w.Kategooria}");
            await File.WriteAllLinesAsync(filePath, lines);
        }

        public async Task LoadWordsFromFileAsync()
        {
            if (File.Exists(filePath))
            {   

                Console.WriteLine($"Text file location: {filePath}");
                var lines = await File.ReadAllLinesAsync(filePath);
                WordList.Clear();
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        WordList.Add(new Sõna { Nimetus = parts[0], Tõlge = parts[1], Selgitus = parts[2], Kategooria = parts[3] });
                    }
                }
            }
        }



        // Back button functionality
        private void BackButtonClicked(object sender, EventArgs e)
        {
            if (wordCarousel.Position > 0)
            {
                wordCarousel.Position -= 1;
            }
        }

        // Forward button functionality
        private void ForwardButtonClicked(object sender, EventArgs e)
        {
            if (wordCarousel.Position < WordList.Count - 1)
            {
                wordCarousel.Position += 1;
            }
        }


      
    }
}

