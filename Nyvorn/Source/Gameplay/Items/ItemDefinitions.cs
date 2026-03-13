using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nyvorn.Source.Gameplay.Items
{
    public static class ItemDefinitions
    {
        private static readonly Dictionary<ItemId, ItemDefinition> definitions = new()
        {
            {
                ItemId.ShortStick,
                new ItemDefinition
                {
                    Id = ItemId.ShortStick,
                    Name = "Short Stick",
                    Description = "Um bastao curto e simples. Fraco, mas confiavel para o primeiro combate.",
                    TexturePath = "weapons/shortStick",
                    Category = ItemCategory.Weapon,
                    UseType = ItemUseType.Equipable,
                    Stackable = false,
                    MaxStack = 1,
                    Damage = 12,
                    Defense = 0,
                    HealAmount = 0,
                    EffectSummary = "Ataque corpo a corpo rapido e curto.",
                    GravityScale = 1.0f,
                    WorldSize = new Point(32, 32),
                    WorldPivot = new Point(10, 20),
                    SpriteSheetCell = new Point(0, 1),
                    SourceArea = Rectangle.Empty,
                    WorldCollisionRect = new Rectangle(6, 18, 20, 8)
                }
            },
            {
                ItemId.HealthPotion1,
                new ItemDefinition
                {
                    Id = ItemId.HealthPotion1,
                    Name = "Health Potion I",
                    Description = "Pocao basica de cura. Recupera uma parte da vida atual do guerreiro.",
                    TexturePath = "itens/health_potion",
                    Category = ItemCategory.Consumable,
                    UseType = ItemUseType.Consumable,
                    Stackable = true,
                    MaxStack = 10,
                    Damage = 0,
                    Defense = 0,
                    HealAmount = 20,
                    EffectSummary = "Restaura 20 de vida ao usar.",
                    GravityScale = 1.0f,
                    WorldSize = new Point(8, 8),
                    WorldPivot = new Point(4, 7),
                    SpriteSheetCell = Point.Zero,
                    SourceArea = new Rectangle(1, 0, 6, 7),
                    WorldCollisionRect = new Rectangle(1, 1, 6, 7)
                }
            }
        };

        public static ItemDefinition Get(ItemId id)
        {
            return definitions[id];
        }

        public static bool TryGet(ItemId id, out ItemDefinition definition)
        {
            return definitions.TryGetValue(id, out definition);
        }

        public static IReadOnlyCollection<ItemDefinition> GetAll()
        {
            return new ReadOnlyCollection<ItemDefinition>(new List<ItemDefinition>(definitions.Values));
        }
    }
}
