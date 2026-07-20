# Math Animator

[![License: ISC](https://img.shields.io/badge/License-ISC-yellow.svg)](https://opensource.org/licenses/ISC)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6.svg)
![UI](https://img.shields.io/badge/UI-WPF-0C54C2.svg)

Visualize and animate mathematical functions in real time with a custom expression parser, parameter animation, zoomable graph rendering, and a built-in function library.

<div align="center">
  <strong>Function Graphs & Parametric Curves → Live Animation</strong><br>
  <em>Real-time rendering • Function library • Theme modes • Interactive controls</em>
</div>

---

## Table of Contents

- [Features](#features)
- [Quick Start](#quick-start)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Controls](#controls)
- [How It Works](#how-it-works)
- [Project Structure](#project-structure)
- [Troubleshooting](#troubleshooting)
- [Technical Notes](#technical-notes)
- [License](#license)
- [Author](#author)

---

## Features

### Function & Parametric Modes
- **Function mode:** `y = f(x)`
- **Parametric mode:** `x(t), y(t)`
- Live preview while typing formulas and parameters

### Real-Time Animation
- Time-based animation of parameters `a`, `b`, and `c`
- Smooth rendering loop (~30 FPS target)
- Animate both normal and parametric graphs

### Interactive Graph View
- Mouse wheel zoom in/out
- Reset zoom with `R`
- Toggle fullscreen with `F11`
- Auto-drawn axes, grid, and axis labels

### Built-In Function Library
- Save formulas into folders
- Rename/create/delete folders
- Move functions between folders
- Persisted in `library.json`

### Theme Support
- Neon, Dark, Classic
- Additional fun themes: White, Crazy, Eik
- Theme-aware colors for background, axes, and curve rendering

### Rich Expression Parser
Supports constants, operators, and many functions:
- Constants: `pi`, `π`, `tau`, `e`
- Variables: `x`, `t`, `a`, `b`, `c`
- Operators: `+`, `-`, `*`, `/`, `^`, parentheses
- Trig/hyperbolic: `sin`, `cos`, `tan`, `asin`, `acos`, `atan`, `atan2`, `sinh`, `cosh`, `tanh`
- Logs/exp: `ln`, `log10`, `log(base, value)`, `exp`
- Utility: `sqrt`, `cbrt`, `abs`, `round`, `floor`, `ceil`, `min`, `max`, `mod`, `random(min, max)`

---

## Quick Start

```bash
# 1) Clone
git clone https://github.com/Tschakala/MathAnimator.git
cd MathAnimator

# 2) Restore dependencies
dotnet restore

# 3) Run
dotnet run --project MathAnimator.csproj
```

---

## Prerequisites

- **OS:** Windows 10 or Windows 11
- **Runtime/SDK:** .NET 8 SDK (Windows Desktop / WPF support)

Optional:
- Visual Studio 2022 (with “Desktop development with .NET” workload)

---

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Tschakala/MathAnimator.git
   cd MathAnimator
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Build:
   ```bash
   dotnet build MathAnimator.csproj -c Release
   ```

---

## Usage

### Start the App

```bash
dotnet run --project MathAnimator.csproj
```

### Create Your Own Animation
1. Open **Eigene Funktion eingeben**
2. Choose mode:
   - **Funktionsgraph** (`y = f(x)`)
   - **Parameterdarstellung** (`x(t), y(t)`)
3. Enter formula(s)
4. Set parameters `a`, `b`, `c`
5. Click **Animieren**

### Use the Function Library
1. Open **Funktionsbibliothek**
2. Select or create folder
3. Save formulas from input view into the selected folder
4. Animate saved entries directly from the library

---

## Controls

- **Mouse Wheel**: Zoom in/out
- **R**: Reset zoom
- **F11**: Toggle fullscreen
- **Zurück**: Return to previous view

---

## How It Works

### Rendering Pipeline
1. User formula is parsed into an expression tree
2. Formula compiles to a delegate (`Func<double,double,double,double,double>`)
3. `AnimationController` updates `a`, `b`, `c` over time
4. `GraphRenderer` draws background, grid, axes, and curve into a `WriteableBitmap`
5. UI updates each frame in WPF via `CompositionTarget.Rendering`

### Storage
- Function library is stored in JSON format:
  - `library.json`
  - Folder-based organization
  - Functions include mode, formulas, and parameters

---

## Project Structure

```text
MathAnimator/
├── App.xaml
├── MainWindow.xaml
├── MathAnimator.csproj
├── library.json
├── Math/
│   └── MathParser.cs
├── Rendering/
│   ├── AnimationController.cs
│   └── GraphRenderer.cs
├── Model/
│   ├── FunctionDefinition.cs
│   ├── LibraryData.cs
│   ├── LibraryFolder.cs
│   └── ThemeSettings.cs
├── Views/
│   ├── StartView.xaml
│   ├── InputView.xaml
│   ├── LibraryView.xaml
│   ├── AnimationView.xaml
│   └── HelpWindow.xaml
└── SettingsView.xaml
```

---

## Troubleshooting

### App does not start
- Ensure .NET 8 SDK is installed:
  ```bash
  dotnet --info
  ```
- Confirm you are on Windows (WPF is Windows-only).

### Build errors in Visual Studio
- Make sure the **Desktop development with .NET** workload is installed.

### Formula errors
- Parser messages are shown in the app (preview/error area or dialog).
- Check:
  - Parentheses are balanced
  - Function names are valid
  - Decimal format uses `.` for fractional values in formulas

### Library not saving
- Verify write access in the app directory (for `library.json`).

---

## Technical Notes

- **Language:** C#
- **Framework:** .NET 8 + WPF
- **Rendering:** `WriteableBitmap` (software rendering)
- **Parser:** Custom expression parser built with `System.Linq.Expressions`
- **Persistence:** JSON
- **Target Runtime:** `win-x64` (self-contained single-file publish settings enabled in project file)

---

## License

ISC License

---

## Author

**Tschakala / Raffael**
- GitHub: [@Tschakala](https://github.com/Tschakala)
- Repository: [Tschakala/MathAnimator](https://github.com/Tschakala/MathAnimator)

---

**Note:** The application UI is primarily in German.

<div align="center">
  <strong>Made with ❤️ in Austria</strong>
</div>
