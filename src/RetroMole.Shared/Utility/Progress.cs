namespace RetroMole.Core.Utility
{
    public class Progress
    {
        public enum StateEnum
        {
            CopyingRom,
            LoadingRom,
            LoadingLevelPalInfo,
            LoadingTSpecPalInfo,
            LoadingOWPalInfo,
            LoadingGfx,
            LoadingExGfx,
            LoadingSuperExGfx,
            DecompressingGfx,
            DecompressingExGfx,
            DecompressingSuperExGfx,
            SavingProject
        }
        
        public int CurrentProgress = 0;
        public int MaxProgress = 0;
        public bool Loaded = false;
        public bool Working = false;
        public StateEnum State;
        public Exception Exception;
        public bool ShowException;
    }
}