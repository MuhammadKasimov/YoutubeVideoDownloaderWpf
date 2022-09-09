using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VideoLibrary;

namespace YouTubeVideoDownloaderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<YouTubeVideo?> Videos;
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoResolutionCmbUI.SelectedItem is null)
            {
                MessageBox.Show("Please choose video resolution");
                return;
            }

            try
            {
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

                if (dialog.ShowDialog(this).GetValueOrDefault())
                {
                    string folderPath = dialog.SelectedPath;
                    ButtonProgressAssist.SetIsIndicatorVisible(SaveBtnUI, true);
                    SaveBtnUI.IsEnabled = false;

                    var video = Videos.First(x => x.Resolution == (int)VideoResolutionCmbUI.SelectedItem);

                    var bytes = await video.GetBytesAsync();

                    await File.WriteAllBytesAsync(Path.Combine(folderPath, video.FullName), bytes);

                    MessageBox.Show("Video has been downloaded successfuly", "Success");

                    VideoResolutionCmbUI.IsEnabled = false;

                    SaveBtnUI.IsEnabled = false;

                }
                else
                {
                    MessageBox.Show("Choose any folder");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ButtonProgressAssist.SetIsIndicatorVisible(SaveBtnUI, false);
            }
        }
        public bool IsValidYoutubeUrl(string link)
        {
            return link.StartsWith("https://youtu.be") || link.StartsWith("https://www.youtube.com");
        }

        private async void VideoLinkTbUI_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length > 10 && IsValidYoutubeUrl(textBox.Text))
            {
                var youtube = YouTube.Default;

                Videos = await youtube.GetAllVideosAsync(textBox.Text);

                VideoResolutionCmbUI.ItemsSource = Videos.Select(x => x.Resolution).Distinct().OrderByDescending(x => x);
                VideoResolutionCmbUI.IsEnabled = true;

                SaveBtnUI.IsEnabled = true;
            }
        }
    }
}
