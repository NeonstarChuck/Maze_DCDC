<p align="center">
  <img src="Assets/Images/together apart logo.png" alt="Together Apart Logo" width="400"/>
</p>

<h1 align="center">Together Apart</h1>

<p align="center">
  <em>"Closer than you think. Further than you see."</em>
</p>

<p align="center">
  A cooperative mixed-reality escape room where two players — physically separated — must communicate, coordinate, and combine perspectives to solve puzzles neither can complete alone.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.3.10f1-000000?logo=unity" alt="Unity Version"/>
  <img src="https://img.shields.io/badge/Platform-Meta%20Quest%203-1877F2?logo=meta" alt="Platform"/>
  <img src="https://img.shields.io/badge/Networking-Photon%20Fusion%202-004480" alt="Networking"/>
  <img src="https://img.shields.io/badge/XR-OpenXR%20%2B%20Meta%20SDK-red" alt="XR Runtime"/>
</p>

---

## Table of Contents

1. [Introduction](#introduction)
2. [Design Process](#design-process)
3. [Features & Functionalities](#features--functionalities)
4. [Installation](#installation)
5. [Usage](#usage)
6. [References](#references)
7. [Contributors](#contributors)
8. [License](#license)

---

## Introduction

**Together Apart** is a two-player cooperative mixed-reality (MR) escape room built for the **Meta Quest 3**. The project was developed as part of the *Designing Complex Digital Communication (DCDC)* course at Stockholm University's Department of Computer and Systems Sciences (DSV).

### What Problem Does It Solve?

Traditional escape rooms require physical co-location, making them inaccessible for remote participants. Meanwhile, most VR multiplayer games treat collaboration as optional — players can often succeed individually. **Together Apart** addresses both limitations by creating an experience where:

- Two players occupy **separate physical rooms** but share a **co-located virtual environment** through mixed reality.
- Every puzzle is designed with **asymmetric interdependence** — Player A holds information or capabilities that Player B needs, and vice versa.
- Success is impossible without real-time verbal communication and coordinated physical actions.

### Educational Value

The project explores several concepts from interaction design and human-computer interaction:

- **Co-located vs. remote collaboration** in mixed-reality environments
- **Asymmetric gameplay** as a driver for verbal coordination and shared mental models
- **Embodied interaction** through hand tracking, physical gestures, and spatial reasoning
- **Affordance design** in 3D interfaces (keypads, card scanners, hand scanners) that leverage players' intuition from real-world objects
- **Networked state synchronization** as a design constraint that shapes the player experience

---

## Design Process

### Goals

The core design goal was to create a cooperative MR experience where **communication is not just helpful but structurally necessary**. Each puzzle was designed so that no single player has enough information or physical access to solve it alone, enforcing what we refer to as *designed interdependence*.

Secondary goals included:

- Leveraging Meta Quest 3's mixed-reality passthrough to anchor virtual objects in the players' real physical spaces
- Making interactions feel **tangible and embodied** rather than menu-driven
- Creating a sense of **escalating tension** through a three-stage progression and a visible speedrun timer

### Design Iterations and Challenges

#### Challenge 1: Choosing Between Controller-Based and Hand-Tracked Interaction

**Decision:** We adopted a **hybrid approach** — the primary interaction mode uses Meta Quest 3 controllers for reliability, while specific puzzle elements (the hand scanner in Stage 3) require the player to place their physical hand on a surface.

**Justification:** Early prototyping revealed that full hand tracking introduced too much input ambiguity for precise puzzle interactions like pressing numpad buttons or swiping keycards. Controllers provided the necessary precision for these interactions. However, for the hand-scanning mechanic — where the *gesture itself* is the puzzle (placing your palm flat on a scanner) — hand tracking was the natural and more immersive modality. This aligns with Bowman et al.'s principle that input modality should match the **semantic intent** of the interaction.

#### Challenge 2: Communicating Asymmetric Information

**Decision:** Each stage pairs two fundamentally different puzzle types, one per player. For example, in Stage 1, one player faces a **color-sequence button puzzle** while the other must **physically swipe a keycard** near a proximity sensor. Neither player can see the other's puzzle.

**Justification:** We considered giving both players identical puzzles, but this would reduce the game to parallel solo play. By making the puzzles asymmetric, we force players into a **shared problem-solving dialogue** — one player might say "I see four colored buttons," and the other responds "I have a card that says Red-Yellow-Green-Blue." This design draws on Clark's (1996) theory of *common ground* in communication: the game's structure compels players to actively build shared understanding.

#### Challenge 3: Sound Design for Feedback and Atmosphere

**Decision:** We implemented a **layered audio system** with three tiers:
1. **Ambient background music** — a looping sci-fi track ("Metronomic Underground") to establish atmosphere.
2. **Puzzle chime SFX** — individual confirmation sounds when each sub-puzzle is solved, giving immediate local feedback.
3. **Environmental gate sounds** — door open/close effects synchronized across the network, signaling stage transitions to both players simultaneously.

**Justification:** We rejected a silent design because, without visual line-of-sight to the other player, audio is the primary **shared sensory channel** confirming that a partner's action succeeded. When Player A solves their puzzle, Player B hears the confirmation chime even though they cannot see the solved state — this audio bridge maintains **mutual awareness** (Dourish & Bellotti, 1992) across the asymmetric setup. The timed door-close sound (with a configurable pre-close warning offset) was added after playtest feedback indicated players were disoriented when doors silently shut behind them.

#### Challenge 4: Physical Interactions in Mixed Reality

**Decision:** Key puzzles use **proximity-based trigger zones** rather than grab-and-drop mechanics. The keycard puzzle activates when the physical controller (representing a keycard) enters within 4 cm of the scanner, measured via `Vector3.Distance`.

**Justification:** Grab-based interactions require complex collision handling and often feel imprecise in MR where virtual and physical spaces overlap. Proximity triggers are both more reliable and more closely mirror the real-world action of *tapping a keycard on a reader* — the player moves the controller near the scanner and the system detects the approach. This leverages Fitts's Law in reverse: rather than requiring a precise *target acquisition*, we enlarged the acceptance zone and made the interaction about *approach and presence*, which feels more natural in embodied MR.

#### Challenge 5: Networked State Consistency

**Decision:** All puzzle states are synchronized via **Photon Fusion 2** using `[Networked]` properties with host authority. Player actions trigger `[Rpc]` calls to the host, which validates and commits state changes, then broadcasts updates via Fusion's `ChangeDetector` pattern.

**Justification:** A peer-to-peer model was considered but rejected because it introduces state divergence when two players solve puzzles simultaneously. The host-authoritative model guarantees that the stage-completion checks (`CheckStage1Completion`, `CheckStage2Completion`) execute atomically on a single machine, preventing race conditions where both players report success but the gate fails to open.

### Design Evolution Summary

| Version | Key Change | Reason |
|---------|-----------|--------|
| v0.1 | Single-player maze prototype | Proof of concept for MR wall spawning |
| v0.2 | Two-player networking via Photon Fusion | Core co-op requirement |
| v0.3 | Asymmetric puzzle pairs per stage | Enforce genuine collaboration |
| v0.4 | Keycard proximity system replaces grab mechanics | Reliability and embodied feel |
| v0.5 | Three-stage progression with timed doors | Escalating tension and pacing |
| v0.6 | QR-code spawner for Stage 2 numpad clue | Physical-digital bridge element |
| v0.7 | Hand scanner final stage with progress bar | Climactic cooperative gesture |
| v0.8 | Full audio system + speedrun timer + master reset | Polish and replayability |

---

## Features & Functionalities

### Three-Stage Cooperative Escape Room

The experience is structured as a linear progression through three locked zones. Completing all sub-puzzles in a stage opens the doors to the next zone.

#### Stage 1 — "First Contact"
- **Player A** — *Color Sequence Puzzle*: A numpad displays four colored buttons (Red, Yellow, Green, Blue). The player must press them in the correct order. Incorrect sequences trigger a 1.2-second error lockout with visual feedback. Progress is tracked with star markers (★ ★ _ _).
- **Player B** — *Keycard Swipe*: The player physically moves a keycard (attached to the controller) within proximity of a card reader. A 5.5-second processing animation plays with audio, followed by "Access Granted."
- **Gate opens** when both puzzles are solved. Doors close automatically after a configurable delay.

#### Stage 2 — "Deeper In"
- **Player A** — *Numpad Code Entry*: A full numeric keypad where the player enters a code. The code is hinted at through QR codes placed in the physical room, which spawn 3D number models in MR when scanned.
- **Player B** — *Security Key Insertion*: A second proximity-based key puzzle. The player brings a security key (controller) near the lock zone to trigger the solve.
- **Gate opens** when both puzzles report completion to the host.

#### Stage 3 — "Final Scan"
- **Both Players** — *Cooperative Hand Scanners*: Four hand scanners must all be activated. Each scanner plays a scan-bar animation (a visual bar sweeps from start to end position over 2 seconds). The scan bar changes color from red (idle) to green (scanning). All four scanners must complete for the final door to open.
- A **central progress bar** tracks overall completion and plays a flashing celebration animation when all scanners finish.

### Multiplayer Networking
- **Photon Fusion 2** host-authoritative networking ensures deterministic puzzle state across both headsets.
- All puzzle completions, door states, timer values, and scanner statuses are synchronized via `[Networked]` properties.
- RPCs handle player-initiated actions; `ChangeDetector` triggers audio and visual updates on remote clients.

### Mixed-Reality Integration
- **Meta Quest 3 passthrough** renders virtual puzzle elements (walls, doors, keypads, scanners) overlaid on the player's real physical environment.
- **QR code scanning** via Meta's MR Utility Kit (MRUK) bridges the physical and digital worlds — real QR codes placed in the room spawn virtual 3D objects.
- **Spatial anchoring** through MRUK's `MRUKTrackable` system.

### Audio System
- Looping background music with automatic restart on game reset.
- Per-puzzle confirmation chimes.
- Networked door open/close sounds with configurable timing offsets.
- Victory celebration audio on game completion.

### Speedrun Timer
- Networked timer visible to both players, formatted as `MM:SS.ms`.
- Automatically freezes when the final puzzle is completed.
- Resets with the master reset system.

### Master Reset System
- Press **B button (OVR)** or **R key (desktop)** to trigger a full game reset.
- Resets all puzzle states, doors, timers, QR-spawned objects, scanner states, and keypad UIs.
- Restarts background music across all clients.
- Protected with try-catch blocks to prevent partial reset failures.

---

## Installation

### Prerequisites

| Requirement | Version / Details |
|---|---|
| **Unity Editor** | `6000.3.10f1` (Unity 6 LTS) |
| **Target Platform** | Meta Quest 3 (Android) |
| **Networking** | Photon Fusion 2 (included in project) |
| **XR Runtime** | OpenXR 1.16.1 + Meta OpenXR 2.5.0 |
| **Meta XR SDK** | `com.meta.xr.sdk.all` v85.0.0 |
| **Operating System** | Windows 10/11 or macOS (for Unity Editor) |
| **Git LFS** | Required (for large binary assets) |

### Step-by-Step Setup

1. **Clone the repository**

   ```bash
   git clone <REPOSITORY_URL>
   cd Maze_DCDC-main
   ```

2. **Install Unity Hub** (if not already installed)

   Download from [unity.com/download](https://unity.com/download). Install Unity Hub and sign in with a Unity account.

3. **Install the correct Unity version**

   In Unity Hub, go to **Installs → Install Editor** and select version **6000.3.10f1**. Ensure the following modules are checked during installation:
   - **Android Build Support** (includes Android SDK & NDK, OpenJDK)

4. **Open the project**

   In Unity Hub, click **Open → Add project from disk** and select the cloned `Maze_DCDC-main` folder. Unity will resolve all packages automatically from `Packages/manifest.json`.

5. **Configure Photon Fusion**

   <!-- PLACEHOLDER: Add your Photon App ID configuration steps here -->
   - Open **Fusion → Fusion Hub** in the Unity menu bar.
   - Enter your **Photon App ID** (create one at [dashboard.photonengine.com](https://dashboard.photonengine.com) under the "Fusion" category).
   - Save the configuration.

6. **Connect your Meta Quest 3**

   - Enable **Developer Mode** on your Quest 3 via the Meta Quest app on your phone.
   - Connect the headset to your PC via USB-C (or use Air Link for wireless deployment).
   - In Unity, go to **File → Build Settings**, select **Android**, and click **Switch Platform**.
   - Under **Run Device**, select your connected Quest 3.

7. **Build and deploy**

   ```
   File → Build Settings → Build And Run
   ```

   The APK will be compiled and pushed directly to the headset.

### Key Dependencies

| Package | Purpose |
|---|---|
| `com.meta.xr.sdk.all` (v85.0.0) | Meta Quest hand tracking, passthrough, spatial anchors |
| `com.unity.xr.interaction.toolkit` (v3.3.1) | XR Interaction Toolkit for controller & hand input |
| `com.unity.xr.openxr` (v1.16.1) | OpenXR runtime |
| `com.unity.xr.meta-openxr` (v2.5.0) | Meta-specific OpenXR extensions |
| `com.unity.xr.oculus` (v4.5.2) | Oculus platform integration |
| Photon Fusion 2 | Real-time multiplayer networking (included in `Assets/Photon`) |
| `com.atteneder.gltfast` | Runtime glTF/GLB model loading (volcano 3D model) |
| `com.veriorpies.parrelsync` | Unity editor cloning for local multiplayer testing |
| `se.su.dsv.extralitylab.unity` | DSV ExtralityLab utilities |
| NavKeypad (Third-party asset) | 3D keypad prefab and input logic (`Assets/Keypad`) |
| `com.unity.render-pipelines.universal` (v17.3.0) | URP rendering for Quest 3 performance |

---

## Usage

### Starting the Experience

1. **Launch the app** on two Meta Quest 3 headsets connected to the same Photon room.
2. One player is automatically designated as the **Host** (state authority). The other joins as a **Client**.
3. Both players see the virtual escape room overlaid on their real environment via MR passthrough.
4. Press the **Start Experience** button to begin. The **speedrun timer** starts counting.

### Interacting with Puzzles

| Interaction | How To Perform | Stage |
|---|---|---|
| **Press colored buttons** | Point controller at the button and press the trigger | Stage 1 |
| **Swipe keycard** | Move your controller (with virtual keycard) within ~4 cm of the card reader | Stage 1 |
| **Enter numpad code** | Point and press number buttons on the 3D keypad | Stage 2 |
| **Scan QR codes** | Look at physical QR codes in the room; the system auto-detects and spawns clue models | Stage 2 |
| **Insert security key** | Move controller near the lock zone (~25 cm proximity) | Stage 2 |
| **Activate hand scanner** | <!-- PLACEHOLDER: Describe the exact hand scanner activation gesture --> Place your hand near the scanner surface to initiate the scan bar animation | Stage 3 |
| **Reset the game** | Press **B button** on the right controller (or **R** on keyboard in editor) | Any time |

### Communication Tips

- **Talk constantly.** The game is designed so that neither player can see the other's puzzle. Describe what you see.
- **Share codes verbally.** Stage 2 requires one player to read QR-spawned numbers aloud while the other enters them on the keypad.
- **Coordinate timing.** Stage 3 requires all scanners to complete — work together to activate them efficiently.

### Testing in the Unity Editor

For local development and testing without two headsets:

1. Use **ParrelSync** (included in the project) to create a clone of the Unity project.
   - Go to **ParrelSync → Clones Manager → Create New Clone**.
2. Open the original project and the clone in two separate Unity Editor instances.
3. Press Play in both editors — one will act as Host, the other as Client.
4. Use keyboard controls (`R` for reset) to simulate gameplay.

<!-- PLACEHOLDER: Add screenshots or GIFs of gameplay here -->
<!-- Example:
![Stage 1 Color Puzzle](docs/screenshots/stage1_colors.png)
![Stage 2 Keypad](docs/screenshots/stage2_keypad.png)
![Stage 3 Hand Scanners](docs/screenshots/stage3_scanners.png)
-->

<!-- PLACEHOLDER: Add a link to a demo video -->
<!-- **🎬 Demo Video:** [Watch on YouTube](https://youtube.com/your-demo-link) -->

---

## References

### Academic & Theoretical

- Bowman, D. A., Kruijff, E., LaViola, J. J., & Poupyrev, I. (2004). *3D User Interfaces: Theory and Practice*. Addison-Wesley.
- Clark, H. H. (1996). *Using Language*. Cambridge University Press.
- Dourish, P., & Bellotti, V. (1992). Awareness and coordination in shared workspaces. *Proceedings of CSCW '92*, 107–114.
- Fitts, P. M. (1954). The information capacity of the human motor system in controlling the amplitude of movement. *Journal of Experimental Psychology*, 47(6), 381–391.

### Software & SDKs

- [Unity Engine](https://unity.com/) — Game engine (v6000.3.10f1)
- [Photon Fusion 2](https://www.photonengine.com/fusion) — Multiplayer networking framework
- [Meta XR SDK](https://developer.oculus.com/documentation/unity/unity-overview/) — Mixed-reality passthrough, hand tracking, spatial anchors
- [OpenXR](https://www.khronos.org/openxr/) — Cross-platform XR runtime standard
- [ParrelSync](https://github.com/VeriorPies/ParrelSync) — Unity editor cloning for multiplayer testing
- [glTFast](https://github.com/atteneder/glTFast) — Runtime glTF/GLB model loading
- [NavKeypad](https://assetstore.unity.com/) — 3D keypad UI asset <!-- PLACEHOLDER: Add exact Asset Store link -->
- [TextMesh Pro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest) — Advanced text rendering

### Audio Assets

<!-- PLACEHOLDER: Add proper attribution for each audio file. Below are the filenames found in the project. Replace with actual source/author. -->

| Sound File | Source / Attribution |
|---|---|
| `Metronomic Underground.mp3` | <!-- PLACEHOLDER: Credit the original artist/source --> |
| `congratulations-you-won.mp3` | <!-- PLACEHOLDER: Source --> |
| `lets-celebrate.mp3` | <!-- PLACEHOLDER: Source --> |
| `minecraft_click.mp3` | <!-- PLACEHOLDER: Source --> |
| `okay-lets-go_buBmJye.mp3` | <!-- PLACEHOLDER: Source --> |
| `sci fi door sound effect.mp3` | <!-- PLACEHOLDER: Source --> |
| `tbeb-correct-answer.mp3` | <!-- PLACEHOLDER: Source --> |
| `usethis.mp3` | <!-- PLACEHOLDER: Source --> |

### 3D Models

| Model | Source / Attribution |
|---|---|
| `mt._vesuvius_italy.glb` | <!-- PLACEHOLDER: Credit the 3D model source (e.g., Sketchfab author) --> |
| `volcano.glb` | <!-- PLACEHOLDER: Credit the 3D model source --> |
| Combination Pad Lock | <!-- PLACEHOLDER: Credit --> |
| Modular Letters (LittleDog) | <!-- PLACEHOLDER: Credit --> |

---

## Contributors

<!-- PLACEHOLDER: Replace with actual team member information -->

| Name | Role | Contact |
|---|---|---|
| <!-- Your Name --> | <!-- e.g., Lead Developer, Interaction Designer --> | <!-- email@student.su.se --> |
| <!-- Team Member 2 --> | <!-- Role --> | <!-- email@student.su.se --> |
| <!-- Team Member 3 --> | <!-- Role --> | <!-- email@student.su.se --> |
| <!-- Team Member 4 --> | <!-- Role --> | <!-- email@student.su.se --> |

**Course:** Designing Complex Digital Communication (DCDC)
**Institution:** Department of Computer and Systems Sciences (DSV), Stockholm University
**Semester:** <!-- PLACEHOLDER: e.g., Spring 2026 -->

---

## License

<!-- PLACEHOLDER: Choose and specify your license. Common options for academic projects: -->

This project was developed for educational purposes as part of the DCDC course at Stockholm University.

<!-- Uncomment one of the following or add your own:
[MIT License](LICENSE)
[CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/)
-->

---

<p align="center">
  <em>Built with 🎮 Unity 6 · 🥽 Meta Quest 3 · 🌐 Photon Fusion 2</em>
</p>
