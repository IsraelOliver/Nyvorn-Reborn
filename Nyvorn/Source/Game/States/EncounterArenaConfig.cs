using Microsoft.Xna.Framework;
using System;

namespace Nyvorn.Source.Game.States
{
    public sealed class EncounterArenaConfig
    {
        public Point WorldSizeTiles { get; init; } = new Point(72, 34);
        public int TileSize { get; init; } = 8;
        public int GroundOffsetFromBottom { get; init; } = 4;
        public int SafeZoneStartTileX { get; init; } = 2;
        public int SafeZoneEndTileX { get; init; } = 13;
        public int EntranceGateTileX { get; init; } = 15;
        public int EntranceTriggerTileX { get; init; } = 18;
        public int ArenaStartTileX { get; init; } = 16;
        public int ExitGateTileX { get; init; } = 67;
        public int BarrierHeightTiles { get; init; } = 8;
        public int EntranceFenceStartTileX { get; init; } = 11;
        public int EntranceFenceEndTileX { get; init; } = 14;
        public int ExitFenceStartTileX { get; init; } = 65;
        public int ExitFenceEndTileX { get; init; } = 67;
        public Vector2 PlayerSpawn { get; init; } = new Vector2(52, 240);
        public Vector2 BossSpawn { get; init; } = new Vector2(472, 240);
        public Vector2 InitialWeaponSpawn { get; init; } = new Vector2(92, 220);
        public string BossName { get; init; } = "Guardiao do Campo";

        public int GroundTileY => WorldSizeTiles.Y - GroundOffsetFromBottom;

        public EncounterArenaConfig Normalize()
        {
            Point worldSize = new Point(
                WorldSizeTiles.X > 0 ? WorldSizeTiles.X : 72,
                WorldSizeTiles.Y > 0 ? WorldSizeTiles.Y : 34);

            int tileSize = TileSize > 0 ? TileSize : 8;
            int groundOffset = Math.Clamp(GroundOffsetFromBottom, 1, Math.Max(1, worldSize.Y - 1));
            int maxTileX = Math.Max(1, worldSize.X - 2);

            int safeStart = Math.Clamp(SafeZoneStartTileX, 1, maxTileX);
            int safeEnd = Math.Clamp(SafeZoneEndTileX, safeStart, maxTileX);
            int entranceGate = Math.Clamp(EntranceGateTileX, 1, maxTileX);
            int entranceTrigger = Math.Clamp(EntranceTriggerTileX, entranceGate, maxTileX);
            int arenaStart = Math.Clamp(ArenaStartTileX, entranceGate, maxTileX);
            int exitGate = Math.Clamp(ExitGateTileX, arenaStart, maxTileX);
            int barrierHeight = Math.Clamp(BarrierHeightTiles, 1, worldSize.Y);

            int entranceFenceStart = Math.Clamp(EntranceFenceStartTileX, 1, maxTileX);
            int entranceFenceEnd = Math.Clamp(EntranceFenceEndTileX, entranceFenceStart, maxTileX);
            int exitFenceStart = Math.Clamp(ExitFenceStartTileX, 1, maxTileX);
            int exitFenceEnd = Math.Clamp(ExitFenceEndTileX, exitFenceStart, maxTileX);

            return new EncounterArenaConfig
            {
                WorldSizeTiles = worldSize,
                TileSize = tileSize,
                GroundOffsetFromBottom = groundOffset,
                SafeZoneStartTileX = safeStart,
                SafeZoneEndTileX = safeEnd,
                EntranceGateTileX = entranceGate,
                EntranceTriggerTileX = entranceTrigger,
                ArenaStartTileX = arenaStart,
                ExitGateTileX = exitGate,
                BarrierHeightTiles = barrierHeight,
                EntranceFenceStartTileX = entranceFenceStart,
                EntranceFenceEndTileX = entranceFenceEnd,
                ExitFenceStartTileX = exitFenceStart,
                ExitFenceEndTileX = exitFenceEnd,
                PlayerSpawn = PlayerSpawn,
                BossSpawn = BossSpawn,
                InitialWeaponSpawn = InitialWeaponSpawn,
                BossName = string.IsNullOrWhiteSpace(BossName) ? "Guardiao do Campo" : BossName
            };
        }
    }
}
