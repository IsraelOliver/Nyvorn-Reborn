using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nyvorn.Source.World
{
    public class WorldMap
    {
        public int Width { get; }
        public int Height { get; }
        public int TileSize { get; }

        private Texture2D _dirt;
        private Texture2D _sand;
        private Texture2D _stone;

        private readonly TileType[,] _tiles;

        public WorldMap(int width, int height, int tileSize)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;

            _tiles = new TileType[Width, Height];
        }

        public TileType GetTile(int x, int y)
        {
            if (!InBounds(x, y))
                return TileType.Empty;

            return _tiles[x, y];
        }

        public void SetTile(int x, int y, TileType type)
        {
            if (!InBounds(x, y))
                return;

            _tiles[x, y] = type;
        }

        public bool InBounds(int x, int y)
            => x >= 0 && y >= 0 && x < Width && y < Height;

        public bool IsSolid(TileType t)
        {
            return t == TileType.Dirt
                || t == TileType.Stone
                || t == TileType.Sand;
        }

        public bool IsSolidAt(int x, int y) => IsSolid(GetTile(x, y));

        public Rectangle GetTileBounds(int x, int y)
            => new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);

        public Point WorldToTile(Vector2 worldPos)
            => new Point((int)(worldPos.X / TileSize), (int)(worldPos.Y / TileSize));

        public void GenerateFieldArena(int groundY, int safeZoneStart, int safeZoneEnd, int entranceGateTileX, int arenaStartTileX)
        {
            if (Width <= 0 || Height <= 0)
                return;

            groundY = Math.Clamp(groundY, 0, Height - 1);
            safeZoneStart = Math.Clamp(safeZoneStart, 0, Width - 1);
            safeZoneEnd = Math.Clamp(safeZoneEnd, safeZoneStart, Width - 1);
            entranceGateTileX = Math.Clamp(entranceGateTileX, 0, Width - 1);
            arenaStartTileX = Math.Clamp(arenaStartTileX, entranceGateTileX, Width - 1);

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    _tiles[x, y] = TileType.Empty;

            for (int x = 0; x < Width; x++)
                _tiles[x, groundY] = TileType.Dirt;

            for (int y = 0; y < Height; y++)
            {
                _tiles[0, y] = TileType.Stone;
                _tiles[Width - 1, y] = TileType.Stone;
            }

            for (int y = groundY + 1; y < Height; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                    _tiles[x, y] = TileType.Stone;
            }

            for (int x = safeZoneStart; x <= safeZoneEnd; x++)
                _tiles[x, groundY] = TileType.Sand;

            int gateFillStart = Math.Clamp(entranceGateTileX - 1, 0, Width - 1);
            for (int x = gateFillStart; x < arenaStartTileX; x++)
                _tiles[x, groundY] = TileType.Stone;

            for (int x = arenaStartTileX; x < Width - 1; x++)
                _tiles[x, groundY] = TileType.Dirt;

        }

        public void SetTextures(Texture2D dirt, Texture2D sand, Texture2D stone)
        {
            _dirt = dirt;
            _sand = sand;
            _stone = stone;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    TileType t = GetTile(x, y);
                    if (t == TileType.Empty)
                        continue;

                    Texture2D tex = t switch
                    {
                        TileType.Dirt => _dirt,
                        TileType.Sand => _sand,
                        TileType.Stone => _stone,
                        _ => null
                    };

                    if (tex == null)
                        continue;

                    spriteBatch.Draw(tex, GetTileBounds(x, y), Color.White);
                }
            }
        }
    }
}
