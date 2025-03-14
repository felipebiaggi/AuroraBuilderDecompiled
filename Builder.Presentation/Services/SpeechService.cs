using Builder.Core.Logging;
using System;
using System.Speech.Synthesis;

namespace Builder.Presentation.Services
{

    public sealed class SpeechService
    {
        private static SpeechService _instance;

        private SpeechSynthesizer _speech;

        public static SpeechService Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SpeechService();
                }
                return _instance;
            }
        }

        public event EventHandler SpeechStarted;

        public event EventHandler SpeechStopped;

        private SpeechService()
        {
            _speech = new SpeechSynthesizer();
            _speech.SpeakCompleted += _speech_SpeakCompleted;
        }

        private void _speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            StopSpeech();
        }

        public void StartSpeech(string input)
        {
            try
            {
                StopSpeech();
                _speech.SpeakAsync(input);
                OnSpeechStarted();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "StartSpeech");
                MessageDialogService.ShowException(ex);
            }
        }

        public void StopSpeech()
        {
            _speech.SpeakAsyncCancelAll();
            OnSpeechStopped();
        }

        private void OnSpeechStarted()
        {
            this.SpeechStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnSpeechStopped()
        {
            this.SpeechStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
