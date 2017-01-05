using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using MICMediaPlayer.Models;
using System.Net.Http;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MICMediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _slideTimer;
        private DispatcherTimer _pollTimer;
        private MySettings _mySettings;        

        public MainPage()
        {
            this.InitializeComponent();

            loadSlideShow();
        }

        private async Task loadSettings()
        {
            // Grab Settings
            var result = await ReadSettingsFile();
            _mySettings = JsonConvert.DeserializeObject<MySettings>(result);

            _slideTimer = new DispatcherTimer();
            _slideTimer.Interval = TimeSpan.FromSeconds(_mySettings.SlideInterval);
            _slideTimer.Tick += ChangeImage;

            _pollTimer = new DispatcherTimer();
            _pollTimer.Interval = TimeSpan.FromMinutes(_mySettings.PollInterval);
            _pollTimer.Tick += PollForSlides;
        }

        private async void loadSlideShow()
        {
            // load all settings from external file and configure timers
            await loadSettings();

            // Initial Load
            LoadMedia(true); 

            // Poll for new slides every X minutes
            _pollTimer.Start();    
                    
        }
        private async void LoadMedia(bool firstPlay = false)
        {

            List<DisplayItem> ItemsToDisplay = new List<DisplayItem>();

            try
            {
                // get new items to display
                ItemsToDisplay = await GetActiveItems();
            }
            catch (Exception)
            {
                // fetching new items failed
                return;
            }

            // fetching new items succeeded

            // clear current items
            fv.Items.Clear();

            foreach (DisplayItem d in ItemsToDisplay.OrderBy(o => o.OrderIndex))
            {
                Uri pathUri = new Uri(d.ImageUri);
                var image = new Image
                {
                    Source = new BitmapImage(pathUri)
                };
                fv.Items.Add(image);
            }

            // start timer to change slides
            if (firstPlay)            
                _slideTimer.Start();            

        }

        private void ChangeImage(object sender, object e)
        {
            var totalItems = fv.Items.Count;
            var newItemIndex = (fv.SelectedIndex + 1) % totalItems;            
            fv.SelectedIndex = newItemIndex;
        }

        private void PollForSlides(object sender, object e)
        {
            // Start over
            LoadMedia();
        }

        private async Task<List<DisplayItem>> GetActiveItems()
        {
            // Grab the slides from web api
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(_mySettings.MediaPlayerAPIUrl);
                HttpContent content = response.Content;
                string result = await content.ReadAsStringAsync();
                List<DisplayItem> newSlides = new List<DisplayItem>();
                newSlides = JsonConvert.DeserializeObject<List<DisplayItem>>(result);
                return newSlides;
            }

        }

        private async Task<string> ReadSettingsFile()
        {
            // Create a file in the root called settings.json and store 
            // settings in json format
            // make sure to set "Build Action" of file to "content"
            var uri = new System.Uri("ms-appx:///settings.json");
            var sampleFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await FileIO.ReadTextAsync(sampleFile);
        }

    }
}
