
**MOLE** is being designed as an **OpenSource**  editor for **Super Mario World** with the intent to bring many of the features from existing SMW ROMHacking tools/patches such as **Lunar Magic**, **Asar**, **Pixi**, **YYCHR**, **GPS**, **UberASM**,  **SA-1**, and many others into a single easy-to-use [**multi-platform**](#how-to-run) tool translated by the volunteers to [many languages](#translation)
___
### How to Run
At the moment MOLE is still in a very early stage and therefore no public releases have been setup, however in the near future you will be able to use MOLE on their specific platform trough one of the following methods:
- Windows Users can download and run one of our WinForms Releases
- ...
- MOLE Is also available [online](https://smw-mole.herokuapp.com/) as a WebApp (Read [this]() for how to run locally)

If you still insist on running MOLE in its current very early stages, read the following section on how to build it yourself.
___
### How to Build
	(SECTION WIP)
___
### Translation
	(SECTION WIP)
___
### Project Structure
```
•─ azure/
	•─ Azure Pipelines
•─ res/
	•─ Icons, Images, Text Tables, etc...
•─ src/
	•─ Source Files
	•─ Back/
		•─ .NET Framework 4.8 Backend DLL containing most of the important MOLE editing code
	•─ Win/
		•─ .NET Framework 4.8 Winforms UI Frontend
	•─ Web/
		•─ .NET Core 3.1 ASP.NET Server for Angular UI Frontend
	•─ Angular/
		•─ Angular 10 App containining components and code for Web to Serve as a Web UI Frontend
```
___
You can find a lot of useful information about MOLE, Lunar Magic, SMW Romhacking, and SNES development in general, in the [Useful Links](/wiki/useful-links) Section of the [MOLE Wiki](/wiki)

This software is licensed under the [GNU General Public License v3.0](/LICENSE.md)
