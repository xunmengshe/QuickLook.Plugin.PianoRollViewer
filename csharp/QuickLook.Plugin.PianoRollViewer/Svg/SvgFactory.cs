using OpenSvip.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace QuickLook.Plugin.PianoRollViewer.Svg {
    public class SvgFactory {
        private List<Rectangle> RectangleElements = new List<Rectangle>();
        private List<PolyLine> PolyLineElements = new List<PolyLine>();
        private List<Text> TextElements = new List<Text>();
        private string Style;
        public CoordinateHelper CoordinateHelper;
        private int NoteRound;
        public void ApplyStyle(
            string noteFillColor,
            string noteStrokeColor,
            int noteStrokeWidth,
            string innerTextColor,
            string sideTextColor,
            int noteRound
        ) {
            this.NoteRound = noteRound;
            this.Style =  
$@".note {{
    fill: {noteFillColor};
    stroke: {noteStrokeColor};
    stroke-width: {noteStrokeWidth}px;
}}
text {{
    font-size: {CoordinateHelper.getFontSize()}px;
    text-anchor: {CoordinateHelper.getTextAnchor()};
}}
.inner {{
    fill: {innerTextColor};
}}
.side {{
    fill: {sideTextColor};
    font-size: {Math.Max(CoordinateHelper.getFontSize() - 4, 10)}px;
}}";
        }
        private void DrawText(string text, NotePositionParameters parameters, bool pinyin) {
            TextElements.Add(new Text
            {
                X = parameters.InnerText.Item1,
                Y = parameters.InnerText.Item2,
                Pinyin = pinyin,
                Content = text,
            });
        }
        public void DrawNote(Note note) {
            var parameters = CoordinateHelper.GetNotePositionParameters(note);
            RectangleElements.Add(new Rectangle {
                X = parameters.Point1.Item1,
                Y = parameters.Point1.Item2,
                Width= parameters.Point2.Item1 - parameters.Point1.Item1,
                Height = parameters.Point2.Item2 - parameters.Point1.Item2,
                R = NoteRound,
            });
            DrawText(note.Lyric, parameters, false);
        }

        public XmlDocument ToXmlDocument()
        {
            var svgFile = new XmlDocument();
            var root = svgFile.CreateElement("svg", "http://www.w3.org/2000/svg");
            root.SetAttribute("width", CoordinateHelper.getSize().Item1.ToString());
            root.SetAttribute("height", CoordinateHelper.getSize().Item2.ToString());
            svgFile.AppendChild(root);
            var style = svgFile.CreateElement("style", svgFile.DocumentElement.NamespaceURI);
            style.InnerText = Style;
            root.AppendChild(style);
            foreach (var rectangleElement in RectangleElements)
            {
                var rectangle = svgFile.CreateElement("rect", svgFile.DocumentElement.NamespaceURI);
                rectangle.SetAttribute("class", "note");
                rectangle.SetAttribute("x", rectangleElement.X.ToString());
                rectangle.SetAttribute("y", rectangleElement.Y.ToString());
                rectangle.SetAttribute("width", rectangleElement.Width.ToString());
                rectangle.SetAttribute("height", rectangleElement.Height.ToString());
                rectangle.SetAttribute("rx", rectangleElement.R.ToString());
                rectangle.SetAttribute("ry", rectangleElement.R.ToString());
                root.AppendChild(rectangle);
            }
            foreach (var polyLineElement in PolyLineElements)
            {
                var polyLine = svgFile.CreateElement("polyline", svgFile.DocumentElement.NamespaceURI);
                polyLine.SetAttribute("class", "pitch");
                polyLine.SetAttribute("points", String.Join(" ", polyLineElement.points.ConvertAll(delegate (Tuple<double, double> point) {
                    return point.Item1 + "," + point.Item2;
                })));
                root.AppendChild(polyLine);
            }
            foreach (var textElement in TextElements)
            {
                var text = svgFile.CreateElement("text", svgFile.DocumentElement.NamespaceURI);
                var classNames = "inner";
                if (textElement.Pinyin) classNames += " pinyin";
                text.SetAttribute("class", classNames);
                text.SetAttribute("x", textElement.X.ToString());
                text.SetAttribute("y", textElement.Y.ToString());
                text.InnerText = textElement.Content;
                root.AppendChild(text);
            }
            return svgFile;
        }

        public string Dumps()
        {
            StringWriter stringWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(stringWriter))
            {
                ToXmlDocument().WriteTo(writer);
                writer.Flush();
            }
            return stringWriter.ToString();
        }

        public void Write(string path) {
            using(XmlWriter writer = XmlWriter.Create(path)) {
                ToXmlDocument().WriteTo(writer);
                writer.Flush();
            }
        }
    }
}