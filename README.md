# NppGZipFileViewer
A Notepad++ plugin to open and save files in the gzip format.
It's written in C# with Visual Studio.

## How to use

### Dll-Files
It depends on your npp version:
1. for x64 use <Name>X64.dll
2. for x86 use <Name>X86.dll or <Name>.dll

### Terminus:
1. tracked file: A tracked file is a file that was decompressed or is selected manually for compression. The Icon and Menu Entry will be checked for this file.
2. excluded file: A file for which the compression was manually disabled.
3. ignored file: neither tracked for compression nor excluded.
4. gz: Any file with a suffix matching one in the settings.
5. non-gz: Any other file

### Settings
This plugin has two settings.
1. 'Try to decompress all files': If set the plugin will try to decompress all files regardless of the extension. All decompressed files will be tracked and if saved automatically compressed. (If the path is still the same)
2. 'GZip-suffixes': The list of suffixes that schould automatically be decompressed. If a file is saved with such suffix it will also be compressed.

### Commands
In the menubar there are 6 Commands:
1. Toogle Compression: If clicked it will mark a compressed (tracked) file as uncompressed (excluded) and a uncompressed file as compressed (tracked). Makes the buffer dirty. It's the same command as the icon in the tool bar. It is marked if the current file is selected as compressed. If you store the file it will automatically store the file compressed.
2. Make Compressed: Compresses the current file text in the editor. This will always exclude the file from automatically compress on save. You can toogle it on by using command 1.
3. Make Uncompressed: Uncompresses the current file in the editor. This will alays exclude the file from automatically compress on save. You can toogle it on by using command 1.
4. Settings: Open the settings dialog.
5. Abaout: Open the about dialog.
6. Credits: Open the credits dialog.

### (De)Compression-File-Rules
On Open:
1. gz-suffix: It will be decompressed if possible. It will be tracked regardless.
2. no-gz-suffix: If 'Try decompress all' then the plugin tries to decompress it. <br/>If possible it will be tracked. <br/>If not or not 'Try decompress all' the file will be ignored.

Save (same path):
1. Any tracked file will be compressed.
2. Any excluded file won't be compressed.
3. Any ignored file will be compressed if the suffix matches a gz-suffix. (Won't happen since this files would be tracked)

Save (diffrent path):
Npp will tell you the old path when notifing about FileBeforeSaved, so it might be saved two time.
1. from gz to non-gz: This file won't be compressed. If you want it to be compressed: after the save, toogle the compression and save the file again.
2. from non-gz to gz: This file will always be compressed. If you don't want to compress it: after the save, toogle the compression and save the file again.
3. same suffix type:<br/>
   If tracked, save compressed.
   If excluded, save uncompressed.
   If ingored go by suffix type.
   
Copy:
1. Will always be stored as seen in the editor since npp won't raise a FileBeforeSave/FileSaved event.

## Credits
Notepad++: https://notepad-plus-plus.org/

This project uses the notepad++ plugin template from https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net.

Visual Studio: https://visualstudio.microsoft.com/de/

The Icons were created by Freepik from www.flaticon.com.
