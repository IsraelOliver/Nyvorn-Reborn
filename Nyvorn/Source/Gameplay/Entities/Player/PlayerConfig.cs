namespace Nyvorn.Source.Gameplay.Entities.Player
{
    public sealed class PlayerConfig
    {
        public float MoveSpeed { get; init; } = 90f;
        public float JumpSpeed { get; init; } = 280f;
        public float GravityScale { get; init; } = 1f;
        public float DodgeSpeed { get; init; } = 230f;
        public float DodgeFrameTime { get; init; } = 0.05f;
        public int DodgeFrames { get; init; } = 7;
        public float DodgeCooldown { get; init; } = 0.30f;
        public float AttackDuration { get; init; } = 0.3f;
        public float HurtCooldown { get; init; } = 0.35f;
        public int MaxHealth { get; init; } = 100;
    }
}
