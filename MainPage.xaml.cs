using System.Text;
using System.Threading.Tasks;

namespace Deepgram_MAUI_Sample_Project
{
    public partial class MainPage : ContentPage
    {
        internal int count = 0;
        internal static string apiKey = "{API_KEY}";
        internal static string apiUrl = "https://api.deepgram.com/v1/listen?smart_format=true&model=nova-2&language=en-US";
        internal static string jsonPayload = "{\"url\":\"https://static.deepgram.com/examples/Bueller-Life-moves-pretty-fast.wav\"}";
        internal static string path = String.Empty;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        #region "Triggers"

        private async void Button_Clicked(object sender, EventArgs e)
        {
            PickOptions options = new PickOptions
            {
                PickerTitle = "Please select a file"
            };

            await FilePickerFunctionality(options);
        }

        private void btn_TranslateFile_Clicked(object sender, EventArgs e)
        {
            //Library.Initialize();

            // The API key we created in step 3
            //var deepgramClient = new ListenRESTClient(secret2);

            // Hosted sample file
            //var audioUrl = "https://static.deepgram.com/examples/Bueller-Life-moves-pretty-fast.wav";
            try
            {
                string result = Task.Run(async () => await MakeApiCall()).GetAwaiter().GetResult();
                editor_Response.Text = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            //Library.Terminate();
        }

        #endregion



        #region "METHODS"

        internal static async Task<string> MakeApiCall()
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepgram.com/v1/listen");
                client.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");
                request.Content = new StreamContent(File.OpenRead(path));
                var response = await client.SendAsync(request);
                // response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
        }


        private async Task FilePickerFunctionality(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(
                    new PickOptions
                    {
                        PickerTitle = "Please select a file",
                        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.iOS, new[] { "public.audio" } },
                            { DevicePlatform.Android, new[] { "audio/*", ".mp3", ".m4a" } },
                            { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".wma", ".m4a" } },
                            { DevicePlatform.Tizen, new[] { "*/*" } },
                            { DevicePlatform.macOS, new[] { "public.audio", ".mp3", ".wav", "m4a" } }
                        })
                    });
                if (result != null)
                {
                    e_fileName.Text = result.FullPath.ToString();
                    //using var stream = result.OpenReadAsync();
                    //var fileStream = stream.Result;
                    path = result.FullPath.ToString();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal static async Task<string> MakeApiCallForWebsiteSample()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
        }


        #endregion

        
    }
}
