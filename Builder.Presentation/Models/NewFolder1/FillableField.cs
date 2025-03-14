using Builder.Core;

namespace Builder.Presentation.Models.NewFolder1
{
    public class FillableField : ObservableObject
    {
        private string _originalContent;

        private string _content;

        private bool _isUserInput;

        public string OriginalContent
        {
            get
            {
                return _originalContent;
            }
            set
            {
                SetProperty(ref _originalContent, value, "OriginalContent");
                if (_content.Equals(_originalContent))
                {
                    IsUserInput = false;
                }
                OnPropertyChanged("Content");
            }
        }

        public string Content
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_content))
                {
                    return _content;
                }
                return _originalContent;
            }
            set
            {
                SetProperty(ref _content, value, "Content");
                IsUserInput = true;
                if (string.IsNullOrWhiteSpace(_content))
                {
                    IsUserInput = false;
                }
                else if (_content.Equals(_originalContent))
                {
                    IsUserInput = false;
                }
            }
        }

        public bool IsUserInput
        {
            get
            {
                return _isUserInput;
            }
            set
            {
                SetProperty(ref _isUserInput, value, "IsUserInput");
            }
        }

        public FillableField()
        {
            _originalContent = "";
            _content = "";
        }

        public void Clear(bool clearOriginalContent = false)
        {
            if (clearOriginalContent)
            {
                OriginalContent = string.Empty;
            }
            Content = string.Empty;
            IsUserInput = false;
        }

        public bool EqualsOriginalContent(string content)
        {
            return OriginalContent.Equals(content);
        }

        public void SetIfNotEqualOriginalContent(string content)
        {
            if (!EqualsOriginalContent(content))
            {
                Content = content;
            }
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
