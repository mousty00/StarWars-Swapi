using Newtonsoft.Json;
using StarWars.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StarWars
{
    public sealed partial class CharacterPage : Page
    {
        private ObservableCollection<Character> charactersList = new ObservableCollection<Character>();
        CharacterService characterService = new CharacterService(new HttpClient());


        public CharacterPage()
        {
            InitializeComponent();
            _ = LoadData();
        }

        private async Task LoadData()
        {
            LoadingRing.IsActive = true;

            var characters = await characterService.GetCharacters();
            if (characters != null)
            {
                charactersList = new ObservableCollection<Character>(characters);
                PeopleListView.ItemsSource = charactersList;
            }

            LoadingRing.IsActive = false;
        }

        #region button view more details
        private void ViewMoreButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button.DataContext is Character ch)
            {
                ch.IsExpanded = !ch.IsExpanded;

                var stackPanel = (StackPanel)button.Parent;

                if (stackPanel?.FindName("DetailsPanel") is StackPanel detailsPanel)
                {
                    if (ch.IsExpanded)
                    {
                        detailsPanel.Visibility = Visibility.Visible;
                        button.Content = "View less";
                    }
                    else
                    {
                        detailsPanel.Visibility = Visibility.Collapsed;
                        button.Content = "View More";
                    }
                }
            }
        }
        #endregion

        #region search bar
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = Search.Text.ToLower();

            var filteredList = charactersList.Where(c =>
                c.name.ToLower().Contains(query) || c.PlanetInfo.name.ToLower().Contains(query)).ToList();

            PeopleListView.ItemsSource = new ObservableCollection<Character>(filteredList);
        }
        #endregion

        #region save and update data

        private string lastSaveFormat = string.Empty;

        private async void Save()
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Choose Save Format",
                    Content = "Select the format to save your data.",
                    PrimaryButtonText = "JSON",
                    SecondaryButtonText = "XML",
                    CloseButtonText = "Cancel"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    lastSaveFormat = "json";
                    await SaveAsJson();
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    lastSaveFormat = "xml";
                    await SaveAsXml();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
        }

        private async Task SaveAsJson()
        {
            var selectedCharacters = charactersList.Where(c => c.IsSelected).ToList();

            if (selectedCharacters.Count > 0)
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync("StarWars.json", CreationCollisionOption.ReplaceExisting);

                string json = JsonConvert.SerializeObject(selectedCharacters, Formatting.Indented);

                await FileIO.WriteTextAsync(file, json);

                var dialog = new ContentDialog
                {
                    Title = "Save Successful",
                    Content = "Your selected data has been saved as JSON.",
                    PrimaryButtonText = "Reload data",
                    CloseButtonText = "OK"
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    Load("json");
                }
            }
            else
            {
                await new ContentDialog
                {
                    Title = "No Selection",
                    Content = "No characters selected to save.",
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
        }

        private async Task SaveAsXml()
        {
            var selectedCharacters = charactersList.Where(c => c.IsSelected).ToList();

            if (selectedCharacters.Count > 0)
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync("StarWars.xml", CreationCollisionOption.ReplaceExisting);

                var xmlSerializer = new XmlSerializer(typeof(List<Character>));
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    xmlSerializer.Serialize(stream, selectedCharacters);
                }

                await new ContentDialog
                {
                    Title = "Save Successful",
                    Content = "Your selected data has been saved as XML.",
                    CloseButtonText = "OK"
                }.ShowAsync();

                Load("xml");
            }
            else
            {
                await new ContentDialog
                {
                    Title = "No Selection",
                    Content = "No characters selected to save.",
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
        }

        private async void Load(string format = "")
        {
            try
            {
                string fileName = format == "json" ? "StarWars.json" : "StarWars.xml";
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.GetFileAsync(fileName);
                var fileExtension = file.Name.Split('.').LastOrDefault()?.ToLower();

                string content = await FileIO.ReadTextAsync(file);

                if (fileExtension == "json")
                {
                    charactersList = JsonConvert.DeserializeObject<ObservableCollection<Character>>(content);
                }
                else if (fileExtension == "xml")
                {
                    var serializer = new XmlSerializer(typeof(ObservableCollection<Character>));
                    using (var reader = new StringReader(content))
                    {
                        charactersList = (ObservableCollection<Character>)serializer.Deserialize(reader);
                    }
                }

                if (charactersList != null && charactersList.Count > 0)
                {
                    PeopleListView.ItemsSource = charactersList;
                    charactersList.ToList().ForEach(c => c.IsSelected = false);
                    await new ContentDialog
                    {
                        Content = "Data loaded successfully.",
                        CloseButtonText = "OK"
                    }.ShowAsync();
                }
                else
                {
                    PeopleListView.ItemsSource = new ObservableCollection<Character>();
                    await new MessageDialog("The loaded data is empty.").ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                }.ShowAsync();
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _ = LoadData();
        }
        #endregion


    }
}
