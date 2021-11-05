using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;
using System.Net;
using System.Threading;
using System.ComponentModel;
using Flurl.Http;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Controls;
using SpotifyAPI_.NET_Framework;
using SpotifyAPI_.NET_Framework.SpotifyTypes;

namespace TestSpotifyServerAPI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpotifyAPI api;

        public MainWindow()
        {
            InitializeComponent();

            /*browser.CoreWebView2InitializationCompleted += Browser_Initialized;
          
            InitializeBrowser();

            this.Closing += MainWindow_Closing;

            _server = new BackgroundWorker() { WorkerSupportsCancellation = true };
            _server.DoWork += RunServer;


            _server.RunWorkerAsync();*/

            api = api = new SpotifyAPI(_my_client_id,_my_client_secret);

            api.StatusChanged += Api_StatusChanged;
        }

        private async void Api_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            SetStatus(e.status.ToString());
            if(e.status == SpotifyAPI.Status.Ready)
            {
                UserPlaylistsResponse resp = await api.FetchPlaylists();


                foreach (SimplifiedPlaylistObject item in resp.items)
                {
                    this.Dispatcher.Invoke(() => {
                        Image cover = new Image()
                        {
                            Source = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri((item.images[0].url ?? ""))),
                            Width = 100,
                            Height = 100
                        };

                        Grid container = new Grid();
                        container.Children.Add(cover);
                        container.Children.Add(new Label() { Content = item.name });

                        //foreach(string key in ((IDictionary<String, Object>)(item.images[0])).Keys)
                        //{
                        //    SetToken(key);
                        //    break;
                        //}

                        //SetToken();

                        PlaylistsComponent.Children.Add(container);
                    });
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            /*_listener.Stop();
            _server.CancelAsync();*/

            if (api.status != SpotifyAPI.Status.Stopped)
                api.Stop();
        }

        private async void InitializeBrowser()
        {
            await browser.EnsureCoreWebView2Async();
        }

        private void Browser_Initialized(object sender, EventArgs e)
        {
            //(sender as WebView2).CoreWebView2.Navigate("http://localhost:3000/login");
            //OpenUrl("http://localhost:3000/login");
        }

        private void SetStatus(string status)
        {
            Dispatcher.Invoke(() => statusLabel.Content = status);
        }

        private void SetToken(Object token)
        {
            Dispatcher.Invoke(() => tokenBox.Text = token.ToString());
        }

        /*private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }*/

        private string _scopes = "user-read-email user-modify-playback-state streaming playlist-read-private playlist-read-collaborative user-library-read";
        private string _redirect_uri = "http://localhost:3000/callback";
        private string _my_client_id = "e0efe7588bb24ab4ae3f91618d2e1568";
        private string _my_client_secret = "d46590dc5e7f4fa7a852a4437990e013";

        /*private HttpListener _listener = null;
        private BackgroundWorker _server = null;
        private string _spotifyAuthToken = null;
        private string _spotifyAcessToken = null;*/
       
        /*private async void RunServer(object sender, DoWorkEventArgs e)
        {
            string[] args = new string[] { "http://localhost:3000/" };

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (args == null || args.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            _listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in args)
            {
                _listener.Prefixes.Add(s);
            }
            _listener.Start();

            while (true)
            {
                if ((sender as BackgroundWorker).CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                try
                {
                    SetStatus("Listening...");
                    HttpListenerContext context = _listener.GetContext();
                    SetStatus("Handling...");
                    HttpListenerRequest request = context.Request;
                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;

                    byte[] inputBuffer = new byte[int.Parse(request.ContentLength64.ToString())];

                    request.InputStream.Read(inputBuffer, 0, int.Parse(request.ContentLength64.ToString()));
                    string responseString = null;

                    // Construct a response.
                    switch (request.Url.LocalPath)
                    {
                        case "/callback":

                            responseString = $@"
                            <html>
                                <body>
                                    <h1>{request.HttpMethod}</h1>
                                    {request.Url.LocalPath}
                                    {request.QueryString.Count}
                                    {String.Join(',', request.Headers.AllKeys)}
                                    {System.Text.Encoding.Default.GetString(inputBuffer)}
                                    <script>window.close()</script>
                                </body>
                            </html>";

                            string token = request.QueryString["code"];
                            if (token.Length > 0)
                            {
                                //SetToken(token);
                                _spotifyAuthToken = token;

                                //var values = new Dictionary<string, string>
                                //{
                                //    { "grant_type", "authorization_code" },
                                //    { "code", _spotifyAuthToken },
                                //    { "redirect_uri", _redirect_uri }
                                //};

                                //var content = new FormUrlEncodedContent(values);
                                
                                //content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                               // content.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_my_client_id}:{_my_client_secret}"))}");

                                //HttpClient client = new HttpClient();
                                //client.DefaultRequestHeaders.Authorization = System.Net.Http.Headers.AuthenticationHeaderValue.Parse($"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_my_client_id}:{_my_client_secret}"))}");
                                //var responseS = await client.PostAsync("https://accounts.spotify.com/api/token", content);

                                //var responseString2 = await responseS.Content.ReadAsStringAsync();

                                try
                                {
                                    var resp = await "https://accounts.spotify.com/api/token".WithHeaders(
                                        new
                                        {
                                            Authorization = $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_my_client_id}:{_my_client_secret}"))}"
                                        }
                                        ).PostUrlEncodedAsync(new
                                        {
                                            grant_type = "authorization_code",
                                            code = _spotifyAuthToken,
                                            redirect_uri = _redirect_uri
                                        });

                                    _spotifyAcessToken = (await resp.GetJsonAsync<SpotifyAccessTokenResponse>()).access_token;

                                    SetToken(_spotifyAcessToken);

                                    FetchPlaylists();
                                }
                                catch (FlurlHttpException ex)
                                {
                                    var error = await ex.GetResponseJsonAsync();
                                    Console.Write($"Error returned from {ex.Call.Request.Url}: {error}");
                                }

                                //SetToken(resp);
                            }

                            break;
                        case "/login":

                            response.Redirect(
                                "https://accounts.spotify.com/authorize" +
                                "?response_type=code" +
                                $"&client_id={ _my_client_id }" +
                                (_scopes.Length > 0 ? $"&scope={Uri.EscapeUriString(_scopes)}" : "") +
                                $"&redirect_uri={Uri.EscapeDataString(_redirect_uri)}"
                                );

                            responseString = "";
                            break;
                        default:
                            responseString = "";
                            response.StatusCode = 404;
                            break;
                    }

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                    SetStatus("Responding...");
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    using (System.IO.Stream output = response.OutputStream)
                    {
                        output.Write(buffer, 0, buffer.Length);
                    }
                }
                catch (HttpListenerException)
                { }
            }
        }*/

        /*private async void FetchPlaylists()
        {
            if (_spotifyAcessToken.Length > 0)
            {
                var res = await "https://api.spotify.com/v1/me/playlists".WithHeaders(new
                {
                    Authorization = $"Bearer {_spotifyAcessToken}"
                }).GetJsonAsync();

                foreach (var item in res.items)
                {
                    this.Dispatcher.Invoke(() => { 
                        Image cover = new Image()
                        {
                            Source = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri((item.images[0].url ?? ""))),
                            Width = 100,
                            Height = 100
                        };

                        Grid container = new Grid();
                        container.Children.Add(cover);
                        container.Children.Add(new Label() { Content = item.name });

                        //foreach(string key in ((IDictionary<String, Object>)(item.images[0])).Keys)
                        //{
                        //    SetToken(key);
                        //    break;
                        //}

                        //SetToken();

                        PlaylistsComponent.Children.Add(container);
                    });
                }
            }
        }*/

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //FetchPlaylists();
        }
    }

    /*public class SpotifyAccessTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }*/
}
