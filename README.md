[![Build](https://github.com/Vawlpe/MOLE/actions/workflows/build.yaml/badge.svg)](https://github.com/Vawlpe/MOLE/actions/workflows/build.yaml)
[![Test](https://github.com/Vawlpe/MOLE/actions/workflows/test.yaml/badge.svg)](https://github.com/Vawlpe/MOLE/actions/workflows/test.yaml)
[![Asar](https://github.com/Vawlpe/MOLE/actions/workflows/asar.yaml/badge.svg)](https://github.com/Vawlpe/MOLE/actions/workflows/asar.yaml)

[![License](https://badgen.net/github/license/Vawlpe/MOLE)](https://github.com/Vawlpe/MOLE/blob/master/LICENSE.md)
[![Latest commit](https://img.shields.io/github/last-commit/Vawlpe/MOLE)](https://github.com/Vawlpe/MOLE/commits/)
[![Version](https://badge.fury.io/gh/Vawlpe%2FMOLE.svg)](https://github.com/Vawlpe/MOLE/tags)

[![Discord](https://img.shields.io/discord/729355207862911027?label=Discord)](https://discord.gg/hAGM9UPv4q)
[![Trello](https://img.shields.io/badge/Trello-workspace-blue)](https://trello.com/mole34)

**MOLE** is being designed as an **OpenSource**  editor for **Super Mario World**.
With the intent to bring many of the features from existing SMW ROMHacking tools/patches such as **Lunar Magic**, **Asar**, **YYCHR**, and [many others](#compatibility),
into a single easy-to-use [**multi-platform**](#how-to-run) and [localized](#localizationtranslation) tool.
Mole doesn't mean to replace existing system, only encapsulate them, the existing formats and standards will be kept,
but we shall create better standards on top of the existing ones.
___
### Platforms
- Windows
- Linux
- MacOS
___
### How to run
Releases are not available yet, but you can [build Mole yourself](#how-to-build), or try out the [build artifacts](https://github.com/Vawlpe/MOLE/actions/workflows/build.yaml) of any commit
___
### How to Build
Requirements: .NET 6
1. Clone repository (`git clone --recursive https://github.com/Vawlpe/MOLE.git`)
2. Build solution (`dotnet build ./src/Mole.sln`) **or** open `./src/Mole.sln` in your prefered IDE and use the built-in `Build` option
3. Done! you can now run Mole from `./src/Mole.Gui/bin/Debug/net6.0/Mole.EntryPoint` (or any other specified output directory)
___
### Localization/Translation
WIP
___
### Project Structure
```
.github
├── dependabot.yml
│   └── Automatically check and update dependencies
├── workflows
│   ├── asar.yaml
│   │   └── Automatically build and commit asar native dependencies from submodule clone in deps
│   ├── build.yaml
│   │   └── Automatically build Mole and upload artifacts
│   └── test.yaml
│      └── Run unit tests on Mole after build.yaml runs
deps
├── TerraCompress
│   └── Modified clone of Smallhacker/TerraCompress
├── asar
│   ├── Asar
│   │   └── Submodule clone of RPGHacker/asar at specific commit (Used by asar.yaml workflow to build libasar native dependencies)
│   └── Auto-built libasar native dependencies (.dll, .so, .dylib)
├── cimgui
│   └── Pre-build native UI framework dependency
res
├── Resources such as images, icons, localization files, etc...
│
src
├── Mole.Shared
│   └── Shared MOLE library, containing all the actual ROM editing code
├── Mole.Monogame/
│   └── GUI renderer relying on MonoGame Engine (OpenGL)
├── Mole.Gui/
│   └── Actual GUI code using ImGui.Net
├── Mole.EntryPoint/
│   └── Entry point for Mole, parses cli arguments, initializes logger, scans for available renderers to dynamically load and runs the UI
├── Mole.BaseRenderer/
│   └── Base interface for the dynamically loaded renderers
└── Mole.MonogameRenderer/
    └── Example renderer that implements IRenderer from Mole.BaseRenderer using Monogame DesktopGL
```
___
### Compatibility
MOLE intends to be compatible with both clean ROMs, as well as most modern SMW ROMhacking tools, ROM variants, and patches. This includes:
#### ROM Versions:
- North American (NTSC U)
- SA-1 (NTSC U only, using the SA-1 Pack) <sup>See [#35](https://github.com/Vawlpe/MOLE/issues/35)</sup>
- Japanese (NTSC J)<sup>Only basic compatibility, see [#20](https://github.com/Vawlpe/MOLE/issues/20)</sup>
- All Stars + World<sup>Only basic compatibility, see [#21](https://github.com/Vawlpe/MOLE/issues/21)</sup>
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
### Resources
You can [get started with MOLE now](https://github.com/Vawlpe/MOLE/wiki/Getting-Started) or find a lot of useful information about MOLE, SMW ROM Hacking, and SNES development in general, in the [Useful Links](https://github.com/Vawlpe/MOLE/wiki/Useful-Links) Section of the [MOLE Wiki](https://github.com/Vawlpe/MOLE/wiki)
___
### License
[GNU General Public License v3.0](https://github.com/Vawlpe/MOLE/blob/master/LICENSE.md)

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
