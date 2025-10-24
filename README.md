# 🎮 Gredelos — Mobile Unity Game

[Baca dalam Bahasa Indonesia](#bahasa-indonesia) | [Read in English](#english)

---

## POIN-POIN MATERI

* 📜 [Deskripsi Singkat](#deskripsi-singkat)
* 🧰 [Struktur Proyek](#struktur-proyek)
* ⚙️ [Teknologi & Arsitektur](#teknologi--arsitektur)
* 📆 [Komponen Inti](#komponen-inti)
* ⚖️ [Cara Instalasi](#cara-instalasi)

---

<a name="bahasa-indonesia"></a>

## 🇮🇩 Bahasa Indonesia

<a name="deskripsi-singkat"></a>

### 📜 Deskripsi Singkat

**Gredelos (mobile-game-gredelos)** adalah proyek game mobile edukasi berbasis Unity. Project ini berfokus pada kumpulan level mekanisme permainan (drag, swipe, click) dengan audio VO, UI interaktif, dan manajemen progress pemain. Repositori berisi assets, scene, script gameplay, dan build contoh (termasuk output iOS/IL2CPP).

Fitur utama yang ditemukan:
* Mekanisme gameplay berbasis drag & swipe (beberapa varian: makan, mandi, bersih, dsb.)
* Sistem progress / data model (Player, Progress, CompletePlay)
* UI layar (menu level, pilih karakter, splash screen)
* Audio VO & affirmations untuk panduan level
* Build iOS (IL2CPP) di `Build/ios/`

---

<a name="struktur-proyek"></a>

### 🧰 Struktur Proyek (ringkasan dari file yang ditemukan)

```
📁 Assets/gredelos/             → Game-specific assets and code
  ├─ Audio/                    → VO and affirmations audio files
  ├─ Scenes/                   → Scenes (e.g., SplashScreen.unity, PilihKarakter.unity)
  ├─ Scripts/                  → C# game code organized by subfolders
  │   ├─ GameLogic/            → Level controllers & gameplay mechanics
  │   ├─ Managers/             → Menu, splash, level managers
  │   ├─ UI/                   → UI controllers and buttons
  │   ├─ Animation/            → small animation helpers (PointerAnimation, IdleScript)
  │   ├─ Data Model/           → Player, Progress, Level, CompletePlay, DbRoot
  │   └─ Data Controller/      → Level data loader, UpToDatabase
  └─ ...
📁 Assets/Editor/               → Editor tools (MenuLevelManagerEditor, NoteDrawer)
📁 Build/                      → Build artifacts (includes `Build/ios/Il2CppOutputProject`)
📁 ProjectSettings/            → Unity project settings (Unity version, input, graphics)
```

---

<a name="teknologi--arsitektur"></a>

### ⚙️ Teknologi & Arsitektur

* **Engine:** Unity (lihat `ProjectSettings/ProjectVersion.txt`, repo uses editor version `m_EditorVersion: 6000.1.9f1` — match this in Unity Hub)
* **Bahasa:** C# (mono/IL2CPP for native builds)
* **Input:** Unity Input System (project includes `Assets/InputSystem_Actions.inputactions`)
* **Rendering:** Universal Render Pipeline (URP present in `Packages/manifest.json`)
* **Visual Scripting:** `com.unity.visualscripting` package is included (check `Assets/` for graphs)
* **Testing:** `com.unity.test-framework` is listed in packages
* **Build target example:** iOS with IL2CPP (see `Build/ios/`)

Arsitektur singkat:
- Modular scene-driven levels. Gameplay logic lives under `Assets/gredelos/Scripts/GameLogic/` with per-level controllers: `ControllerPlayObjekLevel1..5.cs`.
- UI and menus are in `Assets/gredelos/Scripts/UI/` and `Managers/` contains higher-level screen flow (e.g., `MainMenuManager.cs`, `MenuLevelManager.cs`, `PilihKarakterManager.cs`).
- Data model classes under `Scripts/Data Model/` hold persistent player and progress data (e.g., `Player.cs`, `Progress.cs`, `CompletePlay.cs`, `DbRoot.cs`).

---

<a name="komponen-inti"></a>

### 📆 Komponen Inti (file contoh)

* **Bootstrap & Flow:** `GameBootstrap.cs`, `SplashScreenManager.cs`, `MainMenuManager.cs`
* **Manajemen Level & Play:** `ManagerPlayLevel.cs`, `MenuLevelManager.cs`, `PilihKarakterManager.cs`
* **Level Controllers / Gameplay:** `ControllerPlayObjekLevel1.cs` .. `ControllerPlayObjekLevel5.cs`, `LevelHandController.cs`
* **Mekanisme Gameplay:** `DragObject.cs`, `DragSendok.cs`, `DragMandiAir.cs`, `DragAndClean.cs`, `ClickSwipe.cs`, `OnSwipeAll.cs`
* **UI:** `UIController.cs`, `LevelItemController.cs`, `ButtonController.cs`, `ButtonAudio.cs`
* **Audio:** `ManagerAudio.cs` (handles VO/affirmations)
* **Data model & persistence:** `Player.cs`, `Progress.cs`, `CompletePlay.cs`, `DbRoot.cs`, `LevelDataController.cs`, `UpToDatabase.cs`

---

<a name="cara-instalasi"></a>

### ⚖️ Cara Instalasi

1. **Clone repositori:**

```powershell
git clone https://github.com/Iman874/mobile-game-gredelos.git
cd game-mobile-gredelos
```

2. **Buka di Unity Hub**

* Pilih "Open Project" dan arahkan ke folder `game-mobile-gredelos` hasil clone
* Pastikan Unity Editor yang dipilih cocok dengan `ProjectSettings/ProjectVersion.txt` (editor: `6000.1.9f1`).

3. **Jalankan (Play) / Build:**

* Buka scene awal: `Assets/gredelos/Scenes/SplashScreen.unity` atau `Assets/gredelos/Scenes/PilihKarakter.unity` dan tekan Play.
* Untuk build iOS (contoh, IL2CPP) gunakan Build Settings di Editor. Jika automasi CI/Headless diperlukan, buat `BuildScript.PerformBuild` atau gunakan Unity in batchmode (contoh PowerShell):

```powershell
& "C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe" -batchmode -projectPath "d:\Iman874\Documents\Github\mobile-game-gredelos\game-mobile-gredelos" -quit -buildTarget iOS -executeMethod BuildScript.PerformBuild -logFile "build.log"
```

4. **Hal yang perlu diperhatikan**

* Cocokkan versi Unity Editor dengan `ProjectVersion.txt` untuk menghindari masalah serialisasi asset.
* Project memakai Unity Input System — jika Anda mengubah `Assets/InputSystem_Actions.inputactions` dan ada wrapper C# yang di-generate, perbarui wrappernya atau regenarate di Editor.
* Banyak assets audio (VO) disertakan — pastikan import settings audio sesuai platform target (compressed/PCM).

---

<a name="english"></a>

## 🇬🇧 English

<a name="deskripsi-singkat"></a>

### 📜 Overview

**Gredelos (mobile-game-gredelos)** is a Unity-based mobile educational game project. The repository contains scenes, gameplay scripts (drag/swipe mechanics), audio VO, UI logic, and example build outputs (including iOS/IL2CPP).

Key features discovered:
* Drag & swipe based mini-games (many variants per level)
* Player progress & data model
* Menu flows and character selection
* VO audio guidance and affirmation clips
* iOS IL2CPP build present under `Build/ios/`

---

<a name="struktur-proyek"></a>

### 🧰 Project Structure

```
📁 Assets/gredelos/             → Game assets and code
  ├─ Audio/                    → VO and affirmations audio
  ├─ Scenes/                   → Scenes (Splash, PilihKarakter, etc.)
  ├─ Scripts/                  → C# organized into GameLogic, Managers, UI, Data Model
  └─ ...
📁 Assets/Editor/               → Editor helper scripts
📁 Build/                      → Build outputs (e.g., Build/ios/Il2CppOutputProject)
📁 ProjectSettings/            → Unity project settings (version, input, graphics)
```

---

<a name="teknologi--arsitektur"></a>

### ⚙️ Tech Stack & Architecture

* **Engine:** Unity (project indicates editor `6000.1.9f1` in `ProjectSettings/ProjectVersion.txt`)
* **Language:** C#
* **Input:** Unity Input System (`Assets/InputSystem_Actions.inputactions`)
* **Rendering:** Universal Render Pipeline (URP)
* **Visual Scripting:** present in packages (may have graphs in `Assets/`)
* **Build:** Example iOS/IL2CPP output included

Architecture notes:
- Level controllers (per-level) drive gameplay and use small modular gameplay scripts under `GameLogic/MekanismeGameplay`.
- Managers coordinate UI and flow (`Managers/`), while data classes under `Data Model/` store player/progress state.

---

<a name="komponen-inti"></a>

### 📆 Core Components (example files)

* **Bootstrap & Flow:** `GameBootstrap.cs`, `SplashScreenManager.cs`, `MainMenuManager.cs`
* **Level & Gameplay:** `ManagerPlayLevel.cs`, `ControllerPlayObjekLevel1..5.cs`, `LevelHandController.cs`
* **Mechanics:** `DragObject.cs`, `ClickSwipe.cs`, `OnSwipeAll.cs`, `DragSendok.cs`, `DragMandiAir.cs`
* **UI & Audio:** `UIController.cs`, `LevelItemController.cs`, `ManagerAudio.cs`
* **Data & Persistence:** `Player.cs`, `Progress.cs`, `CompletePlay.cs`, `DbRoot.cs`, `LevelDataController.cs`

---

<a name="cara-instalasi"></a>

### ⚖️ Setup and Installation

1. **Clone the repository:**

```bash
git clone https://github.com/Iman874/mobile-game-gredelos.git
cd game-mobile-gredelos
```

2. **Open in Unity Hub**

* Select "Open Project" and point to the cloned folder
* Use a Unity Editor matching `ProjectSettings/ProjectVersion.txt` (editor `6000.1.9f1`)

3. **Run / Build**

* Open `Assets/gredelos/Scenes/SplashScreen.unity` (or `PilihKarakter.unity`) and press Play.
* Use Build Settings to build to your platform. For automated builds, implement a small `BuildScript` and run Unity in batchmode.

4. **Notes**

* Keep Unity version & package versions aligned to avoid reserialization issues.
* If you edit `Assets/InputSystem_Actions.inputactions`, ensure the input system runtime settings are correct (Project Settings → Player → Active Input Handling) and regenerate any wrappers.

---

> 🌟 This README was generated by inspecting the project files (scripts, scenes, packages) present in the repository. If you want, I can add a short developer guide (how to run local tests, CI build workflow, or call out specific scenes/prefabs in more detail).
