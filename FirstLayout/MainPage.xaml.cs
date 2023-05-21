using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.System;

using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Data;

namespace FirstLayout
{
    public sealed partial class MainPage : Page
    {
        private Dictionary<string, List<MyDataClass>> data;
        private List<string> modVersions;

        public MainPage()
        {
            this.InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            string jsonUrl = "https://raw.githubusercontent.com/ComputerElite/ComputerElite.github.io/main/tools/Beat_Saber/mods.json"; // Replace with your JSON file URL

            HttpClient httpClient = new HttpClient();
            string jsonData = await httpClient.GetStringAsync(jsonUrl);

            data = JsonConvert.DeserializeObject<Dictionary<string, List<MyDataClass>>>(jsonData);

            // Get mod versions for dropdown and order them from highest to lowest
            modVersions = data.Keys.OrderByDescending(v => v, StringComparer.OrdinalIgnoreCase).ToList();
            VersionComboBox.ItemsSource = modVersions;

            // Set default selected version
            if (modVersions.Count > 0)
            {
                string defaultVersion = modVersions[0];
                VersionComboBox.SelectedItem = defaultVersion;
                UpdateModList(defaultVersion);
            }
        }

        private void VersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VersionComboBox.SelectedItem is string selectedVersion)
            {
                UpdateModList(selectedVersion);
            }
        }

        private void UpdateModList(string selectedVersion)
        {
            if (data.ContainsKey(selectedVersion))
            {
                List<MyDataClass> versionData = data[selectedVersion];
                ItemsListBox.ItemsSource = versionData;
            }
            else
            {
                ItemsListBox.ItemsSource = null;
            }
        }

        private async void DownloadButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string downloadUrl = button.Tag.ToString();

            Uri uri = new Uri(downloadUrl);
            await Launcher.LaunchUriAsync(uri);
        }

        private async void SourceButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string sourceUrl = button.Tag.ToString();

            Uri uri = new Uri(sourceUrl);
            await Launcher.LaunchUriAsync(uri);
        }

        public class MyDataClass
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Id { get; set; }
            public string Version { get; set; }
            public string Download { get; set; }
            public string Source { get; set; }
            public string Author { get; set; }
            public string Cover { get; set; }
        }
    }

    public class ImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string imageUrl)
            {
                if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uri))
                {
                    BitmapImage image = new BitmapImage(uri);
                    return image;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
