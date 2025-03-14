using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;
using Builder.Core;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Presentation.Controls;
using Builder.Presentation.Extensions;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Telemetry;
using Microsoft.Win32;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.WPF;

namespace Builder.Presentation.Controls
{
    [TemplatePart(Name = "PART_HtmlPanel", Type = typeof(HtmlPanel))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_SnapShotButton", Type = typeof(ScrollViewer))]
    public class ElementDescriptionPanel : Control
    {
        private ScrollViewer _scrollViewer;

        private HtmlPanel _panel;

        private Image _image;

        private Button _snapButton;

        public static readonly DependencyProperty ElementProperty;

        public static readonly DependencyProperty DescriptionProperty;

        public static readonly DependencyProperty StyleSheetProperty;

        public static readonly DependencyProperty StartAudioCommandProperty;

        public static readonly DependencyProperty StopAudioCommandProperty;

        public static readonly DependencyProperty StartAudioVisibleProperty;

        public static readonly DependencyProperty StopAudioVisibleProperty;

        public static readonly DependencyProperty SnapShotCommandProperty;

        public static readonly DependencyProperty SelectedDescriptionTextProperty;

        [Category("Aurora")]
        public ElementBase Element
        {
            get
            {
                return (ElementBase)GetValue(ElementProperty);
            }
            set
            {
                SetValue(ElementProperty, value);
            }
        }

        [Category("Aurora")]
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
                _scrollViewer.ScrollToHome();
            }
        }

        [Category("Aurora")]
        public string StyleSheet
        {
            get
            {
                return (string)GetValue(StyleSheetProperty);
            }
            set
            {
                SetValue(StyleSheetProperty, value);
            }
        }

        [Category("Aurora")]
        public ICommand StartAudioCommand
        {
            get
            {
                return (ICommand)GetValue(StartAudioCommandProperty);
            }
            set
            {
                SetValue(StartAudioCommandProperty, value);
            }
        }

        [Category("Aurora")]
        public ICommand StopAudioCommand
        {
            get
            {
                return (ICommand)GetValue(StopAudioCommandProperty);
            }
            set
            {
                SetValue(StopAudioCommandProperty, value);
            }
        }

        [Category("Aurora")]
        public Visibility StartAudioVisible
        {
            get
            {
                return (Visibility)GetValue(StartAudioVisibleProperty);
            }
            set
            {
                SetValue(StartAudioVisibleProperty, value);
            }
        }

        [Category("Aurora")]
        public Visibility StopAudioVisible
        {
            get
            {
                return (Visibility)GetValue(StopAudioVisibleProperty);
            }
            set
            {
                SetValue(StopAudioVisibleProperty, value);
            }
        }

        [Category("Aurora")]
        public ICommand SnapShotCommand
        {
            get
            {
                return (ICommand)GetValue(SnapShotCommandProperty);
            }
            set
            {
                SetValue(SnapShotCommandProperty, value);
            }
        }

        [Category("Aurora")]
        public string SelectedDescriptionText
        {
            get
            {
                return _panel.SelectedText;
            }
            set
            {
                SetValue(SelectedDescriptionTextProperty, value);
            }
        }

