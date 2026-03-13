using Microsoft.Xna.Framework;

namespace Nyvorn.Source.Gameplay.Items
{
    public sealed class ItemDefinition
    {
        public required ItemId Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string TexturePath { get; init; }
        public required ItemCategory Category { get; init; }
        public required ItemUseType UseType { get; init; }
        public required bool Stackable { get; init; }
        public required int MaxStack { get; init; }
        public int Damage { get; init; }
        public int Defense { get; init; }
        public int HealAmount { get; init; }
        public string EffectSummary { get; init; } = string.Empty;
        public required float GravityScale { get; init; }
        public required Point WorldSize { get; init; }
        public required Point WorldPivot { get; init; }
        public required Point SpriteSheetCell { get; init; }
        public Rectangle SourceArea { get; init; }
        public required Rectangle WorldCollisionRect { get; init; }

        public Rectangle SourceRectangle =>
            SourceArea.Width > 0 && SourceArea.Height > 0
                ? SourceArea
                : new Rectangle(
                    SpriteSheetCell.X * WorldSize.X,
                    SpriteSheetCell.Y * WorldSize.Y,
                    WorldSize.X,
                    WorldSize.Y);

        public bool HasDamage => Damage > 0;
        public bool HasDefense => Defense > 0;
        public bool HasHealing => HealAmount > 0;
        public bool HasEffectSummary => !string.IsNullOrWhiteSpace(EffectSummary);
    }
}
