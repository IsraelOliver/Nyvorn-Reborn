namespace Nyvorn.Source.Gameplay.Combat.Weapons
{
    public sealed class ShortStickConfig
    {
        public int FrameWidth { get; init; } = 32;
        public int FrameHeight { get; init; } = 32;
        public int PivotX { get; init; } = 10;
        public int PivotY { get; init; } = 20;
        public int AttackHitboxWidth { get; init; } = 16;
        public int AttackHitboxHeight { get; init; } = 12;
        public int AttackHitboxOffsetX { get; init; } = 2;
    }
}
