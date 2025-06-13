using System;
using OpenSvip.Model;

namespace QuickLook.Plugin.PianoRollViewer.Svg
{
    public class CoordinateHelper {
        public const int TICKS_PER_BEAT = 480;
        public const int PADDING = 4;
        public int PixelPerBeat { get; set; }
        public int NoteHeight { get; set; }
        private int PositionRangeStart;
        private int PositionRangeEnd;
        private int KeyRangeStart;
        private int KeyRangeEnd;
        public void calculateRange(SingingTrack track) {
            PositionRangeStart = track.NoteList[0].StartPos;
            PositionRangeEnd = track.NoteList[track.NoteList.Count - 1].StartPos + track.NoteList[track.NoteList.Count - 1].Length;
            KeyRangeStart = int.MaxValue;
            KeyRangeEnd = int.MinValue;
            foreach(var note in track.NoteList) {
                KeyRangeStart = Math.Min(KeyRangeStart, note.KeyNumber);
                KeyRangeEnd = Math.Max(KeyRangeEnd, note.KeyNumber);
            }
        }
        
        public NotePositionParameters GetNotePositionParameters(Note note) {
            double textX = 0;
            //TextAlign: Middle
            textX = 1.0 * (note.StartPos + 0.5 * note.Length - PositionRangeStart) * PixelPerBeat / TICKS_PER_BEAT;
            return new NotePositionParameters {
                Point1 = new Tuple<double, double>(
                    1.0 * (note.StartPos - PositionRangeStart) * PixelPerBeat / TICKS_PER_BEAT,
                    (KeyRangeEnd - note.KeyNumber) * NoteHeight
                ),
                Point2 = new Tuple<double, double>(
                    1.0 * (note.StartPos + note.Length - PositionRangeStart) * PixelPerBeat / TICKS_PER_BEAT,
                    (KeyRangeEnd - note.KeyNumber + 1) * NoteHeight
                ),
                TextSize = NoteHeight - 2 * PADDING,
                InnerText = new Tuple<double, double>(
                    textX,
                    (KeyRangeEnd - note.KeyNumber + 1) * NoteHeight - PADDING * 1.5
                ),
                UpperText = new Tuple<double, double>(
                    textX,
                    (KeyRangeEnd - note.KeyNumber) * NoteHeight - PADDING
                ),
                LowerText = new Tuple<double, double>(
                    textX,
                    (KeyRangeEnd - note.KeyNumber + 2) * NoteHeight - PADDING
                ),
            };
        }
        public Tuple<double, double> getSize() {
            return new Tuple<double, double>(
                1.0 * (PositionRangeEnd - PositionRangeStart) * PixelPerBeat / TICKS_PER_BEAT,
                (KeyRangeEnd - KeyRangeStart + 1) * NoteHeight
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