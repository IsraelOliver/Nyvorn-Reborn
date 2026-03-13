using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nyvorn.Source.Game;
using Nyvorn.Source.Gameplay.Items;

namespace Nyvorn.Source.Game.States
{
    public sealed class InventoryState : IGameState
    {
        public bool UpdateBelow => true;
        public bool DrawBelow => true;
        public bool BlockInputBelow => false;

        private readonly GraphicsDevice graphicsDevice;
        private readonly StateMachine stateMachine;
        private readonly PlayingSession session;
        private readonly Texture2D pixel;
        private KeyboardState previousKeyboard;
        private MouseState previousMouse;
        private readonly InventorySlot heldSlot = new InventorySlot();
        private bool heldSlotOriginIsHotbar;
        private int heldSlotOriginIndex = -1;

        public InventoryState(GraphicsDevice graphicsDevice, StateMachine stateMachine, PlayingSession session)
        {
            this.graphicsDevice = graphicsDevice;
            this.stateMachine = stateMachine;
            this.session = session;
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void OnEnter()
        {
            previousKeyboard = Keyboard.GetState();
            previousMouse = Mouse.GetState();
        }

        public void OnExit()
        {
            ReturnHeldItem();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            bool closePressed = (keyboard.IsKeyDown(Keys.E) && !previousKeyboard.IsKeyDown(Keys.E)) ||
                                (keyboard.IsKeyDown(Keys.Escape) && !previousKeyboard.IsKeyDown(Keys.Escape));
            bool dropPressed = keyboard.IsKeyDown(Keys.T) && !previousKeyboard.IsKeyDown(Keys.T);

            if (closePressed)
            {
                ReturnHeldItem();
                stateMachine.PopState();
            }

            bool clickPressed = mouse.LeftButton == ButtonState.Pressed &&
                               previousMouse.LeftButton == ButtonState.Released;

            if (clickPressed)
                TryHandleSlotClick(mouse.Position);

            if (dropPressed)
                TryDropItem(mouse.Position);

            previousKeyboard = keyboard;
            previousMouse = mouse;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(pixel, new Rectangle(0, 0, screenW, screenH), Color.Black * 0.2f);
            session.DrawInventory(spriteBatch, screenW, screenH);
            DrawHeldItem(spriteBatch, Mouse.GetState().Position);
            spriteBatch.End();
        }

        public bool ContainsMouse(Point mousePosition)
        {
            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;
            return session.GetInventoryPanelBounds(screenW, screenH).Contains(mousePosition);
        }

        private void TryHandleSlotClick(Point mousePosition)
        {
            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;

            if (!session.HudRenderer.TryGetSlotAtPoint(session.Hotbar, session.Inventory, screenW, screenH, mousePosition, out bool isHotbar, out int slotIndex))
            {
                if (!heldSlot.IsEmpty && !ContainsMouse(mousePosition) && session.TryDropItem(heldSlot.ItemId))
                    heldSlot.RemoveOne();

                return;
            }

            InventorySlot clickedSlot = isHotbar ? session.Hotbar.GetSlot(slotIndex) : session.Inventory.GetSlot(slotIndex);

            if (heldSlot.IsEmpty)
            {
                RememberHeldOrigin(isHotbar, slotIndex);
                heldSlot.CopyFrom(clickedSlot);
                clickedSlot.Clear();
                return;
            }

            if (clickedSlot.IsEmpty)
            {
                clickedSlot.CopyFrom(heldSlot);
                heldSlot.Clear();
                ClearHeldOrigin();
                return;
            }

            if (heldSlot.ItemId == clickedSlot.ItemId && ItemDefinitions.TryGet(heldSlot.ItemId, out ItemDefinition definition) && definition.Stackable)
            {
                int added = clickedSlot.Add(definition, heldSlot.Quantity);
                if (added > 0)
                    heldSlot.Set(heldSlot.ItemId, heldSlot.Quantity - added);

                if (heldSlot.IsEmpty)
                    ClearHeldOrigin();

                return;
            }

            InventorySlot temp = clickedSlot.Clone();
            clickedSlot.CopyFrom(heldSlot);
            heldSlot.CopyFrom(temp);
            RememberHeldOrigin(isHotbar, slotIndex);
        }

        private void DrawHeldItem(SpriteBatch spriteBatch, Point mousePosition)
        {
            if (heldSlot.IsEmpty || !ItemDefinitions.TryGet(heldSlot.ItemId, out ItemDefinition definition) || !session.TryGetItemTexture(heldSlot.ItemId, out Texture2D itemTexture))
                return;

            Rectangle iconRect = new Rectangle(mousePosition.X - 16, mousePosition.Y - 16, 32, 32);
            spriteBatch.Draw(itemTexture, iconRect, definition.SourceRectangle, Color.White);
        }

        private void TryDropItem(Point mousePosition)
        {
            if (!heldSlot.IsEmpty)
            {
                if (session.TryDropItem(heldSlot.ItemId))
                    heldSlot.RemoveOne();

                return;
            }

            int screenW = graphicsDevice.PresentationParameters.BackBufferWidth;
            int screenH = graphicsDevice.PresentationParameters.BackBufferHeight;

            if (!session.HudRenderer.TryGetSlotAtPoint(session.Hotbar, session.Inventory, screenW, screenH, mousePosition, out bool isHotbar, out int slotIndex))
                return;

            InventorySlot slot = isHotbar ? session.Hotbar.GetSlot(slotIndex) : session.Inventory.GetSlot(slotIndex);
            if (slot.IsEmpty)
                return;

            if (session.TryDropItem(slot.ItemId))
                slot.RemoveOne();
        }

        private void ReturnHeldItem()
        {
            if (heldSlot.IsEmpty)
                return;

            if (TryReturnHeldItemToOrigin())
            {
                heldSlot.Clear();
                ClearHeldOrigin();
                return;
            }

            if (session.TryStoreItem(heldSlot.ItemId, heldSlot.Quantity, preferInventory: true))
            {
                heldSlot.Clear();
                ClearHeldOrigin();
            }
        }

        private bool TryReturnHeldItemToOrigin()
        {
            if (heldSlotOriginIndex < 0)
                return false;

            InventorySlot targetSlot = heldSlotOriginIsHotbar
                ? session.Hotbar.GetSlot(heldSlotOriginIndex)
                : session.Inventory.GetSlot(heldSlotOriginIndex);

            if (!targetSlot.IsEmpty)
                return false;

            targetSlot.CopyFrom(heldSlot);
            return true;
        }

        private void RememberHeldOrigin(bool isHotbar, int slotIndex)
        {
            heldSlotOriginIsHotbar = isHotbar;
            heldSlotOriginIndex = slotIndex;
        }

        private void ClearHeldOrigin()
        {
            heldSlotOriginIsHotbar = false;
            heldSlotOriginIndex = -1;
        }
    }
}
