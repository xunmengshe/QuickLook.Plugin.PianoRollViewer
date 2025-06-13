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
            var track = (SingingTrack)project.TrackList
                .Where(trackIt => trackIt is SingingTrack)
                .ElementAt(TrackIndex);
            coordinateHelper.calculateRange(track);
            svgFactory.CoordinateHelper = coordinateHelper;
            svgFactory.ApplyStyle(
                NoteFillColor,
                NoteStrokeColor,
                NoteStrokeWidth,
                InnerTextColor,
                SideTextColor,
                NoteRound
            );
            foreach(var note in track.NoteList) {
                svgFactory.DrawNote(note);
            }
            return svgFactory;
        }
    }
}