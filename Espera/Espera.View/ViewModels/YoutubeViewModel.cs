﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Espera.Core;
using Espera.Core.Library;
using Espera.View.Properties;

namespace Espera.View.ViewModels
{
    internal class YoutubeViewModel : SongSourceViewModel<YoutubeViewModel>
    {
        private SortOrder currentYoutubeSongDurationOrder;
        private SortOrder currentYoutubeSongRatingOrder;
        private SortOrder currentYoutubeSongTitleOrder;
        private bool isSearching;
        private string searchText;
        private Func<IEnumerable<YoutubeSong>, IOrderedEnumerable<YoutubeSong>> songOrderFunc;

        public YoutubeViewModel(Library library)
            : base(library)
        {
            this.searchText = String.Empty;

            // We need a default sorting order
            this.OrderByTitle();
        }

        public int DurationColumnWidth
        {
            get { return Settings.Default.YoutubeDurationColumnWidth; }
            set { Settings.Default.YoutubeDurationColumnWidth = value; }
        }

        public bool IsSearching
        {
            get { return this.isSearching; }
            private set
            {
                if (this.IsSearching != value)
                {
                    this.isSearching = value;
                    this.OnPropertyChanged(vm => vm.IsSearching);
                }
            }
        }

        public int LinkColumnWidth
        {
            get { return Settings.Default.YoutubeLinkColumnWidth; }
            set { Settings.Default.YoutubeLinkColumnWidth = value; }
        }

        public int RatingColumnWidth
        {
            get { return Settings.Default.YoutubeRatingColumnWidth; }
            set { Settings.Default.YoutubeRatingColumnWidth = value; }
        }

        public override string SearchText
        {
            get { return this.searchText; }
            set
            {
                if (this.SearchText != value)
                {
                    this.searchText = value;
                    this.OnPropertyChanged(vm => vm.SearchText);
                }
            }
        }

        public IEnumerable<SongViewModel> SelectableSongs
        {
            get
            {
                this.IsSearching = true;

                var finder = new YoutubeSongFinder(this.SearchText);
                finder.Start();

                this.IsSearching = false;

                return finder.SongsFound
                    .OrderBy(this.songOrderFunc)
                    .Select(song => new SongViewModel(song));
            }
        }

        public int TitleColumnWidth
        {
            get { return Settings.Default.YoutubeTitleColumnWidth; }
            set { Settings.Default.YoutubeTitleColumnWidth = value; }
        }

        public void OrderByDuration()
        {
            this.songOrderFunc = SortHelpers.GetOrderByDuration<YoutubeSong>(this.currentYoutubeSongDurationOrder);
            SortHelpers.InverseOrder(ref this.currentYoutubeSongDurationOrder);

            this.OnPropertyChanged(vm => vm.SelectableSongs);
        }

        public void OrderByRating()
        {
            this.songOrderFunc = SortHelpers.GetOrderByRating(this.currentYoutubeSongRatingOrder);
            SortHelpers.InverseOrder(ref this.currentYoutubeSongRatingOrder);

            this.OnPropertyChanged(vm => vm.SelectableSongs);
        }

        public void OrderByTitle()
        {
            this.songOrderFunc = SortHelpers.GetOrderByTitle<YoutubeSong>(this.currentYoutubeSongTitleOrder);
            SortHelpers.InverseOrder(ref this.currentYoutubeSongTitleOrder);

            this.OnPropertyChanged(vm => vm.SelectableSongs);
        }

        public void StartSearch()
        {
            Task.Factory.StartNew(() => this.OnPropertyChanged(vm => vm.SelectableSongs));
        }
    }
}