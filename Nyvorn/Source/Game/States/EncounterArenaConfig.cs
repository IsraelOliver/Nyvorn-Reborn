using Microsoft.Xna.Framework;

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
    }
}
