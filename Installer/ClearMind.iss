#define MyAppName "ClearMind"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "ClearMind"
#define MyAppExeName "ClearMindUI.exe"
#define MyEngineExeName "ClearMindEngine.exe"
#define UIBuildDir "..\ClearMindUI\bin\PublishSingleFile"
#define EngineBuildDir "..\ClearMindScript\dist"

[Setup]
AppId={{9161B19B-35C6-4B56-BCAB-2A2BD0E6A854}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\Programs\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; Per-user install (no UAC prompt) - matches the app running entirely out of %AppData%
PrivilegesRequired=lowest
OutputDir=Output
OutputBaseFilename=ClearMindSetup
SetupIconFile=..\ClearMindUI\Images\ClearMindLogo Icon.ico
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#UIBuildDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#EngineBuildDir}\{#MyEngineExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked

[Registry]
; Starts the enforcement engine silently at every login
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}Engine"; ValueData: """{app}\{#MyEngineExeName}"""; Flags: uninsdeletevalue

[Run]
Filename: "{app}\{#MyEngineExeName}"; Description: "Start the ClearMind engine now"; Flags: nowait postinstall skipifsilent runasoriginaluser
Filename: "{app}\{#MyAppExeName}"; Description: "Launch ClearMind"; Flags: nowait postinstall skipifsilent unchecked runasoriginaluser

[UninstallRun]
Filename: "taskkill.exe"; Parameters: "/IM {#MyEngineExeName} /F"; Flags: runhidden; RunOnceId: "KillEngine"
Filename: "taskkill.exe"; Parameters: "/IM {#MyAppExeName} /F"; Flags: runhidden; RunOnceId: "KillUI"

[Code]
// Both exes must be closed before (re)installing over them, otherwise the file copy fails
// because Windows locks running executables.
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssInstall then
  begin
    Exec('taskkill.exe', '/IM {#MyEngineExeName} /F', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Exec('taskkill.exe', '/IM {#MyAppExeName} /F', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;
