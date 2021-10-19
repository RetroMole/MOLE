[![Build & Test status](https://github.com/Vawlpe/MOLE/actions/workflows/build.yaml/badge.svg)](https://github.com/Vawlpe/MOLE/actions/workflows/build.yaml)
[![License](https://badgen.net/github/license/Vawlpe/MOLE)](./LICENSE.md)
[![Latest commit](https://img.shields.io/github/last-commit/Vawlpe/MOLE)](https://github.com/Vawlpe/MOLE/commits/)
[![Version](https://badge.fury.io/gh/Vawlpe%2FMOLE.svg)](https://github.com/Vawlpe/MOLE)
[![Discord](https://img.shields.io/discord/591914197219016707.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/hAGM9UPv4q)

**MOLE** is being designed as an **OpenSource**  editor for **Super Mario World** with the intent to bring many of the features from existing SMW ROMHacking tools/patches such as **Lunar Magic**, **Asar**, **YYCHR**, and [many others](#compatibility) into a single easy-to-use [**multi-platform**](#how-to-run) and [localized](#localizationtranslation) tool.

**This is a fork. For support, visit the original repo.**
___
### Platforms
Thanks to OpenGL Monogame
- Windows
- Linux
- MacOS
___
### How to run
Releases are not available yet, but you can compile it yourself.
___
### How to Build
1) Clone repository
2) Open `src/` directory as solution
3) Compile `Mole.Gui` project
4) Done!
___
### Localization/Translation
This is not yet done.
___
### Project Structure
```
res
└── Resources
    
src
├── Mole.Shared
│   └── Shared MOLE library, backend
├── Mole.Monogame/
│   └── GUI relying on Crossplaform MonoGame Engine (OpenGL), uses ImGui UI
├── Mole.Veldrid/
│   └── GUI relying on Veldrid, uses ImGui UI
└── Mole.Gui/
    └── ImGui UI stuff, entry point
```
___
### Compatibility
MOLE intends to be compatible with both clean ROMs, as well as most modern SMW ROMhacking tools, ROM variants, and patches. This includes:
#### ROM Versions:
- North American (NTSC U)
- SA-1 (NTSC U only, using the SA-1 Kit patch)
- Japanese (NTSC J)<sup>Only basic compatibility, see [#20](/../../issues/20)</sup>
- All Stars + World<sup>Only basic compatibility, see [#21](/../../issues/20)</sup>
#### Tools:
|  | To MOLE | From MOLE | File formats |
|:---:|:---:|:---:|:---:|
| Lunar Magic | ⚠️ Partial WIP️<br>Currently our efforts are focused on clean ROMs, therefore certain LM specific features may be incompatible | ⚠ ️Partial WIP<br>LM will force a copier header<br>Some MOLE specific hijacks may be incompatible | ROM's (.smc, .sfc), Level (.mwl), Map16 (.map16), Graphics (.bin), Palette (.pal) |
| Asar | ✅ Full | ⚠️ Partial WIP<br>Asar will refuse to work .sfc files with a copier header, and .smc files without one, for consistency | ROM (.smc, .sfc), Patch (.asm) |
| YY-CHR (And other SNES tile editors) | ✅ Full WIP | ✅ Full WIP | Graphics (.bin), Palette (.pal) |
| AddMusicK | ✅ Full WIP | ⚠️ Partial WIP<br>Most tools expect the ROM to have a copier header and a .smc file extension | ROM (.smc, .sfc), Music (.txt), Sample (.brr), Config info (Addmusic_sample groups.txt, Addmusic_list.txt)|
| PIXI | ✅ Full WIP | ⚠️ Partial WIP️<br>Most tools expect the ROM to have a copier header and a .smc file extension | ROM (.smc, .sfc), Sprite code (.asm), Config info (.cfg, .json) |
| GPS | ✅ Full WIP | ⚠️ Partial WIP<br>Most tools expect the ROM to have a copier header and a .smc file extension | ROM (.smc, .sfc), Block code (.asm, routines/.asm), Config info (list.txt) |
| UberASM | ✅ Full WIP | ⚠️ Partial WIP<br>Most tools expect the ROM to have a copier header and a .smc file extension | ROM (.smc, .sfc), Patch (.asm), Config info (list.txt) |
___
### More information on SMW ROM Hacking and MOLE
You can find a lot of useful information about MOLE, SMW ROM Hacking, and SNES development in general, in the [Useful Links](../../wiki/useful-links) Section of the [MOLE Wiki](../../wiki)
___
### License
[GNU General Public License v3.0](/LICENSE.md)

MOLE is an open source Super Mario World ROM editor and is in no way affiliated with Nintendo.
Copyright (C) 2021 Vawlpe

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
