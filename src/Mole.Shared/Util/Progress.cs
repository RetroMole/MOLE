namespace Mole.Shared.Util
{
    public class Progress
    {
        public enum StateEnum
        {
            LoadingGfx,
            LoadingExGfx,
            LoadingSuperExGfx,
            DecompressingGfx,
            CleaningUpGfx,
            DecompressingExGfx,
            CleaningUpExGfx,
            DecompressingSuperExGfx,
            CleaningUpSuperExGfx,
            SavingProject,
            CopyingRom
        }
        
        public int CurrentProgress = 0;
        public int MaxProgress = 0;
        public bool Loaded = false;
        public StateEnum State;
    }
}