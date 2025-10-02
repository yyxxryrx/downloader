using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Downloader.Modules.Models;

namespace Downloader.Modules.ViewModels
{
    public partial class Downloader : ObservableObject
    {
        [ObservableProperty] private int _speed;
        [ObservableProperty] private double _progress = -1;
        public ObservableCollection<ChunkDownloader> Chunks = [];
        public string Path { get; set; }
        [ObservableProperty] private DownloadStatue _statue;
        public string Url { get; set; }
        public string FileName { get; set; }

        public Downloader(string url, string fileName)
        {
            Url = url;
            FileName = fileName;
        }

        private async void GetInfo()
        {
        }

        public async void Init()
        {
        }
    }
}