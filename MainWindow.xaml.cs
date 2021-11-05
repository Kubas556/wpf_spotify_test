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

            api = api = new SpotifyAPI(_my_client_id,_my_client_secret);

            api.StatusChanged += Api_StatusChanged;
        }

        private async void Api_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            SetStatus(e.status.ToString());
            if(e.status == SpotifyAPI.Status.Ready)
            {
                UserPlaylistsResponse playlistsResp = await api.FetchPlaylists();

                foreach (SimplifiedPlaylistObject item in playlistsResp.items)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Image cover = new Image()
                        {
                            Source = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri((item.images[0].url ?? ""))),
                            Width = 100,
                            Height = 100
                        };

                        Grid container = new Grid();
                        container.DataContext = item.id;
                        container.MouseLeftButtonDown += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                        {
                            new ShowSongsWindow(api, ((Grid)sender).DataContext.ToString()).ShowDialog();
                        };
                        container.Children.Add(cover);
                        container.Children.Add(new Label() { Content = item.name });

                        PlaylistsComponent.Children.Add(container);
                    });
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {

            if (api.status != SpotifyAPI.Status.Stopped)
                api.Stop();
        }

        private void SetStatus(string status)
        {
            Dispatcher.Invoke(() => statusLabel.Content = status);
        }

        private void SetToken(Object token)
        {
            Dispatcher.Invoke(() => tokenBox.Text = token.ToString());
        }

        private string _my_client_id = "";
        private string _my_client_secret = "";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
