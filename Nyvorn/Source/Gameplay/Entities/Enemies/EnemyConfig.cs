namespace Nyvorn.Source.Gameplay.Entities.Enemies
{
    public sealed class EnemyConfig
    {
        public int MaxHealth { get; init; } = 500;
        public float BaseMoveSpeed { get; init; } = 58f;
        public float EnragedMoveSpeed { get; init; } = 84f;
        public float JumpSpeed { get; init; } = 250f;
        public float JumpInterceptCooldown { get; init; } = 0.9f;
        public float JumpInterceptRange { get; init; } = 76f;
        public float JumpInterceptHeightThreshold { get; init; } = 18f;
        public float DashTriggerMinDistance { get; init; } = 24f;
        public float DashTriggerMaxDistance { get; init; } = 88f;
        public float DashWindupDuration { get; init; } = 0.38f;
        public float DashDuration { get; init; } = 0.18f;
        public float DashRecoveryDuration { get; init; } = 0.48f;
        public float DashCooldown { get; init; } = 1.2f;
        public float EnragedDashDuration { get; init; } = 0.24f;
        public float EnragedDashCooldown { get; init; } = 0.8f;
        public float DashSpeed { get; init; } = 190f;
        public float EnragedDashSpeed { get; init; } = 250f;
        public int DashHitboxWidth { get; init; } = 16;
        public int DashHitboxHeight { get; init; } = 16;
        public int DashHitboxOffsetX { get; init; } = 14;
        public int DashHitboxOffsetY { get; init; } = 20;
        public float CloseAttackRange { get; init; } = 28f;
        public float CloseAttackWindupDuration { get; init; } = 0.18f;
        public float CloseAttackDuration { get; init; } = 0.16f;
        public float CloseAttackRecoveryDuration { get; init; } = 0.26f;
        public float CloseAttackCooldown { get; init; } = 0.75f;
        public float EnragedCloseAttackCooldown { get; init; } = 0.55f;
    }
}
