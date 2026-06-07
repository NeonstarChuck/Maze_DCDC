<p align="center">
  <img src="Assets/Images/together apart logo_cropped.png" alt="Together Apart Logo" width="400">
</p>

<p align="center">
  <strong>Mixed-Reality Cooperative Escape Room</strong>
</p>

<p align="center">
  <em>"Closer than you think. Further than you see."</em>
</p>

---

## Introduction

Together Apart is a two-player cooperative mixed-reality (MR) escape room, developed as part of the *Designing Complex Digital Communication (DCDC)* course at Stockholm University (DSV).

Traditional escape rooms require physical co-location, and most VR multiplayer games allow players to succeed individually. Together Apart addresses both: two players occupy separate physical rooms but share a co-located virtual environment through MR passthrough, with every puzzle designed around asymmetric interdependence,each player holds information or that the other player needs. Success is impossible without real-time verbal communication.

---

## Design Process

### Goals

The core goal was to make an MR experience where communication and collaborating was necessary. Secondary goals were to anchor virtual objects in the players' real spaces using MR passthrough and make satisfying interactions.

### Key Design Challenges & Solutions

Hand tracking vs. controllers — Hand tracking is the primary mode for most interactions; controllers are used as physical "keys" for puzzle mechanics. This keeps the experience embodied while providing reliable input for certain interactions.

Asymmetric information — Each stage pairs two different puzzle types, one per player. Neither player can solve their puzzle without information held by the other. This allows for proper collaboration.

Audio feedback — An audio system was implemented so that Player B can hear when Player A solves their puzzle, even without seeing it. Audio is the aspect that both players share together.

Physical interactions in MR — Grab-based interactions proved imprecise where virtual and physical spaces overlap. Key puzzles use proximity zones instead: the player moves a controller near a scanner, mirroring the real-world gesture of tapping a keycard.

Networked state consistency — Photon Fusion 2 was used to ensure stage-completion checks execute in unison between the players.


## Features & Functionalities

### Three-Stage Cooperative Escape Room

#### Stage 1 — "First Contact"
- **Player A:** Color sequence puzzle — press four colored buttons in the correct order. Finds the pattern from thier partner.
- **Player B:** Keycard swipe — Locate the key from their partner and put it in proximity of the lock area.
- Gate opens when both puzzles are solved; door closes automatically after a configurable delay.

#### Stage 2 — "Deeper In"
- **Player A:** Numpad code entry - Finds the code from their partner
- **Player B:** Security key insertion — Locate the key from their partner and put it in proximity of the lock area.

#### Stage 3 — "Final Scan"
- **Both players:** Four hand scanners must all be activated. A scan-bar animation sweeps to indicate that the scanners are working.

### Additional Systems
- **Multiplayer networking** — Photon Fusion 2 host-authoritative model; all puzzle states, door states, timer values, and scanner statuses synchronised via networked properties.
- **Mixed-reality integration** — Meta Quest 3 passthrough overlays virtual puzzle elements on the real environment.
- **Speedrun timer** — Networked `MM:SS.ms` timer visible to both players; freezes on final puzzle completion.
- **Reset controls** — A supervisor participates in the experience, and they can reset the puzzle and open the doors with virtual buttons and controller buttons. This is used in the event that players get stuck.
> 🎬 **Demo Video here:** 

---

## Installation

### Prerequisites

| Requirement | Version |
|---|---|
| Unity Editor | `6000.3.10f1` (Unity 6 LTS) |
| Target Platform | Meta Quest 3|
| Networking | Photon Fusion 2 (included in project) |
| XR Runtime | OpenXR 1.16.1 + Meta OpenXR 2.5.0 |
| Meta XR SDK | `com.meta.xr.sdk.all` v85.0.0 |
| Git LFS | Required for large binary assets |

### Setup Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/NeonstarChuck/Maze_DCDC.git
   cd Maze_DCDC-main
   ```

2. **Open in Unity and build**
   ```
   File → Build Settings → Build And Run for Android
   Upload to headsets via meta quest developer hub
   ```



### Key Dependencies

| Package | Purpose |
|---|---|
| `com.meta.xr.sdk.all` v85.0.0 | Hand tracking, passthrough, spatial anchors |
| `com.unity.xr.interaction.toolkit` v3.3.1 | Controller and hand input |
| `com.unity.xr.openxr` v1.16.1 | OpenXR runtime |
| Photon Fusion 2 | Real-time multiplayer networking |
| `com.unity.render-pipelines.universal` v17.3.0 | URP rendering for Quest 3 |

---

## Usage

1. The supervisor starts the game on their headset, and places the controller keys in the environment. They can remove the walls temporarily with a virtual button in order to place the controllers in the correct places.
2. The other two players join the game session on their headsets. They both begin outside of the maze.
3. They press the big red button to start the timer, and each player a side of the maze to enter, and they begin solving the puzzles.
4. TODO: Some screenshots of the interactions, showing how it works.


### Software & SDKs

- [Unity Engine](https://unity.com/) — v6000.3.10f1
- [Photon Fusion 2](https://www.photonengine.com/fusion)
- [Meta XR SDK](https://developer.oculus.com/documentation/unity/unity-overview/)

### Assets


| Asset | Source |
|---|---|
| `Metronomic Underground.mp3` | Stereolab |

---

## Contributors

Samuel Windheim 
| Chuch Long Ching 
| Evangelos Giaxidis 
| Theeshani Gunarathna 
| Raman Ghimire 

**Course:** Designing Complex Digital Communication (DCDC)  
**Institution:** DSV, Stockholm University · <!-- PLACEHOLDER: e.g. Spring 2026 -->

---