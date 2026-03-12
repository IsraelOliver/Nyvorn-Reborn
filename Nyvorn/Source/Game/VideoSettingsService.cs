using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Nyvorn.Source.Game
{
    public sealed class VideoSettingsService
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly List<Point> resolutions;
        private int selectedResolutionIndex;

        public VideoSettingsService(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            resolutions = new List<Point>
            {
                new Point(1280, 720),
                new Point(1600, 900),
                new Point(1920, 1080)
            };

            Point current = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            selectedResolutionIndex = resolutions.FindIndex(point => point == current);
            if (selectedResolutionIndex < 0)
            {
                resolutions.Insert(0, current);
                selectedResolutionIndex = 0;
            }
        }

        public Point CurrentResolution => resolutions[selectedResolutionIndex];
        public bool IsFullscreen => graphics.IsFullScreen;

        public string CurrentResolutionLabel => $"{CurrentResolution.X}x{CurrentResolution.Y}";
        public string FullscreenLabel => IsFullscreen ? "Ligado" : "Desligado";

        public void CycleResolution(int direction)
        {
            if (resolutions.Count == 0)
                return;

            selectedResolutionIndex += direction;
            if (selectedResolutionIndex < 0)
                selectedResolutionIndex = resolutions.Count - 1;
            else if (selectedResolutionIndex >= resolutions.Count)
                selectedResolutionIndex = 0;

            Apply();
        }

        public void ToggleFullscreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            Apply();
        }

        private void Apply()
        {
            Point resolution = CurrentResolution;
            graphics.PreferredBackBufferWidth = resolution.X;
            graphics.PreferredBackBufferHeight = resolution.Y;
            graphics.ApplyChanges();
        }
    }
}
