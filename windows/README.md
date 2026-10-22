Contest DVK for Windows 10

IMPORTANT
This package is NOT a precompiled EXE. It includes C# source and a build script that
attempts to compile locally using the .NET Framework C# compiler included on many
Windows installations. I cannot truthfully describe it as a tested, ready-made EXE.

Contents
- Program.cs       Windows Forms DVK app source
- build.bat        Double-click to compile ContestDVK.exe
- audio\           Put your three WAV files here

Required audio files:
  audio\track1.wav
  audio\track2.wav
  audio\track3.wav

Steps
1. Extract the ZIP to a normal folder, e.g. C:\ContestDVK.
2. Copy your three WAV files into the audio subfolder.
3. Double-click build.bat.
4. If successful, run ContestDVK.exe. Keep the audio folder next to the EXE.

Compatibility notes
- This version is a native Windows Forms app and does not require Python or PyQt.
- Uses Windows SoundPlayer, which supports WAV audio but does not provide volume control.
  The volume slider is therefore a placeholder and does NOT change playback volume.
- Loop delay is the interval between play starts, matching the basic behavior of the
  original script; if a WAV is longer than the delay, playback may be interrupted/restarted.
- If build.bat reports that csc.exe is missing, this PC does not have the .NET Framework
  compiler at the expected path. In that case use Visual Studio Build Tools or ask for
  a revised build route.
