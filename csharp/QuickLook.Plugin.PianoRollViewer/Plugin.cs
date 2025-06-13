using FlutyDeer.MidiPlugin.Stream;
using OpenSvip.Framework;
using OpenSvip.Model;
using OxygenDioxide.UstxPlugin.Stream;
using QuickLook.Common.Helpers;
using QuickLook.Common.Plugin;
using QuickLook.Plugin.PianoRollViewer.Svg;
using SkiaSharp;
using Svg.Skia;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace QuickLook.Plugin.PianoRollViewer
{
    public class Plugin : IViewer
    {
        public int Priority => 0;

        private Dictionary<string, IProjectConverter> Converters;

        public Plugin()
        {
            Converters = new Dictionary<string, IProjectConverter>()
            {
                {".mid", new MidiConverter() },
                {".midi", new MidiConverter() },
                {".ustx", new UstxConverter() },
            };
        }

        public void Init()
        {
        }

        public bool CanHandle(string path)
        {
            return Converters.ContainsKey(Path.GetExtension(path).ToLower());
        }

        public void Prepare(string path, ContextObject context)
        {
            context.PreferredSize = new System.Windows.Size(800, 600);
            context.Theme = (Themes)SettingHelper.Get("LastTheme", 1, "QuickLook.Plugin.PianoRollViewer");
        }
        
        BitmapSource SvgRender(string svg)
        {
            using var sksvg = new SKSvg();
            if (sksvg.FromSvg(svg) is SKPicture picture) {
                using var ms = new MemoryStream();
                picture.ToImage(ms, SKColors.Empty, SKEncodedImageFormat.Png, 100, 1f, 1f, SKColorType.Rgba8888, SKAlphaType.Unpremul, null);
                return BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            return null;
        }

        /// <summary>
        /// In-place project preparison for svg output 
        /// </summary>
        /// <param name="project"></param>
        public void PreProcess(Project project)
        {
            //Remove leading spaces in project
            int startPos = project.TrackList
                .OfType<SingingTrack>()
                .Where(t => t.NoteList.Count > 0)
                .Select(t => t.NoteList[0].StartPos)
                .Min();
            foreach(var note in project.TrackList
                .OfType<SingingTrack>()
                .SelectMany(t => t.NoteList))
            {
                note.StartPos -= startPos;
            }
            //Only show notes in the first 57600 ticks
            foreach(var track in project.TrackList
                .OfType<SingingTrack>())
            {
                track.NoteList = track.NoteList
                    .Where(n => n.StartPos + n.Length < 57600)
                    .ToList();
            }
        }

        public void View(string path, ContextObject context)
        {
            string ext = Path.GetExtension(path);
            var converter = Converters[ext];
            var project = converter.Load(path,
                new ConverterOptions(
                    new Dictionary<string, string>()
                )
            );
            PreProcess(project);
            string svg = SvgConverter.Dumps(project,
                new ConverterOptions(
                    new Dictionary<string, string>()
                )
            );
            ImagePanel imagePanel = new ImagePanel();
            context.ViewerContent = imagePanel;
            context.Title = "{Path.GetFileName(path)}";
            imagePanel.BackgroundVisibility = Visibility.Hidden;
            imagePanel.Source = SvgRender(svg);
            //imagePanel.DoZoomToFit();
            imagePanel.ResetZoom();
            context.IsBusy = false;
            context.ViewerContent = imagePanel;
            context.Title = $"{Path.GetFileName(path)}";
        }

        public void Cleanup()
        {
            
        }
    }
}
