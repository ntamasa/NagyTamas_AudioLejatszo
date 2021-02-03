using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioLejatszo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<string> _playlist = new BindingList<string>();
        MediaPlayer _mediaPlayer = new MediaPlayer();
        DispatcherTimer _mediaPlayerTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            lista.ItemsSource = _playlist;
            _mediaPlayer.MediaOpened += _mediaPlayer_MediaOpened;
            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;
        }

        private void _mediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                timeSlider.Maximum = _mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                _mediaPlayerTimer.Interval = TimeSpan.FromMilliseconds(200);
                _mediaPlayerTimer.Tick += UpdateTime;
                _mediaPlayerTimer.Start();
            }
        }

        void UpdateTime(object sender, EventArgs e)
        {
            timeSlider.Value = _mediaPlayer.Position.TotalMilliseconds;
        }



        private void _mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (lista.SelectedIndex == -1) return;

            lista.SelectedIndex++;
            if (lista.SelectedIndex != lista.Items.Count-1)
            {
                PlayCurrentSong();
            }
            else
            {
                Pause();
                lita.SelectedIndex = -1;
                _mediaPlayer.Stop();
            }
        }

        private void gomb_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Audio Files (*.mp3; *.flac; *.wav) |*.mp3;*.flac;*.wav"
            }; 
            if ((bool)fileDialog.ShowDialog())
            {
                fileDialog.FileNames.ToList().ForEach(x => 
                { 
                    _playlist.Add(x);
                });
                lista.SelectedIndex = 0;
                
            }
        }

        private void PlayStop_Click(object sender, RoutedEventArgs e)
        {
            if ((string)PlayStop.Content == "Play" && _playlist.Count != 0)
            {
                Play();
            }
            else if ((string)PlayStop.Content == "Pause")
            {
                Pause();
            }
        }

        private void lista_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lista.SelectedIndex == -1) return;
            try
            {
                PlayCurrentSong();
            }
            catch(Exception)
            {
                MessageBox.Show("Nem lehet lejátszani a kiválasztott fájlt!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        void Pause()
        {
            PlayStop.Content = "Play";
            _mediaPlayer.Pause();
        }

        void PlayCurrentSong()
        {
            _mediaPlayer.Open(new Uri((string)lista.SelectedItem));
            Play();
        }
        void Play()
        {
            if (lista.SelectedIndex == -1) return;

            PlayStop.Content = "Pause";
            _mediaPlayer.Play();
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Pause();
            lista.SelectedIndex = -1;
            _mediaPlayer.Stop();
        }

        
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (lista.SelectedIndex != lista.Items.Count-1)
                lista.SelectedIndex++;
            PlayCurrentSong();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Volume = e.NewValue;
        }
        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (lista.SelectedIndex != 0)
                lista.SelectedIndex--;
            PlayCurrentSong();
        }
        private void timeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Position = TimeSpan.FromMilliseconds(e.NewValue);
        }
    }
}
