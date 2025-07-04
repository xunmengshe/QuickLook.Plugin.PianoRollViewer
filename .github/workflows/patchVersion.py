import sys
import xmltodict

version:str = sys.argv[1]
filePath = "csharp/QuickLook.Plugin.PianoRollViewer/QuickLook.Plugin.Metadata.config"
with open(filePath, encoding="utf8") as file:
    xmlData = xmltodict.parse(file.read())
xmlData['Metadata']['Version'] = version
with open(filePath, "w", encoding="utf8") as file:
    file.write(xmltodict.unparse(xmlData))