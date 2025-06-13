Remove-Item ..\QuickLook.Plugin.PianoRollViewer.qlplugin -ErrorAction SilentlyContinue

$files = Get-ChildItem -Path ..\QuickLook.Plugin.PianoRollViewer\bin\Release\ #-Exclude *.pdb,*.xml
Compress-Archive $files ..\QuickLook.Plugin.PianoRollViewer.zip
Move-Item ..\QuickLook.Plugin.PianoRollViewer.zip ..\QuickLook.Plugin.PianoRollViewer.qlplugin