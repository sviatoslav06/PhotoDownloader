using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using Microsoft.Win32;
using PropertyChanged;
using System.Diagnostics;

namespace Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebClient client = new();
        ViewModel model = new ViewModel();
        string destination = null;
        public MainWindow()
        {
            InitializeComponent();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            this.DataContext = model;
        }
        private async void DownloadBtnClick(object sender, RoutedEventArgs e)
        {
            if (client.IsBusy)
            {
                MessageBox.Show("Try again later!");
                return;
            }
            string url = @$"https://source.unsplash.com/random/{model.Width}x{model.Height}/?{model.Category}&1";
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Invalid URL!");
                return;
            }
            //SaveFileDialog dialog = new SaveFileDialog();
            //if(dialog.ShowDialog() == true)
            //{
                await DownloadFileAsync(url, model.Path);
            //}
            AddHistoryItem(url);
            OpenImage(destination);
        }
        private void OpenImage(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "mspaint.exe"; // Path to Paint executable
            startInfo.Arguments = path; // Image file path in quotes
            Process.Start(startInfo);
        }
        private async Task DownloadFileAsync(string url, string path)
        {
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string name = Guid.NewGuid().ToString();
            //string ext = Path.GetExtension(url);
            string dest = Path.Combine(path, $"{name}.jpg");
            destination = dest;
            await client.DownloadFileTaskAsync(url, dest);
        }
        private void AddHistoryItem(string fileName)
        {
            historyList.Items.Add($"{DateTime.Now.ToShortTimeString()}: {fileName}");
        }
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
    }
    [AddINotifyPropertyChangedInterface]
    class ViewModel
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public string Category { get; set; }
        public string Path { get; set; }
    }
}
