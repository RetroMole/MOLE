using ImGuiNET;
using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;
using System.Numerics;

namespace RetroMole.Gui.Dialogs
{
    public class FilePicker : WindowBase
    {
        public string WindowTitle;
        public string RootFolder = "";
        public string CurrentFolder = "";
        public string SelectedFile = "";
        public List<string> SelectedFiles;
        public List<List<string>> AllowedExtensions;
        public int CurrentExtensions;
        public bool OnlyFolders;
        public bool MultipleSelections;
        public bool ShowBookmarksAndHistory;

        public FilePicker(string WindowTitle, string StartingPath, string SearchFilter = null, bool OnlyFolders = false, bool MultipleSelections = false)
        {
            if (File.Exists(StartingPath))
            {
                StartingPath = new FileInfo(StartingPath).DirectoryName;
            }
            else if (string.IsNullOrEmpty(StartingPath) || !Directory.Exists(StartingPath))
            {
                StartingPath = Environment.CurrentDirectory;
                if (string.IsNullOrEmpty(StartingPath))
                    StartingPath = AppContext.BaseDirectory;
            }
            this.WindowTitle = WindowTitle;
            RootFolder = StartingPath;
            CurrentFolder = StartingPath;
            this.OnlyFolders = OnlyFolders;
            this.MultipleSelections = MultipleSelections;
            if (MultipleSelections)
            {
                SelectedFiles = new();
            }
            if (SearchFilter is not null)
            {
                AllowedExtensions = new();
                AllowedExtensions.AddRange(SearchFilter.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()).ToList());
            }
        }
        public override void Draw(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            // Deal with opening the window
            if (!ShouldDraw) return;
            if (!ImGui.IsPopupOpen($"{WindowTitle}##DialogFilePicker"))
                ImGui.OpenPopup($"{WindowTitle}##DialogFilePicker");

            ImGui.SetNextWindowSize(new Vector2(600, 500));
            if (!ImGui.BeginPopupModal($"{WindowTitle}##DialogFilePicker", ref ShouldDraw, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.MenuBar))
            {
                return;
            }

            //============Menu Bar=============
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Import System Bookmarks")) { }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Bookmarks And History", "", ShowBookmarksAndHistory))
                        ShowBookmarksAndHistory = !ShowBookmarksAndHistory;
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            //===========Path Entry============
            ImGui.SameLine(); ImGui.Text("Path:");
            ImGui.PushItemWidth(-5);
            ImGui.SameLine(); ImGui.InputText("##Path", ref CurrentFolder, 500);
            ImGui.PopItemWidth();

            //=======Bookmarks & History=======
            if (ShowBookmarksAndHistory)
            {
                ImGui.Columns(2, "##FilePicker", false);
                ImGui.SetColumnWidth(0, 300);
                ImGui.SetColumnWidth(1, 300);
                DrawBookmarks();
                DrawHistory();
                ImGui.NextColumn();
            }

            //==============Files==============
            DrawFiles();
            DrawButtons();
            ImGui.Columns(1);
            ImGui.EndPopup();
        }

        /// <summary>
        /// Draw File/Folder Entires
        /// </summary>
        private void DrawFiles()
        {
            if (ImGui.BeginListBox("##Files", new Vector2(-1, -25)))
            {
                var di = new DirectoryInfo(CurrentFolder is null or "" ? "/" : CurrentFolder);
                if (di.Exists)
                {
                    // Force an extra entry at the beggining to go up the folder hierarchy if possible
                    if (di.Parent != null && CurrentFolder != RootFolder)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FFFF);
                        if (ImGui.Selectable("../", false, ImGuiSelectableFlags.DontClosePopups))
                            CurrentFolder = di.Parent.FullName;

                        ImGui.PopStyleColor();
                    }

                    var fileSystemEntries = GetFileSystemEntries(di.FullName);
                    foreach (var fse in fileSystemEntries)
                    {
                        // Draw Directory entry
                        if (Directory.Exists(fse))
                        {
                            var name = Path.GetFileName(fse);
                            ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FFFF);
                            if (ImGui.Selectable(name + "/", false, ImGuiSelectableFlags.DontClosePopups))
                            {
                                CurrentFolder = fse;
                                SelectedFile = null;
                                SelectedFiles?.Clear();
                            }
                            ImGui.PopStyleColor();
                        }
                        // Draw File entry (handle multi-selection)
                        else if (MultipleSelections)
                        {
                            var name = Path.GetFileName(fse);
                            bool isSelected = SelectedFiles.Contains(fse);
                            if (ImGui.Selectable(name, isSelected, ImGuiSelectableFlags.DontClosePopups))
                            {
                                // Ctrl+Click to toggle selection
                                if (ImGui.GetIO().KeyCtrl)
                                {
                                    if (SelectedFiles.Contains(fse)) SelectedFiles.Remove(fse); // remove selection
                                    else SelectedFiles.Add(fse); // add selection
                                }
                                // Shift+Click to select a range of entries (bit broken rn
                                else if (ImGui.GetIO().KeyShift && SelectedFiles.Any())
                                {
                                    SelectedFiles.AddRange(fileSystemEntries
                                        .Skip(fileSystemEntries.IndexOf(SelectedFiles[0]))
                                        .Take(fileSystemEntries.IndexOf(fse) - fileSystemEntries.IndexOf(SelectedFiles[0]) + 1));
                                }
                                // Click to select a single file
                                else
                                {
                                    SelectedFiles.Clear();
                                    SelectedFiles.Add(fse);
                                }
                            }
                        }
                        // Draw File entry
                        else
                        {
                            var name = Path.GetFileName(fse);
                            bool isSelected = SelectedFile == fse;
                            if (ImGui.Selectable(name, isSelected, ImGuiSelectableFlags.DontClosePopups))
                                SelectedFile = fse;

                            if (ImGui.IsMouseDoubleClicked(0))
                                ShouldDraw = false;
                        }
                    }
                }
                ImGui.EndListBox();
            }
        }

        /// <summary>
        /// Draw Bookmarked/Favorite entries
        /// </summary>
        private static void DrawBookmarks()
        {
            ImGui.Text("Bookmarks"); ImGui.SameLine();
            ImGui.Button("Add Bookmark"); ImGui.SameLine(); ImGui.Button("Clear Bookmarks");
            if (ImGui.BeginListBox("##Bookmarks", new Vector2(-1, 185)))
            {
                ImGui.Selectable("Test#1", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#2", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#3", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#4", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.EndListBox();
            }
        }

        /// <summary>
        /// Draw Recent/History entries
        /// </summary>
        private static void DrawHistory()
        {
            ImGui.Text("Recent"); ImGui.SameLine(); ImGui.Button("Clear History");
            if (ImGui.BeginListBox("##Recent", new Vector2(-1, -1)))
            {
                ImGui.Selectable("Test#1", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#2", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#3", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#4", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#5", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#6", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#7", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#8", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#9", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.Selectable("Test#A", false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetWindowContentRegionMax().X - 25, 0)); ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 15); ImGui.Button("X");
                ImGui.EndListBox();
            }
        }

        private void DrawButtons()
        {
            if (ImGui.Button("Deselect"))
                SelectedFiles.Clear();

            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                SelectedFile = null;
                ShouldDraw = false;
                TriggerClose();
            }

            if (OnlyFolders)
            {
                ImGui.SameLine();
                if (ImGui.Button("Open"))
                {
                    SelectedFile = CurrentFolder;
                    ShouldDraw = false;
                    TriggerClose();
                }
            }
            else if (SelectedFile is not null || SelectedFiles?.Any() is true)
            {
                ImGui.SameLine();
                if (ImGui.Button("Open"))
                {
                    ShouldDraw = false;
                    TriggerClose();
                }
            }
            if (AllowedExtensions is not null)
            {
                var Exts = AllowedExtensions.Select(x => string.Join(", ", x)).ToArray();
                ImGui.SameLine();
                ImGui.PushItemWidth(-1);
                ImGui.Combo("##Extensions", ref CurrentExtensions, Exts, Exts.Length);
                ImGui.PopItemWidth();
            }
        }

        List<string> GetFileSystemEntries(string fullName)
        {
            var files = new List<string>();
            var dirs = new List<string>();

            foreach (var fse in Directory.GetFileSystemEntries(fullName, ""))
            {
                if (Directory.Exists(fse))
                {
                    dirs.Add(fse);
                }
                else if (!OnlyFolders)
                {
                    if (AllowedExtensions is not null)
                    {
                        var ext = Path.GetExtension(fse);
                        if (AllowedExtensions.ElementAt(CurrentExtensions).Contains(ext))
                            files.Add(fse);
                    }
                    else
                    {
                        files.Add(fse);
                    }
                }
            }

            var ret = new List<string>(dirs);
            ret.AddRange(files);

            return ret;
        }
    }
}