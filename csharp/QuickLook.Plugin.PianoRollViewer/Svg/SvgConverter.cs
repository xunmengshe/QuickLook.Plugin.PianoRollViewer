using OpenSvip.Framework;
using OpenSvip.Model;
using System;

namespace QuickLook.Plugin.PianoRollViewer.Svg
{
    public static class SvgConverter {

        public static SvgFactory ToSvgFactory(Project project, ConverterOptions options)
        {
            var svgFactory = new SvgEncoder
            {
                TrackIndex = options.GetValueAsInteger("TrackIndex"),
                PixelPerBeat = options.GetValueAsInteger("PixelPerBeat", 48),
                NoteHeight = options.GetValueAsInteger("NoteHeight", 24),
                NoteRound = options.GetValueAsInteger("NoteRound", 4),
                NoteFillColor = options.GetValueAsString("NoteFillColor", "#CCFFCC"),
                NoteStrokeColor = options.GetValueAsString("NoteStrokeColor", "#006600"),
                NoteStrokeWidth = options.GetValueAsInteger("NoteStrokeWidth", 1),
                InnerTextColor = options.GetValueAsString("InnerTextColor", "#000000"),
                SideTextColor = options.GetValueAsString("SideTextColor", "#000000"),
            }.Generate(project);
            return svgFactory;
        }
        public static void Save(string path, Project project, ConverterOptions options) 
        {
            ToSvgFactory(project, options).Write(path);
        }

        public static string Dumps(Project project, ConverterOptions options)
        {
            return ToSvgFactory(project, options).Dumps();
        }
    }
}