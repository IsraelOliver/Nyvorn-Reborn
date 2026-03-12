using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nyvorn.Source.Game;
using System;

namespace Nyvorn.Source.Game.States
{
    public sealed class PauseState : IGameState
    {
        public bool UpdateBelow => false;
        public bool DrawBelow => true;
        public bool BlockInputBelow => true;

        private readonly GraphicsDevice graphicsDevice;
        private readonly ContentManager content;
        private readonly StateMachine stateMachine;
        private readonly Action restartAction;
        private readonly Action exitAction;
        private readonly VideoSettingsService videoSettings;
        private readonly SpriteFont font;
        private readonly Texture2D pixel;

        private KeyboardState previousKeyboard;
        private MouseState previousMouse;
        private int selectedIndex;

        private readonly string[] options =
        {
            "Recomecar",
            "Configuracoes",
            "Sair"
        };

        public PauseState(GraphicsDevice graphicsDevice, ContentManager content, StateMachine stateMachine, Action restartAction, VideoSettingsService videoSettings, Action exitAction)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.stateMachine = stateMachine;
            this.restartAction = restartAction;
            this.videoSettings = videoSettings;
            this.exitAction = exitAction;
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
                selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
            else if (IsKeyPressed(keyboard, Keys.Down))
                selectedIndex = (selectedIndex + 1) % options.Length;

            for (int i = 0; i < options.Length; i++)
            {
                if (GetButtonBounds(i).Contains(mouse.Position))
                    selectedIndex = i;
            }

            bool activateSelected = IsKeyPressed(keyboard, Keys.Enter) ||
                                    (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released);

            if (activateSelected)
                ActivateSelection(mouse.Position);

            previousKeyboard = keyboard;
            previousMouse = mouse;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(pixel, new Rectangle(0, 0, screenW, screenH), Color.Black * 0.55f);

            string title = "Pausado";
            Vector2 titleSize = font.MeasureString(title);
            spriteBatch.DrawString(font, title, new Vector2((screenW - titleSize.X) * 0.5f, (screenH * 0.5f) - 110f), Color.White);

            string hint = "ESC para continuar";
            Vector2 hintSize = font.MeasureString(hint);
            spriteBatch.DrawString(font, hint, new Vector2((screenW - hintSize.X) * 0.5f, (screenH * 0.5f) - 80f), new Color(210, 210, 210));

            for (int i = 0; i < options.Length; i++)
                DrawButton(spriteBatch, GetButtonBounds(i), options[i], i == selectedIndex);

            spriteBatch.End();
        }

        private void ActivateSelection(Point mousePosition)
        {
            int index = selectedIndex;
            for (int i = 0; i < options.Length; i++)
            {
                if (GetButtonBounds(i).Contains(mousePosition))
                {
                    index = i;
                    break;
                }
            }

            switch (index)
            {
                case 0:
                    restartAction?.Invoke();
                    break;
                case 1:
                    stateMachine.PushState(new SettingsState(graphicsDevice, content, stateMachine, videoSettings));
                    break;
                case 2:
                    exitAction?.Invoke();
                    break;
            }
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

        private Rectangle GetButtonBounds(int index)
        {
            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;
            return new Rectangle((screenW / 2) - 110, (screenH / 2) - 24 + (index * 52), 220, 40);
        }

        private bool IsKeyPressed(KeyboardState current, Keys key)
        {
            return current.IsKeyDown(key) && !previousKeyboard.IsKeyDown(key);
        }
    }
}
