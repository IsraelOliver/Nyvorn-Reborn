using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nyvorn.Source.Game;

namespace Nyvorn.Source.Game.States
{
    public sealed class SettingsState : IGameState
    {
        public bool UpdateBelow => false;
        public bool DrawBelow => true;
        public bool BlockInputBelow => true;

        private readonly GraphicsDevice graphicsDevice;
        private readonly StateMachine stateMachine;
        private readonly VideoSettingsService videoSettings;
        private readonly SpriteFont font;
        private readonly Texture2D pixel;

        private KeyboardState previousKeyboard;
        private MouseState previousMouse;
        private int selectedIndex;

        public SettingsState(GraphicsDevice graphicsDevice, ContentManager content, StateMachine stateMachine, VideoSettingsService videoSettings)
        {
            this.graphicsDevice = graphicsDevice;
            this.stateMachine = stateMachine;
            this.videoSettings = videoSettings;
            font = content.Load<SpriteFont>("ui/UIFont");
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void OnEnter()
        {
            previousKeyboard = Keyboard.GetState();
            previousMouse = Mouse.GetState();
            selectedIndex = 0;
        }

        public void OnExit() { }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (IsKeyPressed(keyboard, Keys.Escape))
            {
                stateMachine.PopState();
                previousKeyboard = keyboard;
                previousMouse = mouse;
                return;
            }

            if (IsKeyPressed(keyboard, Keys.Up))
                selectedIndex = (selectedIndex - 1 + 3) % 3;
            else if (IsKeyPressed(keyboard, Keys.Down))
                selectedIndex = (selectedIndex + 1) % 3;

            if (GetResolutionRowBounds().Contains(mouse.Position))
                selectedIndex = 0;
            else if (GetFullscreenRowBounds().Contains(mouse.Position))
                selectedIndex = 1;
            else if (GetBackButtonBounds().Contains(mouse.Position))
                selectedIndex = 2;

            bool leftPressed = IsKeyPressed(keyboard, Keys.Left);
            bool rightPressed = IsKeyPressed(keyboard, Keys.Right);
            bool enterPressed = IsKeyPressed(keyboard, Keys.Enter);
            bool mouseClicked = mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released;

            if (selectedIndex == 0)
            {
                if (leftPressed)
                    videoSettings.CycleResolution(-1);
                else if (rightPressed)
                    videoSettings.CycleResolution(1);
            }

            if (enterPressed || mouseClicked)
                HandleActivation(mouse.Position);

            previousKeyboard = keyboard;
            previousMouse = mouse;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(pixel, new Rectangle(0, 0, screenW, screenH), Color.Black * 0.72f);

            string title = "Configuracoes";
            Vector2 titleSize = font.MeasureString(title);
            spriteBatch.DrawString(font, title, new Vector2((screenW - titleSize.X) * 0.5f, (screenH * 0.5f) - 120f), Color.White);

            DrawOptionRow(spriteBatch, GetResolutionRowBounds(), $"Resolucao: {videoSettings.CurrentResolutionLabel}", "Use <- e ->", selectedIndex == 0);
            DrawOptionRow(spriteBatch, GetFullscreenRowBounds(), $"Fullscreen: {videoSettings.FullscreenLabel}", "Enter para alternar", selectedIndex == 1);
            DrawButton(spriteBatch, GetBackButtonBounds(), "Voltar", selectedIndex == 2);

            spriteBatch.End();
        }

        private void HandleActivation(Point mousePosition)
        {
            if (GetResolutionRowBounds().Contains(mousePosition))
                selectedIndex = 0;
            else if (GetFullscreenRowBounds().Contains(mousePosition))
                selectedIndex = 1;
            else if (GetBackButtonBounds().Contains(mousePosition))
                selectedIndex = 2;

            switch (selectedIndex)
            {
                case 0:
                    videoSettings.CycleResolution(1);
                    break;
                case 1:
                    videoSettings.ToggleFullscreen();
                    break;
                case 2:
                    stateMachine.PopState();
                    break;
            }
        }

        private void DrawOptionRow(SpriteBatch spriteBatch, Rectangle bounds, string label, string hint, bool selected)
        {
            Color outer = selected ? new Color(212, 190, 108, 255) : new Color(18, 18, 18, 235);
            Color inner = selected ? new Color(96, 78, 30, 255) : new Color(60, 60, 60, 235);

            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4), outer);
            spriteBatch.Draw(pixel, bounds, inner);

            spriteBatch.DrawString(font, label, new Vector2(bounds.X + 12, bounds.Y + 9), Color.White);
            spriteBatch.DrawString(font, hint, new Vector2(bounds.X + 12, bounds.Y + 24), new Color(210, 210, 210));
        }

        private void DrawButton(SpriteBatch spriteBatch, Rectangle bounds, string text, bool selected)
        {
            Color outer = selected ? new Color(212, 190, 108, 255) : new Color(18, 18, 18, 235);
            Color inner = selected ? new Color(96, 78, 30, 255) : new Color(60, 60, 60, 235);

            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4), outer);
            spriteBatch.Draw(pixel, bounds, inner);

            Vector2 textSize = font.MeasureString(text);
            Vector2 textPos = new Vector2(bounds.X + (bounds.Width - textSize.X) * 0.5f, bounds.Y + (bounds.Height - textSize.Y) * 0.5f);
            spriteBatch.DrawString(font, text, textPos, Color.White);
        }

        private Rectangle GetResolutionRowBounds()
        {
            return GetPanelRowBounds(0);
        }

        private Rectangle GetFullscreenRowBounds()
        {
            return GetPanelRowBounds(1);
        }

        private Rectangle GetBackButtonBounds()
        {
            return GetPanelRowBounds(2);
        }

        private Rectangle GetPanelRowBounds(int index)
        {
            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;
            return new Rectangle((screenW / 2) - 150, (screenH / 2) - 34 + (index * 58), 300, 46);
        }

        private bool IsKeyPressed(KeyboardState current, Keys key)
        {
            return current.IsKeyDown(key) && !previousKeyboard.IsKeyDown(key);
        }
    }
}
