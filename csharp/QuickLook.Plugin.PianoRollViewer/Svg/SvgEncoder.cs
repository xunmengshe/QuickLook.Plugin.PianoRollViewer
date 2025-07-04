using System.Linq;
using OpenSvip.Model;

namespace QuickLook.Plugin.PianoRollViewer.Svg {
    public class SvgEncoder {
        public int TrackIndex { get; set; }
        public int PixelPerBeat { get; set; }
        public int NoteHeight { get; set; }
        public int NoteRound { get; set; }
        public string NoteFillColor { get; set; }
        public string NoteStrokeColor { get; set; }
        public int NoteStrokeWidth { get; set; }
        public string InnerTextColor { get; set; }
        public string SideTextColor { get; set; }
        public SvgFactory Generate(Project project) {
            var svgFactory = new SvgFactory();
            var coordinateHelper = new CoordinateHelper {
                PixelPerBeat = PixelPerBeat,
                NoteHeight = NoteHeight,
            };
            coordinateHelper.calculateRange(project);
            svgFactory.CoordinateHelper = coordinateHelper;
            svgFactory.ApplyStyle(
                NoteFillColor,
                NoteStrokeColor,
                NoteStrokeWidth,
                InnerTextColor,
                SideTextColor,
                NoteRound,
                project.TrackList.Count
            );
            foreach (var track in project.TrackList.OfType<SingingTrack>()) { 
                svgFactory.DrawTrack(track);
            }
            return svgFactory;
        }
    }
}