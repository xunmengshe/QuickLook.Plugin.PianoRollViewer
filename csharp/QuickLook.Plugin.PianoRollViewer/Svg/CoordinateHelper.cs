using System;
using System.Linq;
using OpenSvip.Model;

namespace QuickLook.Plugin.PianoRollViewer.Svg
{
    public class CoordinateHelper {
        public const int TICKS_PER_BEAT = 480;
        public const int PADDING = 4;
        public int PixelPerBeat { get; set; }
        public int NoteHeight { get; set; }
        private int StartPos;
        private int EndPos;
        private int MinKey;
        private int MaxKey;
        public void calculateRange(SingingTrack track) {
            StartPos = track.NoteList[0].StartPos;
            EndPos = track.NoteList[track.NoteList.Count - 1].StartPos + track.NoteList[track.NoteList.Count - 1].Length;
            MinKey = track.NoteList.Select(n => n.KeyNumber).Min();
            MaxKey = track.NoteList.Select(n => n.KeyNumber).Max();
        }

        public void calculateRange(Project project)
        {
            StartPos = project.TrackList
                .OfType<SingingTrack>()
                .Where(tr => tr.NoteList.Count > 0)
                .Select(tr => tr.NoteList[0].StartPos)
                .Min();
            EndPos = project.TrackList
                .OfType<SingingTrack>()
                .SelectMany(tr => tr.NoteList)
                .Select(n => n.StartPos + n.Length)
                .Max();
            MinKey = project.TrackList
                .OfType<SingingTrack>()
                .SelectMany(tr => tr.NoteList)
                .Select(n => n.KeyNumber)
                .Min();
            MaxKey = project.TrackList
                .OfType<SingingTrack>()
                .SelectMany(tr => tr.NoteList)
                .Select(n => n.KeyNumber)
                .Max();
        }
        
        public NotePositionParameters GetNotePositionParameters(Note note) {
            double textX = 0;
            //TextAlign: Middle
            textX = 1.0 * (note.StartPos + 0.5 * note.Length - StartPos) * PixelPerBeat / TICKS_PER_BEAT;
            return new NotePositionParameters {
                Point1 = new Tuple<double, double>(
                    1.0 * (note.StartPos - StartPos) * PixelPerBeat / TICKS_PER_BEAT,
                    (MaxKey - note.KeyNumber) * NoteHeight
                ),
                Point2 = new Tuple<double, double>(
                    1.0 * (note.StartPos + note.Length - StartPos) * PixelPerBeat / TICKS_PER_BEAT,
                    (MaxKey - note.KeyNumber + 1) * NoteHeight
                ),
                TextSize = NoteHeight - 2 * PADDING,
                InnerText = new Tuple<double, double>(
                    textX,
                    (MaxKey - note.KeyNumber + 1) * NoteHeight - PADDING * 1.5
                ),
                UpperText = new Tuple<double, double>(
                    textX,
                    (MaxKey - note.KeyNumber) * NoteHeight - PADDING
                ),
                LowerText = new Tuple<double, double>(
                    textX,
                    (MaxKey - note.KeyNumber + 2) * NoteHeight - PADDING
                ),
            };
        }
        public Tuple<double, double> getSize() {
            return new Tuple<double, double>(
                1.0 * (EndPos - StartPos) * PixelPerBeat / TICKS_PER_BEAT,
                (MaxKey - MinKey + 1) * NoteHeight
            );
        }
        public int getFontSize() {
            return NoteHeight - 2 * PADDING;
        }
        public string getTextAnchor() {
            return "middle";
        }
    }
    public class NotePositionParameters {
        public Tuple<double, double> Point1, Point2, InnerText, UpperText, LowerText;
        public double TextSize;
    }
}