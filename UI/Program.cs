namespace MOLE
{
    class Program
    {
        public static void Main(string[] args)
        {
            string g = args.Length >= 1 ? args[0] : "";
            UI UI = new();

            switch (g)
            {
                case "d":
                    _ = new VeldridController.Controller(Veldrid.GraphicsBackend.Direct3D11, UI);
                    break;
                case "v":
                    _ = new VeldridController.Controller(Veldrid.GraphicsBackend.Vulkan, UI);
                    break;
                case "m":
                    _ = new VeldridController.Controller(Veldrid.GraphicsBackend.Metal, UI);
                    break;
                case "g":
                default:
                    using (var game = new XNAController.Controller(UI)) game.Run();
                    break;
            }
        }
    }
}
