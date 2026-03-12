using Microsoft.Xna.Framework;
using Nyvorn.Source.World;

namespace Nyvorn.Source.Gameplay.Entities.Enemies
{
    public sealed class EnemyMotor
    {
        private readonly EnemyConfig config;
        private Vector2 position;
        private float velocityY;
        private float knockbackVelocityX;
        private float horizontalVelocity;

        public EnemyMotor(Vector2 startPosition, EnemyConfig config)
        {
            this.config = config;
            position = startPosition;
            velocityY = 0f;
            knockbackVelocityX = 0f;
            IsGrounded = false;
        }

        public Vector2 Position => position;
        public float KnockbackVelocityX => knockbackVelocityX;
        public float HorizontalVelocity => horizontalVelocity;
        public float VerticalVelocity => velocityY;
        public bool IsGrounded { get; private set; }

        private float HitLeft => position.X - 8f;
        private float HitRight => HitLeft + 16f - 1f;
        private float HitBottom => position.Y;
        private float HitTop => HitBottom - 24f + 1f;

        public Rectangle Hurtbox => new Rectangle((int)HitLeft, (int)HitTop, 16, 24);

        public void Update(float dt, WorldMap worldMap, float desiredVelocityX)
        {
            float prevHitBottom = HitBottom;
            float prevHitTop = HitTop;

            float totalVelocityX = desiredVelocityX + knockbackVelocityX;
            horizontalVelocity = totalVelocityX;
            position.X += totalVelocityX * dt;
            ResolveWorldCollisionsX(worldMap, totalVelocityX);
            knockbackVelocityX = MathHelper.Lerp(knockbackVelocityX, 0f, MathHelper.Clamp(dt * 10f, 0f, 1f));

            velocityY += 800f * dt;
            position.Y += velocityY * dt;
            ResolveWorldCollisionsY(worldMap, prevHitBottom, prevHitTop);
        }

        public void ApplyKnockback(float forceX, float forceY)
        {
            knockbackVelocityX = forceX;
            if (forceY < velocityY)
                velocityY = forceY;
        }

        public float ResolveChaseVelocity(float targetX, bool isActive, bool enraged)
        {
            if (!isActive)
                return 0f;

            float delta = targetX - position.X;
            if (System.Math.Abs(delta) <= 6f)
                return 0f;

            float moveSpeed = enraged ? config.EnragedMoveSpeed : config.BaseMoveSpeed;
            return delta > 0f ? moveSpeed : -moveSpeed;
        }

        public void TryJump()
        {
            if (!IsGrounded)
                return;

            velocityY = -config.JumpSpeed;
            IsGrounded = false;
        }

        private void ResolveWorldCollisionsX(WorldMap worldMap, float velocityX)
        {
            int ts = worldMap.TileSize;
            float top = HitTop + 1f;
            float bottom = HitBottom - 1f;
            int tileYTop = (int)(top / ts);
            int tileYBottom = (int)(bottom / ts);

            if (velocityX > 0f)
            {
                int tileX = (int)(HitRight / ts);
                if (worldMap.IsSolidAt(tileX, tileYTop) || worldMap.IsSolidAt(tileX, tileYBottom))
                {
                    float tileLeft = tileX * ts;
                    position.X = tileLeft - 8f;
                    knockbackVelocityX = 0f;
                }
            }
            else if (velocityX < 0f)
            {
                int tileX = (int)(HitLeft / ts);
                if (worldMap.IsSolidAt(tileX, tileYTop) || worldMap.IsSolidAt(tileX, tileYBottom))
                {
                    float tileRight = tileX * ts + ts;
                    position.X = tileRight + 8f;
                    knockbackVelocityX = 0f;
                }
            }
        }

        private void ResolveWorldCollisionsY(WorldMap worldMap, float prevHitBottom, float prevHitTop)
        {
            IsGrounded = false;
            int ts = worldMap.TileSize;

            float left = HitLeft + 1;
            float right = HitRight - 1;
            int tileXLeft = (int)(left / ts);
            int tileXRight = (int)(right / ts);

            if (velocityY > 0)
            {
                int fromY = (int)(prevHitBottom / ts);
                int toY = (int)(HitBottom / ts);

                for (int y = fromY; y <= toY; y++)
                {
                    if (worldMap.IsSolidAt(tileXLeft, y) || worldMap.IsSolidAt(tileXRight, y))
                    {
                        position.Y = y * ts;
                        velocityY = 0f;
                        IsGrounded = true;
                        return;
                    }
                }
            }
            else if (velocityY < 0)
            {
                int fromY = (int)(prevHitTop / ts);
                int toY = (int)(HitTop / ts);

                for (int y = fromY; y >= toY; y--)
                {
                    if (worldMap.IsSolidAt(tileXLeft, y) || worldMap.IsSolidAt(tileXRight, y))
                    {
                        float tileBottom = y * ts + ts;
                        position.Y = tileBottom + 24f - 1f;
                        velocityY = 0f;
                        return;
                    }
                }
            }
        }
    }
}
