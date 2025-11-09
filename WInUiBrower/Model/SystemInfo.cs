using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WInUiBrower.Enums;

namespace WInUiBrower.Model
{
    internal partial class SystemInfo : INotifyPropertyChanged
    {
        private string _appName = "";
        private Origin _originalPath = Origin.Url;
        private string _url = "http://localhost";
        private string _fetchUrl = "http://localhost";
        private bool _developerMode = false;
        private bool _disableRightClick = false;
        private bool _isForwardSelfBrower = false;

        public string AppName
        {
            get => _appName;
            set
            {
                if (_appName != value)
                {
                    _appName = value;
                    OnPropertyChanged();
                }
            }
        }

        public Origin OriginalPath
        {
            get => _originalPath;
            set
            {
                if (_originalPath != value)
                {
                    _originalPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Url
        {
            get => _url;
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FetchUrl
        {
            get => _fetchUrl;
            set
            {
                if (_fetchUrl != value)
                {
                    _fetchUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DeveloperMode
        {
            get => _developerMode;
            set
            {
                if (_developerMode != value)
                {
                    _developerMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DisableRightClick
        {
            get => _disableRightClick;
            set
            {
                if (_disableRightClick != value)
                {
                    _disableRightClick = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsForwardSelfBrower
        {
            get => _isForwardSelfBrower;
            set
            {
                if (_isForwardSelfBrower != value)
                {
                    _isForwardSelfBrower = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ServerItem> Items { get; set; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}