using SpotifyAPI_.NET_Framework;
using SpotifyAPI_.NET_Framework.SpotifyTypes;
using System;
using System.Collections.Generic;
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

namespace TestSpotifyServerAPI
{
    /// <summary>
    /// Interakční logika pro ShowSongsWindow.xaml
    /// </summary>
    public partial class ShowSongsWindow : Window
    {
        private string _playlistId;

        private SpotifyAPI _api;

        public ShowSongsWindow(SpotifyAPI api, string playlistId)
        {
            _playlistId = playlistId;

            _api = api;

            InitializeComponent();

            fetchSongs();
        }

        private async void fetchSongs() 
        {
            PlaylistTracksResponse songsResp = await _api.FetchPlaylistTracks(_playlistId);

            foreach (PlaylistTrackObject track in songsResp.items)
            {
                this.Dispatcher.Invoke(() =>
                {
                    WrapPanel panel = new WrapPanel();
                    panel.Children.Add(new Image()
                    {
                        Source = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri(track.track.album.images[0].url ?? "")),
                        Width = 100,
                        Height = 100
                    });
                    StackPanel labels = new StackPanel();
                    labels.Children.Add(new Label() { Content = track.track.name });
                    labels.Children.Add(new Label() { Content = track.track.artists[0].name });
                    panel.Children.Add(labels);

                    songsPanel.Children.Add(panel);
                });
            }
        }
    }
}
