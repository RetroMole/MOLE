using System.IO;
using Serilog;
using ImGuiNET;

namespace RetroMole;

public static partial class Gui
{
    public partial class WindowTypes
    {
        public class FileDialog : RetroMole.Gui.Window
        {
            public (List<string>, List<string>) Fse = (new List<string>(), new List<string>());
            public int Selected = -1;
            public string StartPath;
            public string CurrPath;
            public FileMode Mode;
            public string Filter;
            public FileDialog(string Name, string StartPath, FileMode Mode, string Filter) : base(Name, 300, 600, ImGuiWindowFlags.Modal, IsPopUp: true) 
            {
                CurrPath = this.StartPath = StartPath;
                this.Mode = Mode;
                this.Filter = Filter;
                Fse = GetFSE(CurrPath, Filter);
            }
            public override void Draw(string Name, int W, int H)
            {
                ImGui.SetNextItemWidth(W);
                ImGui.InputText("", ref CurrPath, 4096, ImGuiInputTextFlags.ReadOnly);

                var p = CurrPath;
                if(!ImGui.BeginListBox("", new(W, H-25))) return;

                if (new DirectoryInfo(Path.GetFullPath(CurrPath)).Parent is not null)
                    if (ImGui.Selectable(".."))
                        ItemClicked(-1);

                for (int i = 2; i < Fse.Item1.Count + 2; i++)
                    Utility.WithColors(() => {
                            if (ImGui.Selectable(Fse.Item1[i-2], Selected == -i))
                                ItemClicked(-i);
                        },
                        (ImGuiCol.Text, 0xFF00FFFF)
                    );

                if (!new FileMode[] {FileMode.OpenFolder, FileMode.SaveFolder}.Contains(Mode))
                    for (int i = 0; i < Fse.Item2.Count; i++)
                        Utility.WithColors(() => {
                            if (ImGui.Selectable(Fse.Item2[i], Selected == i))
                                ItemClicked(i);
                            },
                            (ImGuiCol.Text, 0xFFFFFF00)
                        );

                ImGui.EndListBox();

                if (p != CurrPath)
                    Fse = GetFSE(CurrPath, Filter);

                Utility.WithSameLine(
                    () => { if (ImGui.Button("Cancel")) CloseWindow(String.Empty); },
                    () => Utility.WithDisabled(Mode switch {
                            FileMode.OpenFile => Selected < 0,
                            FileMode.SaveFile => Selected < 0,
                            FileMode.OpenFolder => Selected > -2,
                            FileMode.SaveFolder => Selected > -2,
                            _ => throw new System.Exception("Invalid Mode")
                        },
                        () => { if (ImGui.Button("Select"))
                            CloseWindow(Path.GetFullPath(Path.Combine(CurrPath, Mode switch {
                                FileMode.OpenFile   => Fse.Item2[Selected],
                                FileMode.OpenFolder => Fse.Item1[-Selected-2],
                                FileMode.SaveFile   => Fse.Item2[Selected],
                                FileMode.SaveFolder => Fse.Item1[-Selected-2],
                                _ => throw new System.Exception("Invalid Mode")
                            })));
                        }
                    )
                );
            }

            public void ItemClicked(int idx)
            {
                if (Selected == idx)
                {
                    switch (Selected)
                    {
                        case -1:
                            CurrPath = Path.GetFullPath(Path.Combine(CurrPath, ".."));
                            break;
                        case <0:
                            CurrPath = Path.GetFullPath(Path.Combine(CurrPath, Fse.Item1[-Selected-2]));
                            break;
                        default:
                            CloseWindow(new object[] { 
                                Path.GetFullPath(Path.Combine(CurrPath, Mode switch {
                                    FileMode.OpenFile   => Fse.Item2[Selected],
                                    FileMode.OpenFolder => Fse.Item1[-Selected-2],
                                    FileMode.SaveFile   => Fse.Item2[Selected],
                                    FileMode.SaveFolder => Fse.Item1[-Selected-2],
                                    _ => throw new System.Exception("Invalid Mode")
                                }))
                            });
                            break;
                    }
                    Selected = -1;
                    return;
                }
                Selected = idx;
            }

            public (List<string>, List<string>) GetFSE(string path, string filter = "*")
            {
                var entries = new DirectoryInfo(path).GetFileSystemInfos(filter);
                List<string> dirs = new List<string>();
                List<string> files = new List<string>();
                foreach (var entry in entries)
                {
                    if (entry is FileInfo)
                    {
                        files.Add(entry.Name);
                        continue;
                    }

                    dirs.Add(entry.Name);
                }
                dirs.Sort();
                files.Sort();
                return (dirs, files);
            }

            public enum FileMode
            {
                OpenFile,
                OpenFolder,
                SaveFile,
                SaveFolder
            }
        }
    }
}
