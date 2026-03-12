using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nyvorn.Source.Gameplay.Combat.Interfaces;
using Nyvorn.Source.Gameplay.Entities.Enemies.EnemyAnimations;
using Nyvorn.Source.World;
using System;

namespace Nyvorn.Source.Gameplay.Entities.Enemies
{
    public class Enemy : IDamageable, IHitSource
    {
        private enum BossState : byte
        {
            Chase = 0,
            DashWindup = 1,
            Dash = 2,
            DashRecovery = 3,
            CloseAttackWindup = 4,
            CloseAttack = 5,
            CloseAttackRecovery = 6
        }

        private readonly EnemyConfig config;
        private readonly Texture2D texture;
        private readonly EnemyAnimator animator;
        private readonly EnemyCombat combat;
        private readonly EnemyMotor motor;
        private bool facingRight;
        private float jumpInterceptCooldownTimer;
        private float dashCooldownTimer;
        private float closeAttackCooldownTimer;
        private float stateTimer;
        private int attackDirection;
        private BossState state;

        private const int FrameW = 32;
        private const int FrameH = 32;

        public Vector2 Position => motor.Position;
        public bool IsAlive => combat.IsAlive;
        public bool IsActive { get; private set; }
        bool IHitSource.HasActiveHitbox => IsAlive && IsActive && (state == BossState.Dash || state == BossState.CloseAttack);
        Rectangle IHitSource.ActiveHitbox => GetActiveHitbox();
        int IHitSource.HitSequence => 0;
        public int Health => combat.Health;
        public int MaxHealth => combat.MaxHealth;

        public Rectangle Hurtbox => motor.Hurtbox;

        public bool TryReceiveHit(Rectangle hitbox, int hitSequence, int damage)
        {
            return combat.TryReceiveHit(Hurtbox, hitbox, hitSequence, damage);
        }

        public Enemy(Texture2D texture, Vector2 position, EnemyConfig config)
        {
            this.config = config;
            this.texture = texture;
            motor = new EnemyMotor(position, config);
            combat = new EnemyCombat(config.MaxHealth);
            facingRight = false;
            jumpInterceptCooldownTimer = 0f;
            dashCooldownTimer = 0f;
            closeAttackCooldownTimer = 0f;
            stateTimer = 0f;
            attackDirection = -1;
            state = BossState.Chase;
            animator = new EnemyAnimator(EnemyTestAnimations.Create(), EnemyAnimState.Idle);
        }

        public void Update(float dt, WorldMap worldMap, Vector2 playerPosition, bool isActive)
        {
            IsActive = isActive;
            combat.Tick(dt);

            if (jumpInterceptCooldownTimer > 0f)
                jumpInterceptCooldownTimer -= dt;
            if (dashCooldownTimer > 0f)
                dashCooldownTimer -= dt;
            if (closeAttackCooldownTimer > 0f)
                closeAttackCooldownTimer -= dt;

            bool enraged = Health <= (MaxHealth / 2);
            float desiredVelocityX = ResolveState(dt, playerPosition, enraged);
            motor.Update(dt, worldMap, desiredVelocityX);

            EnemyAnimState animState = ResolveAnimState();
            animator.Play(animState);
            animator.Update(dt);
        }

        public void ApplyKnockback(float forceX, float forceY = -55f)
        {
            motor.ApplyKnockback(forceX, forceY);
        }

        public void TriggerAttackVisual(float duration = 0.12f)
        {
            combat.TriggerAttackVisual(duration);
        }

        void IHitSource.OnHitConnected()
        {
            TriggerAttackVisual();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsAlive)
                return;

            Rectangle src = animator.CurrentFrame;
            if (src == Rectangle.Empty)
                src = new Rectangle(0, 1 * FrameH, FrameW, FrameH);

