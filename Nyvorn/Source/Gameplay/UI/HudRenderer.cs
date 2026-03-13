using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nyvorn.Source.Gameplay.Items;
using System.Collections.Generic;
using System.Text;

namespace Nyvorn.Source.Gameplay.UI
{
    public sealed class HudRenderer
    {
        public const int SlotSize = 36;
        public const int SlotGap = 6;
        private const float InventoryTextScale = 0.8f;

        private readonly Texture2D pixel;
        private readonly SpriteFont font;
        private readonly IReadOnlyDictionary<ItemId, Texture2D> itemTextures;

        public HudRenderer(GraphicsDevice graphicsDevice, SpriteFont font, IReadOnlyDictionary<ItemId, Texture2D> itemTextures)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            this.font = font;
            this.itemTextures = itemTextures;
        }

        public void Draw(SpriteBatch spriteBatch, Hotbar hotbar, int selectedHotbarIndex, int currentHealth, int maxHealth, int screenWidth)
        {
            DrawHotbar(spriteBatch, hotbar, selectedHotbarIndex);
            DrawPlayerHealth(spriteBatch, currentHealth, maxHealth, screenWidth);
        }

        public void DrawBossBar(SpriteBatch spriteBatch, string bossName, int currentHealth, int maxHealth, int screenWidth, int screenHeight)
        {
            const int width = 360;
            const int height = 18;
            int x = (screenWidth - width) / 2;
            int y = screenHeight - 42;

            float ratio = maxHealth <= 0 ? 0f : MathHelper.Clamp((float)currentHealth / maxHealth, 0f, 1f);
            int fill = (int)(width * ratio);

            spriteBatch.Draw(pixel, new Rectangle(x - 3, y - 3, width + 6, height + 6), Color.Black * 0.9f);
            spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), new Color(48, 48, 48, 240));
            if (fill > 0)
                spriteBatch.Draw(pixel, new Rectangle(x, y, fill, height), new Color(148, 28, 36));

            Vector2 titleSize = font.MeasureString(bossName);
            Vector2 titlePos = new Vector2(x + (width - titleSize.X) * 0.5f, y - titleSize.Y - 4f);
            spriteBatch.DrawString(font, bossName, titlePos + new Vector2(1f, 1f), Color.Black);
            spriteBatch.DrawString(font, bossName, titlePos, Color.White);

            string value = $"{currentHealth}/{maxHealth}";
            Vector2 valueSize = font.MeasureString(value);
            Vector2 valuePos = new Vector2(x + (width - valueSize.X) * 0.5f, y + (height - valueSize.Y) * 0.5f - 1f);
            spriteBatch.DrawString(font, value, valuePos + new Vector2(1f, 1f), Color.Black);
            spriteBatch.DrawString(font, value, valuePos, Color.White);
        }

        public void DrawEncounterBanner(SpriteBatch spriteBatch, string text, int screenWidth, int screenHeight, float alpha)
        {
            if (alpha <= 0f)
                return;

            Vector2 textSize = font.MeasureString(text);
            Rectangle panel = new Rectangle(
                (int)((screenWidth - (textSize.X + 40f)) * 0.5f),
                (int)(screenHeight * 0.18f),
                (int)(textSize.X + 40f),
                (int)(textSize.Y + 18f));

            spriteBatch.Draw(pixel, panel, Color.Black * (0.65f * alpha));
            spriteBatch.Draw(pixel, new Rectangle(panel.X - 2, panel.Y - 2, panel.Width + 4, panel.Height + 4), new Color(212, 190, 108) * alpha);
            Vector2 textPos = new Vector2(panel.X + (panel.Width - textSize.X) * 0.5f, panel.Y + (panel.Height - textSize.Y) * 0.5f);
            spriteBatch.DrawString(font, text, textPos + new Vector2(1f, 1f), Color.Black * alpha);
            spriteBatch.DrawString(font, text, textPos, Color.White * alpha);
        }

        public Rectangle GetInventoryPanelBounds(int screenWidth, int screenHeight)
        {
            const int hotbarWidth = (4 * SlotSize) + (3 * SlotGap);
            const int panelWidth = hotbarWidth + 220;
            const int panelHeight = 170;
            return new Rectangle(
                (screenWidth - panelWidth) / 2,
                screenHeight - panelHeight - 72,
                panelWidth,
                panelHeight);
        }

        public void DrawInventoryPanel(SpriteBatch spriteBatch, Hotbar hotbar, Inventory inventory, int selectedHotbarIndex, int screenWidth, int screenHeight)
        {
            Rectangle panel = GetInventoryPanelBounds(screenWidth, screenHeight);
            spriteBatch.Draw(pixel, panel, new Color(12, 12, 12, 220));
            spriteBatch.Draw(pixel, new Rectangle(panel.X - 2, panel.Y - 2, panel.Width + 4, panel.Height + 4), Color.Black * 0.9f);
            DrawInventoryText(spriteBatch, "Loadout", new Vector2(panel.X + 12, panel.Y + 8), Color.White);

            int hotbarStartX = panel.X + 12;
            int hotbarStartY = panel.Y + 34;
            DrawSlots(spriteBatch, hotbar.Slots, hotbarStartX, hotbarStartY, hotbar.Capacity, 1, selectedHotbarIndex, true);

            int backpackStartY = hotbarStartY + SlotSize + 16;
            DrawInventoryText(spriteBatch, "Mochila", new Vector2(panel.X + 12, backpackStartY - 16), new Color(210, 210, 210));
            DrawSlots(spriteBatch, inventory.Slots, hotbarStartX, backpackStartY, inventory.Capacity, 1);

            InventorySlot detailSlot = GetDetailSlot(hotbar, inventory, selectedHotbarIndex);
            DrawDetailPanel(spriteBatch, detailSlot, panel.Right - 196, panel.Y + 12, 180, panel.Height - 24);
        }

        public bool TryGetSlotAtPoint(Hotbar hotbar, Inventory inventory, int screenWidth, int screenHeight, Point point, out bool isHotbar, out int slotIndex)
        {
            Rectangle panel = GetInventoryPanelBounds(screenWidth, screenHeight);
            int hotbarStartX = panel.X + 12;
            int hotbarStartY = panel.Y + 34;

            for (int i = 0; i < hotbar.Capacity; i++)
            {
                Rectangle bounds = GetSlotBounds(hotbarStartX, hotbarStartY, hotbar.Capacity, i);
                if (bounds.Contains(point))
                {
                    isHotbar = true;
                    slotIndex = i;
                    return true;
                }
            }

            for (int i = 0; i < inventory.Capacity; i++)
            {
                Rectangle bounds = GetSlotBounds(panel.X + 12, panel.Y + 34 + SlotSize + 16, inventory.Capacity, i);
                if (bounds.Contains(point))
                {
                    isHotbar = false;
                    slotIndex = i;
                    return true;
                }
            }

            isHotbar = false;
            slotIndex = -1;
            return false;
        }

        private void DrawHotbar(SpriteBatch spriteBatch, Hotbar hotbar, int selectedHotbarIndex)
        {
            const int padding = 14;
            int startX = padding;
            int y = padding;

            for (int i = 0; i < hotbar.Capacity; i++)
            {
                int x = startX + i * (SlotSize + SlotGap);
                DrawSlot(spriteBatch, hotbar.Slots[i], x, y, i == selectedHotbarIndex);
            }
        }

        private void DrawSlots(SpriteBatch spriteBatch, IReadOnlyList<InventorySlot> slots, int startX, int startY, int columns, int rows)
        {
            DrawSlots(spriteBatch, slots, startX, startY, columns, rows, -1);
        }

        private void DrawSlots(SpriteBatch spriteBatch, IReadOnlyList<InventorySlot> slots, int startX, int startY, int columns, int rows, int selectedIndex)
        {
            DrawSlots(spriteBatch, slots, startX, startY, columns, rows, selectedIndex, false);
        }

        private void DrawSlots(SpriteBatch spriteBatch, IReadOnlyList<InventorySlot> slots, int startX, int startY, int columns, int rows, int selectedIndex, bool drawKeyLabels)
        {
            for (int i = 0; i < slots.Count && i < columns * rows; i++)
            {
                Rectangle slotBounds = GetSlotBounds(startX, startY, columns, i);
                int x = slotBounds.X;
                int y = slotBounds.Y;
                DrawSlot(spriteBatch, slots[i], x, y, i == selectedIndex);

                if (drawKeyLabels)
                {
                    string keyLabel = (i + 1).ToString();
                    Vector2 labelPos = new Vector2(x + 4f, y + 2f);
                    DrawInventoryText(spriteBatch, keyLabel, labelPos + new Vector2(1f, 1f), Color.Black);
                    DrawInventoryText(spriteBatch, keyLabel, labelPos, new Color(210, 210, 210));
                }
            }
        }

        private void DrawSlot(SpriteBatch spriteBatch, InventorySlot slot, int x, int y, bool selected)
        {
            Rectangle outer = new Rectangle(x, y, SlotSize, SlotSize);
            Rectangle inner = new Rectangle(x + 2, y + 2, SlotSize - 4, SlotSize - 4);

            spriteBatch.Draw(pixel, outer, selected ? new Color(212, 190, 108, 240) : new Color(18, 18, 18, 220));
            spriteBatch.Draw(pixel, inner, new Color(66, 66, 66, 220));

            if (slot.IsEmpty || !itemTextures.TryGetValue(slot.ItemId, out Texture2D itemTexture) || !ItemDefinitions.TryGet(slot.ItemId, out ItemDefinition definition))
                return;

            int iconSize = definition.Category == ItemCategory.Consumable ? 18 : 32;
            int iconX = x + (SlotSize - iconSize) / 2;
            int iconY = y + (SlotSize - iconSize) / 2;
            Rectangle iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
            spriteBatch.Draw(itemTexture, iconRect, definition.SourceRectangle, Color.White);

            if (slot.Quantity > 1)
            {
                string text = slot.Quantity.ToString();
                Vector2 textSize = font.MeasureString(text);
                Vector2 textPos = new Vector2(x + SlotSize - textSize.X - 3f, y + SlotSize - textSize.Y - 2f);
                spriteBatch.DrawString(font, text, textPos + new Vector2(1f, 1f), Color.Black);
                spriteBatch.DrawString(font, text, textPos, Color.White);
            }
        }

        private Rectangle GetSlotBounds(int startX, int startY, int columns, int slotIndex)
        {
            int column = slotIndex % columns;
            int row = slotIndex / columns;
            int x = startX + column * (SlotSize + SlotGap);
            int y = startY + row * (SlotSize + SlotGap);
            return new Rectangle(x, y, SlotSize, SlotSize);
        }

        private void DrawPlayerHealth(SpriteBatch spriteBatch, int currentHealth, int maxHealth, int screenWidth)
        {
            const int width = 120;
            const int height = 14;
            const int padding = 14;

            float ratio = maxHealth <= 0 ? 0f : MathHelper.Clamp((float)currentHealth / maxHealth, 0f, 1f);
            int fill = (int)(width * ratio);
            int x = screenWidth - width - padding;
            int y = padding;

            spriteBatch.Draw(pixel, new Rectangle(x - 2, y - 2, width + 4, height + 4), Color.Black * 0.85f);
            spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), new Color(52, 52, 52));
            if (fill > 0)
                spriteBatch.Draw(pixel, new Rectangle(x, y, fill, height), new Color(196, 44, 56));

            string label = $"{currentHealth}/{maxHealth}";
            Vector2 size = font.MeasureString(label);
            Vector2 textPos = new Vector2(x + (width - size.X) * 0.5f, y + height + 2f);
            spriteBatch.DrawString(font, label, textPos + new Vector2(1f, 1f), Color.Black);
            spriteBatch.DrawString(font, label, textPos, Color.White);
        }

        private InventorySlot GetDetailSlot(Hotbar hotbar, Inventory inventory, int selectedHotbarIndex)
        {
            if (selectedHotbarIndex >= 0 && selectedHotbarIndex < hotbar.Capacity)
            {
                InventorySlot selected = hotbar.GetSlot(selectedHotbarIndex);
                if (!selected.IsEmpty)
                    return selected;
            }

            for (int i = 0; i < inventory.Capacity; i++)
            {
                InventorySlot slot = inventory.GetSlot(i);
                if (!slot.IsEmpty)
                    return slot;
            }

            for (int i = 0; i < hotbar.Capacity; i++)
            {
                InventorySlot slot = hotbar.GetSlot(i);
                if (!slot.IsEmpty)
                    return slot;
            }

            return null;
        }

        private void DrawDetailPanel(SpriteBatch spriteBatch, InventorySlot slot, int x, int y, int width, int height)
        {
            Rectangle panel = new Rectangle(x, y, width, height);
            spriteBatch.Draw(pixel, panel, new Color(28, 28, 28, 220));
            spriteBatch.Draw(pixel, new Rectangle(panel.X - 2, panel.Y - 2, panel.Width + 4, panel.Height + 4), new Color(18, 18, 18, 235));

            const int textPadding = 10;
            float contentWidth = panel.Width - (textPadding * 2);
            DrawInventoryText(spriteBatch, "Detalhes", new Vector2(panel.X + textPadding, panel.Y + 8), Color.White);

            if (slot == null || slot.IsEmpty || !ItemDefinitions.TryGet(slot.ItemId, out ItemDefinition definition))
            {
                DrawInventoryText(spriteBatch, "Nenhum item", new Vector2(panel.X + textPadding, panel.Y + 34), new Color(210, 210, 210));
                return;
            }

            float textY = panel.Y + 34;
            Vector2 linePosition = new Vector2(panel.X + textPadding, textY);

            DrawInventoryText(spriteBatch, definition.Name, linePosition, Color.White);
            textY += 16f;

            textY += DrawWrappedInventoryText(spriteBatch, definition.Description, new Vector2(panel.X + textPadding, textY), contentWidth, new Color(210, 210, 210));
            DrawInventoryText(spriteBatch, $"Categoria: {FormatLabel(definition.Category)}", new Vector2(panel.X + textPadding, textY), new Color(210, 210, 210));
            textY += 15f;
            DrawInventoryText(spriteBatch, $"Uso: {FormatLabel(definition.UseType)}", new Vector2(panel.X + textPadding, textY), new Color(210, 210, 210));
            textY += 15f;
            DrawInventoryText(spriteBatch, $"Qtd: {slot.Quantity}", new Vector2(panel.X + textPadding, textY), new Color(210, 210, 210));
            textY += 15f;
            DrawInventoryText(spriteBatch, definition.Stackable ? $"Pilha: {definition.MaxStack}" : "Pilha: 1", new Vector2(panel.X + textPadding, textY), new Color(210, 210, 210));
            textY += 18f;

            if (definition.HasDamage)
            {
                DrawInventoryText(spriteBatch, $"Dano: {definition.Damage}", new Vector2(panel.X + textPadding, textY), Color.White);
                textY += 15f;
            }

            if (definition.HasDefense)
            {
                DrawInventoryText(spriteBatch, $"Defesa: {definition.Defense}", new Vector2(panel.X + textPadding, textY), Color.White);
                textY += 15f;
            }

            if (definition.HasHealing)
            {
                DrawInventoryText(spriteBatch, $"Cura: {definition.HealAmount}", new Vector2(panel.X + textPadding, textY), Color.White);
                textY += 15f;
            }

            if (definition.HasEffectSummary)
            {
                textY += DrawWrappedInventoryText(spriteBatch, $"Efeito: {definition.EffectSummary}", new Vector2(panel.X + textPadding, textY), contentWidth, new Color(210, 210, 210));
            }

            DrawInventoryText(spriteBatch, "E para organizar.", new Vector2(panel.X + textPadding, panel.Bottom - 30), new Color(210, 210, 210));
            DrawInventoryText(spriteBatch, "T para dropar.", new Vector2(panel.X + textPadding, panel.Bottom - 16), new Color(210, 210, 210));
        }

        private void DrawInventoryText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, text, position, color, 0f, Vector2.Zero, InventoryTextScale, SpriteEffects.None, 0f);
        }

        private float DrawWrappedInventoryText(SpriteBatch spriteBatch, string text, Vector2 position, float maxWidth, Color color)
        {
            string wrapped = WrapInventoryText(text, maxWidth);
            DrawInventoryText(spriteBatch, wrapped, position, color);
            return MeasureWrappedInventoryTextHeight(wrapped) + 6f;
        }

        private string WrapInventoryText(string text, float maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string[] words = text.Split(' ');
            StringBuilder builder = new();
            StringBuilder line = new();

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                string candidate = line.Length == 0 ? word : $"{line} {word}";
                if (MeasureInventoryTextWidth(candidate) <= maxWidth)
                {
                    line.Clear();
                    line.Append(candidate);
                    continue;
                }

                if (builder.Length > 0)
                    builder.Append('\n');

                builder.Append(line);
                line.Clear();
                line.Append(word);
            }

            if (line.Length > 0)
            {
                if (builder.Length > 0)
                    builder.Append('\n');

                builder.Append(line);
            }

            return builder.ToString();
        }

        private float MeasureWrappedInventoryTextHeight(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;

            int lineCount = 1;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                    lineCount++;
            }

            return lineCount * (font.LineSpacing * InventoryTextScale);
        }

        private float MeasureInventoryTextWidth(string text)
        {
            return font.MeasureString(text).X * InventoryTextScale;
        }

        private static string FormatLabel(ItemCategory category)
        {
            return category switch
            {
                ItemCategory.Weapon => "Arma",
                ItemCategory.Shield => "Escudo",
                ItemCategory.Armor => "Armadura",
                ItemCategory.Consumable => "Consumivel",
                ItemCategory.Utility => "Utilitario",
                _ => "Diverso"
            };
        }

        private static string FormatLabel(ItemUseType useType)
        {
            return useType switch
            {
                ItemUseType.Equipable => "Equipavel",
                ItemUseType.Consumable => "Consumivel",
                _ => "Passivo"
            };
        }
    }
}
