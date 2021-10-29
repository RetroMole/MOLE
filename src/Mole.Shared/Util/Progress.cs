using System;

namespace Mole.Shared.Util
{
    public class Progress
    {
        public enum StateEnum
        {
            LoadingLevelPalInfo,
            LoadingTSpecPalInfo,
            LoadingGfx,
            LoadingExGfx,
            LoadingSuperExGfx,
            DecompressingGfx,
            DecompressingExGfx,
            DecompressingSuperExGfx,
            SavingProject,
            CopyingRom
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