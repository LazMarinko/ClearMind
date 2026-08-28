<div align="center">
  <img src="ClearMindUI/Images/ClearMindLogo White.png" alt="ClearMind logo" width="120" />

  # ClearMind

  A lightweight Windows app that locks distracting programs on a schedule you set.

  [![Latest release](https://img.shields.io/github/v/release/LazMarinko/ClearMind?label=download&color=6c5ce7)](https://github.com/LazMarinko/ClearMind/releases/latest/download/ClearMindSetup.exe)
  ![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11%20x64-0078D6)

  **[⬇ Download the installer](https://github.com/LazMarinko/ClearMind/releases/latest/download/ClearMindSetup.exe)**
</div>

---

## What it does

ClearMind lets you pick specific processes (games, browsers, anything with a `.exe`) and lock them behind a schedule:

- **Active days** — choose which days of the week a process is locked.
- **Time ranges** — set the hours during which the process is blocked.
- **Multiple schedules per process** — switch a process into "complex mode" to give it several day/time windows instead of just one.

While a schedule is active, ClearMind closes the matching process automatically if you try to run it — no need to keep the window open and watch it yourself.

## How it works

ClearMind is two pieces that install and run together:

| Component | Role |
|---|---|
| **ClearMind UI** (`ClearMindUI.exe`) | The window you use to add processes and set schedules. |
| **ClearMind engine** (`ClearMindEngine.exe`) | A background process that reads your schedules and enforces them, even when the UI isn't open. |

The installer registers the engine to start automatically at login, so enforcement keeps working in the background. The UI and engine talk to each other over a local socket and a small set of files in `%AppData%\ClearMind`.

## Installation

1. [Download the installer](https://github.com/LazMarinko/ClearMind/releases/latest/download/ClearMindSetup.exe).
2. Run `ClearMindSetup.exe`. It installs per-user (no admin rights needed), so it won't prompt for elevation.
3. Launch **ClearMind** from the Start Menu, add the processes you want to lock, and set their schedules.

No separate .NET or Python install is required — both components are bundled as standalone executables.

## Building from source

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download), Python 3.11+, and [Inno Setup](https://jrsoftware.org/isinfo.php) if you want to build the installer.

```bash
# Publish the UI as a self-contained single-file exe
dotnet publish ClearMindUI -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ClearMindUI/bin/PublishSingleFile

# Freeze the engine
cd ClearMindScript
python -m venv .venv
.venv\Scripts\pip install -r requirements.txt pyinstaller
.venv\Scripts\python -m PyInstaller --onefile --noconsole --name ClearMindEngine main.py
cd ..

# Build the installer
"C:\Program Files\Inno Setup 7\ISCC.exe" Installer\ClearMind.iss
```

The finished installer is written to `Installer/Output/ClearMindSetup.exe`.
