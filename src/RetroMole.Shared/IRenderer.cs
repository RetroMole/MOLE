using ImGuiNET;

namespace RetroMole.Core
{
    public interface IRenderer
    {
        // Main application loop
        /// <summary>
        /// Starts your renderer
        /// </summary>
        public void Start(Action callback);


        // Textures
        /// <summary>
        /// List of bound textures
        /// </summary>
        public Dictionary<IntPtr, object> Textures { get; set; }

        /// <summary>
        /// ID of Font Atlas texture, should be 0 unless the atlas is rebuilt
        /// </summary>
        public IntPtr? FontTextureID { get; set; }

        /// <summary>
        /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="ImGui.Image" />. That pointer is then used by ImGui to let us know what texture to draw
        /// </summary>
        /// <param name="data">Texture data</param>
        /// <returns>Texture ID to use with ImGui</returns>
        public IntPtr BindTexture(uint[,] data);

        /// <summary>
        /// Update an existing texture pointer for ImGui
        /// </summary>
        /// <param name="ID">ID of the texture to update</param>
        /// <param name="data">New texture data to replace the old with</param>
        public void UpdateTexture(IntPtr ID, uint[,] data);

        /// <summary>
        /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
        /// </summary>
        /// <param name="ID">ImGui Texture ID</param>
        public void UnbindTexture(IntPtr ID);



        // Rendering
        /// <summary>
        /// Sets up ImGui for a new frame, should be called at frame start
        /// </summary>
        public void BeforeLayout();

        /// <summary>
        /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls
        /// </summary>
        public void AfterLayout();

        /// <summary>
        /// Gets the geometry as set up by ImGui and sends it to the graphics device
        /// </summary>
        /// <param name="drawData">ImGui draw data from ImGui.Render()</param>
        public void RenderDrawData(ImDrawDataPtr drawData);



        // IO
        /// <summary>
        /// Hook raw inputs to ImGui
        /// </summary>
        public void SetupInput();

        /// <summary>
        /// Update inputs for ImGui, should be called from BeforeLayout()
        /// </summary>
        public void UpdateInput();
    }
}