        static ElementDescriptionPanel()
        {
            ElementProperty = DependencyProperty.Register("Element", typeof(ElementBase), typeof(ElementDescriptionPanel), new PropertyMetadata((object)null));
            DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ElementDescriptionPanel), new PropertyMetadata((object)null));
            StyleSheetProperty = DependencyProperty.Register("StyleSheet", typeof(string), typeof(ElementDescriptionPanel), new PropertyMetadata((object)null));
            StartAudioCommandProperty = DependencyProperty.Register("StartAudioCommand", typeof(ICommand), typeof(ElementDescriptionPanel), new PropertyMetadata((object)null));
            StopAudioCommandProperty = DependencyProperty.Register("StopAudioCommand", typeof(ICommand), typeof(ElementDescriptionPanel), new PropertyMetadata((object)null));
            StartAudioVisibleProperty = DependencyProperty.Register("StartAudioVisible", typeof(Visibility), typeof(ElementDescriptionPanel), new PropertyMetadata(Visibility.Visible));
            StopAudioVisibleProperty = DependencyProperty.Register("StopAudioVisible", typeof(Visibility), typeof(ElementDescriptionPanel), new PropertyMetadata(Visibility.Visible));
            SnapShotCommandProperty = DependencyProperty.Register("SnapShotCommand", typeof(ICommand), typeof(ElementDescriptionPanel), new PropertyMetadata((object)null));
            SelectedDescriptionTextProperty = DependencyProperty.Register("SelectedDescriptionText", typeof(string), typeof(ElementDescriptionPanel), new PropertyMetadata((object)null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ElementDescriptionPanel), new FrameworkPropertyMetadata(typeof(ElementDescriptionPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollViewer = base.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;
            _panel = base.Template.FindName("PART_HtmlPanel", this) as HtmlPanel;
            _image = base.Template.FindName("PART_Image", this) as Image;
            _snapButton = base.Template.FindName("PART_SnapShotButton", this) as Button;
            _snapButton.Click += _snapButton_Click;
            if (StartAudioCommand == null)
            {
                StartAudioCommand = new RelayCommand(StartSpeech);
            }
            if (StopAudioCommand == null)
            {
                StopAudioCommand = new RelayCommand(StopSpeech);
            }
        }

        private void DisplayLinkedDescription(HtmlLinkClickedEventArgs args)
        {
            try
            {
                if (args.Link.Contains("ID_"))
                {
                    string id = args.Attributes["href"].Trim('#').Trim();
                    ElementBase element = DataManager.Current.ElementsCollection.GetElement(id);
                    if (element != null)
                    {
                        Element = element;
                        Description = (element.HasGeneratedDescription ? element.GeneratedDescription : element.Description);
                        args.Handled = true;
                    }
                    MessageBox.Show("Hello you!");
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DisplayLinkedDescription");
            }
        }

        private void _panel_LinkClicked(object sender, RoutedEvenArgs<HtmlLinkClickedEventArgs> args)
        {
        }

        private void _snapButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateImage();
        }

        private void StartSpeech()
        {
            try
            {
                if (SelectedDescriptionText.Length > 0)
                {
                    SpeechService.Default.StartSpeech(SelectedDescriptionText);
                    return;
                }
                string text = Description.Replace("</h1>", "</h1>__ENTER__");
                text = text.Replace("</h2>", "</h2>__ENTER__");
                text = text.Replace("</h3>", "</h3>__ENTER__");
                text = text.Replace("</h4>", "</h4>__ENTER__");
                text = text.Replace("</h5>", "</h5>__ENTER__");
                text = text.Replace("</h6>", "</h6>__ENTER__");
                text = text.Replace("<br/>", "<br/>__ENTER__");
                text = text.Replace("</p>", "</p>__ENTER__");
                text = text.Replace("d10", "d 10").Replace("d12", "d 12").Replace("d20", "d 20")
                    .Replace("—", " , ")
                    .Replace(" cp ", " copper pieces ")
                    .Replace(" cp)", " copper pieces)")
                    .Replace(" cp.", " copper pieces.")
                    .Replace(" sp ", " silver pieces ")
                    .Replace(" sp)", " silver pieces)")
                    .Replace(" sp.", " silver pieces.")
                    .Replace(" ep ", " electrum pieces ")
                    .Replace(" ep)", " electrum pieces)")
                    .Replace(" ep.", " electrum pieces.")
                    .Replace(" gp ", " gold pieces ")
                    .Replace(" gp)", " gold pieces)")
                    .Replace(" gp.", " gold pieces.")
                    .Replace(" pp ", " platinum pieces ")
                    .Replace(" pp)", " platinum pieces)")
                    .Replace(" pp.", " platinum pieces.");
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                string input = xmlDocument.InnerText.Replace("__ENTER__", Environment.NewLine);
                SpeechService.Default.StartSpeech(input);
                if (Element != null)
                {
                    AnalyticsEventHelper.DescriptionPanelReadAloud(Element.Name);
                }
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowException(ex);
            }
        }

        private void StopSpeech()
        {
            SpeechService.Default.StopSpeech();
        }

        public void GenerateImage()
        {
            try
            {
                string fileName = ((Element == null) ? "snap" : Element.Name.ToSafeFilename());
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Image (*.png)|*.png",
                    FileName = fileName,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    AddExtension = true,
                    DefaultExt = "png",
                    Title = "Save Snapshot"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    BitmapFrame item = HtmlRender.RenderToImage(_panel.GetHtml(), (int)_panel.ActualWidth);
                    PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
                    pngBitmapEncoder.Frames.Add(item);
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        pngBitmapEncoder.Save(stream);
                    }
                    Process.Start(saveFileDialog.FileName);
                    if (Element != null)
                    {
                        AnalyticsEventHelper.DescriptionPanelSnap(Element.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GenerateImage");
                MessageDialogService.ShowException(ex);
            }
        }
    }

}
