using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nyvorn.Source.Engine.Input;
using Nyvorn.Source.Game;
using System;

namespace Nyvorn.Source.Game.States
{
    public class PlayingState : IGameState
    {
        public bool UpdateBelow => false;
        public bool DrawBelow => false;
        public bool BlockInputBelow => true;

        private readonly GraphicsDevice graphicsDevice;
        private readonly StateMachine stateMachine;
        private readonly ContentManager content;
        private readonly VideoSettingsService videoSettings;
        private readonly Action exitGame;
        private readonly PlayingSession session;
        private readonly InputService inputService = new();
        private bool deathStatePushed;

        public PlayingState(GraphicsDevice graphicsDevice, ContentManager content, StateMachine stateMachine, VideoSettingsService videoSettings, Action exitGame)
            : this(graphicsDevice, content, stateMachine, videoSettings, exitGame, new PlayingSessionFactory(graphicsDevice, content).Create())
        {
        }

        public PlayingState(GraphicsDevice graphicsDevice, ContentManager content, StateMachine stateMachine, VideoSettingsService videoSettings, Action exitGame, PlayingSession session)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.stateMachine = stateMachine;
            this.videoSettings = videoSettings;
            this.exitGame = exitGame;
            this.session = session;
            deathStatePushed = false;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;

            InputState input = inputService.Update();
            if (input.OpenInventoryPressed && stateMachine.CurrentState is not InventoryState)
                stateMachine.PushState(new InventoryState(graphicsDevice, stateMachine, session));

            bool inventoryOpen = stateMachine.CurrentState is InventoryState;
            if (input.OpenPausePressed && !inventoryOpen && stateMachine.CurrentState == this)
            {
                stateMachine.PushState(new PauseState(graphicsDevice, content, stateMachine, RetrySession, videoSettings, exitGame));
                return;
            }

            if (stateMachine.CurrentState is InventoryState inventoryState &&
                inventoryState.ContainsMouse(Mouse.GetState().Position))
            {
                input = input.ConsumeWorldMouseInput();
            }

            Vector2 mouseWorld = session.Camera.ScreenToWorld(input.MouseScreenPosition);
            session.Update(dt, input, mouseWorld);

            if (!session.Player.IsAlive && !deathStatePushed)
            {
                deathStatePushed = true;
                stateMachine.PushState(new DeathState(graphicsDevice, content, RetryFromDeath));
                return;
            }

            session.Camera.Follow(session.Player.Position + new Vector2(8f, 12f), screenW, screenH);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: session.Camera.GetViewMatrix());
            session.DrawWorld(spriteBatch);
            spriteBatch.End();

            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            session.DrawHud(spriteBatch, screenW, screenH);
            spriteBatch.End();
        }

        private void RetryFromDeath()
        {
            RetrySession();
        }

        private void RetrySession()
        {
            stateMachine.Clear();
            stateMachine.PushState(new PlayingState(graphicsDevice, content, stateMachine, videoSettings, exitGame));
        }
    }
}