            Vector2 drawPosition = GetDrawPosition();
            Vector2 origin = new Vector2(16f, 32f);
            SpriteEffects effects = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, drawPosition, src, Color.White, 0f, origin, 1f, effects, 0f);
        }

        private float ResolveState(float dt, Vector2 playerPosition, bool enraged)
        {
            if (!IsActive)
            {
                state = BossState.Chase;
                stateTimer = 0f;
                return 0f;
            }

            if (ShouldJumpToIntercept(playerPosition) && state != BossState.Dash && state != BossState.CloseAttack)
            {
                motor.TryJump();
                jumpInterceptCooldownTimer = config.JumpInterceptCooldown;
            }

            switch (state)
            {
                case BossState.DashWindup:
                    stateTimer -= dt;
                    if (stateTimer <= 0f)
                    {
                        state = BossState.Dash;
                        stateTimer = enraged ? config.EnragedDashDuration : config.DashDuration;
                        TriggerAttackVisual(stateTimer);
                    }
                    return 0f;

                case BossState.Dash:
                    stateTimer -= dt;
                    if (stateTimer <= 0f)
                    {
                        state = BossState.DashRecovery;
                        stateTimer = config.DashRecoveryDuration;
                        dashCooldownTimer = enraged ? config.EnragedDashCooldown : config.DashCooldown;
                        return 0f;
                    }

                    float dashSpeed = enraged ? config.EnragedDashSpeed : config.DashSpeed;
                    return attackDirection * dashSpeed;

                case BossState.DashRecovery:
                    stateTimer -= dt;
                    if (stateTimer <= 0f)
                        state = BossState.Chase;
                    return 0f;

                case BossState.CloseAttackWindup:
                    stateTimer -= dt;
                    if (stateTimer <= 0f)
                    {
                        state = BossState.CloseAttack;
                        stateTimer = config.CloseAttackDuration;
                        TriggerAttackVisual(stateTimer);
                    }
                    return 0f;

                case BossState.CloseAttack:
                    stateTimer -= dt;
                    if (stateTimer <= 0f)
                    {
                        state = BossState.CloseAttackRecovery;
                        stateTimer = config.CloseAttackRecoveryDuration;
                        closeAttackCooldownTimer = enraged ? config.EnragedCloseAttackCooldown : config.CloseAttackCooldown;
                    }
                    return 0f;

                case BossState.CloseAttackRecovery:
                    stateTimer -= dt;
                    if (stateTimer <= 0f)
                        state = BossState.Chase;
                    return 0f;

                default:
                    float deltaX = playerPosition.X - Position.X;
                    float distance = Math.Abs(deltaX);
                    if (closeAttackCooldownTimer <= 0f && distance <= config.CloseAttackRange)
                    {
                        StartCloseAttack(deltaX);
                        return 0f;
                    }

                    if (dashCooldownTimer <= 0f && distance >= config.DashTriggerMinDistance && distance <= config.DashTriggerMaxDistance)
                    {
                        StartDashWindup(deltaX);
                        return 0f;
                    }

                    float chaseVelocity = motor.ResolveChaseVelocity(playerPosition.X, true, enraged);
                    if (chaseVelocity > 0f)
                        facingRight = true;
                    else if (chaseVelocity < 0f)
                        facingRight = false;
                    return chaseVelocity;
            }
        }

        private void StartDashWindup(float deltaX)
        {
            state = BossState.DashWindup;
            stateTimer = config.DashWindupDuration;
            attackDirection = deltaX >= 0f ? 1 : -1;
            facingRight = attackDirection > 0;
            TriggerAttackVisual(config.DashWindupDuration);
        }

        private void StartCloseAttack(float deltaX)
        {
            state = BossState.CloseAttackWindup;
            stateTimer = config.CloseAttackWindupDuration;
            attackDirection = deltaX >= 0f ? 1 : -1;
            facingRight = attackDirection > 0;
            TriggerAttackVisual(config.CloseAttackWindupDuration);
        }

        private EnemyAnimState ResolveAnimState()
        {
            if (!IsAlive)
                return EnemyAnimState.Dead;

            if (combat.HurtTimer > 0f)
                return EnemyAnimState.Hurt;

            if (!motor.IsGrounded)
                return motor.VerticalVelocity < 0f ? EnemyAnimState.Jump : EnemyAnimState.Fall;

            if (state == BossState.DashWindup)
                return EnemyAnimState.Idle;

            if (state == BossState.Dash)
                return EnemyAnimState.Attack;

            if (state == BossState.CloseAttackWindup)
                return EnemyAnimState.Windup;

            if (state == BossState.CloseAttack)
                return EnemyAnimState.CloseAttack;

            if (Math.Abs(motor.HorizontalVelocity) > 8f)
                return EnemyAnimState.Move;

            return EnemyAnimState.Idle;
        }

        private bool ShouldJumpToIntercept(Vector2 playerPosition)
        {
            if (!IsActive || !motor.IsGrounded || jumpInterceptCooldownTimer > 0f)
                return false;

            float horizontalDistance = Math.Abs(playerPosition.X - Position.X);
            bool playerAbove = playerPosition.Y < Position.Y - config.JumpInterceptHeightThreshold;
            return playerAbove && horizontalDistance <= config.JumpInterceptRange;
        }

        private Rectangle GetActiveHitbox()
        {
            if (!IsAlive || !IsActive)
                return Rectangle.Empty;

            if (state == BossState.Dash)
                return GetDashHitbox();

            if (state == BossState.CloseAttack)
                return GetCloseAttackHitbox();

            return Rectangle.Empty;
        }

        private Rectangle GetDashHitbox()
        {
            int left = attackDirection > 0
                ? (int)Position.X + config.DashHitboxOffsetX
                : (int)Position.X - config.DashHitboxOffsetX - config.DashHitboxWidth;

            int top = (int)Position.Y - config.DashHitboxOffsetY;
            return new Rectangle(left, top, config.DashHitboxWidth, config.DashHitboxHeight);
        }

        private Rectangle GetCloseAttackHitbox()
        {
            int frameIndex = Math.Min(animator.FrameIndex, 1);
            int localLeft = frameIndex == 0 ? 17 : 16;
            int localTop = frameIndex == 0 ? 7 : 14;
            int localRightInclusive = frameIndex == 0 ? 26 : 25;
            int width = localRightInclusive - localLeft + 1;
            int height = 14;
            const int forwardOffset = 3;

            Vector2 topLeft = Position - new Vector2(16f, 32f);

            int worldLeft;
            if (facingRight)
            {
                worldLeft = (int)topLeft.X + localLeft + forwardOffset;
            }
            else
            {
                int mirroredLeft = (FrameW - 1) - localRightInclusive;
                worldLeft = (int)topLeft.X + mirroredLeft - forwardOffset;
            }

            int worldTop = (int)topLeft.Y + localTop;
            return new Rectangle(worldLeft, worldTop, width, height);
        }

        private Vector2 GetDrawPosition()
        {
            if (state != BossState.DashWindup)
                return Position;

            float phase = stateTimer * 70f;
            float offsetX = MathF.Sin(phase) * 1.5f;
            return new Vector2(Position.X + offsetX, Position.Y);
        }
    }
}
