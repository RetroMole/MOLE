# ***OLD CODE MOST OF THIS WON'T BE USED, CHECK THE master BRANCH FOR CURRENT CODE***
___
**MOLE** is being designed as an **OpenSource**  editor for **Super Mario World** with the intent to bring many of the features from existing SMW ROMHacking tools/patches such as **Lunar Magic**, **Asar**, **Pixi**, **YYCHR**, **GPS**, **UberASM**,  **SA-1**, and many others into a single easy-to-use [**multi-platform**](#how-to-run) and [localized](#localizationtranslation) tool.
___
### How to Run
At the moment MOLE is still in a very early stage and therefore no public releases have been setup, however in the near future you will be able to use MOLE on their specific platform trough one of the following methods:
- Windows (WIP)
- Linux (WIP)
- MacOS (OSX) (WIP)

Or you could build it yourself:
___
### How to Build
	(SECTION WIP)
___
### Localization/Translation
	(SECTION WIP)
___
### Project Structure
```
•─ buildSystem/
	•─ Automatic build pipelines
•─ res/
	•─ Icons, Images,  Text KeyTables (for localization), etc...
•─ src/
	•─ MOLE/
		•─ Backend code, most systems rely on this to edit the ROM.
	•─ UI/
		•─ ImGui UI window definitions and main program entry point.
	•─ ImGUI.Net/
		•─ ImGui.Net library submodule.
	•─ VeldridController/
		•─ Veldrid UI Rendering backend for DirectX 11, Vulka, and Metal (OSX) support (with more targets coming soon)
	•─ XNAController/
		•─ Monogame XNA based UI Rendering backend for OpenGL support (with more targets coming soon)
```
___
### More information on SMW ROMHacking and MOLE
You can find a lot of useful information about MOLE, Lunar Magic, SMW Romhacking, and SNES development in general, in the [Useful Links](/wiki/useful-links) Section of the [MOLE Wiki](/wiki)
___
### License
[GNU General Public License v3.0](/LICENSE.md)

MOLE is an open source Super Mario world ROM editor and is in no way affiliated with Nintendo.
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
