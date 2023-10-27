using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Match3Test
{
    public class TileBase : MonoBehaviour
    {
        public int XIndex;
        public int YIndex;
        public DropItem DropItem;
        public TileCanDropSpawn CanSpawnDrop;
        public TileEmptyOrFull TileEmptyOrFull;
        public TileState TileState;
        public GameObject TileSpriteMask;
        public List<TileNeighbours> TileNeighboursList;

        public void TileSpriteMaskIsActive(bool isActive)
        {
            TileSpriteMask.SetActive(isActive);
        }

        public void AddNeighbour(Tile targetTile, Direction direction)
        {
            TileNeighbours tileNeighbours = new TileNeighbours();
            tileNeighbours.NeighboursDirection = direction;
            tileNeighbours.NeighbourTile = targetTile;
            TileNeighboursList.Add(tileNeighbours);
        }
    }

    [Serializable]
    public struct TileNeighbours
    {
        public Direction NeighboursDirection;
        public Tile NeighbourTile;
    }
}

