using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WInUiBrower.Model
{
    public partial class ServerItem : INotifyPropertyChanged
    {
        private bool _isEnable = false;
        private string _key = "";
        private string _workingDirectory = "";
        private string _executableFile = "";
        private string _args = "";
        private int _port = 0;
        private bool _delayPortDetect = false;
        private bool _waitExit = false;

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        public required string Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    OnPropertyChanged();
                }
            }
        }

        public required string WorkingDirectory
        {
            get => _workingDirectory;
            set
            {
                if (_workingDirectory != value)
                {
                    _workingDirectory = value;
                    OnPropertyChanged();
                }
            }
        }

        public required string ExecutableFile
        {
            get => _executableFile;
            set
            {
                if (_executableFile != value)
                {
                    _executableFile = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Args
        {
            get => _args;
            set
            {
                if (_args != value)
                {
                    _args = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DelayPortDetect
        {
            get => _delayPortDetect;
            set
            {
                if (_delayPortDetect != value)
                {
                    _delayPortDetect = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool WaitExit
        {
            get => _waitExit;
            set
            {
                if (_waitExit != value)
                {
                    _waitExit = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}