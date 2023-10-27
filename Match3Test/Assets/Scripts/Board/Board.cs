using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;

namespace Match3Test
{
    public class Board : MonoBehaviour
    {
        public AllItemContainer AllItemContainer;
        public GameObject BoardTileParent;

        #region  Board Size 
            public int BoardWidth;
            
            public int BoardHeight;
        #endregion

        [SerializeField] private List<Tile> AllTileList;
        public List<Column> Columns;

        private void Start()
        {
            Invoke("GameStart", .01f);
        }
        private void GameStart()
        {
            GenerateBoard();
            FindAllNeighborsForAllTiles();
            SplitTileListIntoColumns();
        }

        #region Generate Board
        
            private void GenerateBoard()
            {
                for (int i = 0; i < BoardWidth; i++)
                {
                    for (int j = 0; j < BoardHeight; j++)
                    {
                        GenerateTile(i, j);
                    }
                }
            }

            private void GenerateTile(int xIndex, int yIndex)
            {
                Tile tile = ObjectPoolManager.SpawnObject(AllItemContainer.Tile.gameObject,ObjectPoolManager.PoolType.TileObj).GetComponent<Tile>();
                tile.gameObject.transform.SetParent(BoardTileParent.transform);
                tile.name = "Tile" + xIndex + "x" + yIndex+"(Clone)";
                tile.gameObject.SetActive(true);
                tile.transform.position = new Vector3(xIndex * 2.55f, yIndex * 2.55f, 0);
                tile.XIndex = xIndex;
                tile.YIndex = yIndex;
                tile.TileSpriteMaskIsActive(false);
                
                if (yIndex == BoardHeight - 1)
                {
                    tile.CanSpawnDrop = TileCanDropSpawn.CanDropSpawn;
                    tile.TileSpriteMaskIsActive(true);
                }

                AllTileList.Add(tile);
            }

                    #region Find Neighbours
                            private void FindAllNeighborsForAllTiles()
                            {
                                foreach (Tile tile in AllTileList)
                                {
                                    FindNeighbors(tile);
                                }
                            }
                            
                            public void FindNeighbors(Tile tile)
                            {
                                int x = tile.XIndex; 
                                int y = tile.YIndex; 
                               
                                // up
                                if (y < BoardHeight - 1)
                                {
                                    Tile targetTile = AllTileList[x * BoardWidth + (y + 1)];
                                    tile.AddNeighbour(targetTile, Direction.Up);
                                }
                        
                                // right
                                if (x < BoardWidth - 1)
                                {
                                    Tile targetTile = AllTileList[(x + 1) * BoardWidth + y];
                                    tile.AddNeighbour(targetTile, Direction.Right);
                                }
                        
                                // Down
                                if (y > 0)
                                {
                                    Tile targetTile = AllTileList[x * BoardWidth + (y - 1)];
                                    tile.AddNeighbour(targetTile, Direction.Down);
                                }
                        
                                // Left
                                if (x > 0)
                                {
                                    Tile targetTile = AllTileList[(x - 1) * BoardWidth + y];
                                    tile.AddNeighbour(targetTile, Direction.Left);
                                }
                            }
                
                    #endregion

                    #region Split Tile List Into Columns
                        public void SplitTileListIntoColumns()
                        {
                            var groupedTiles = AllTileList.GroupBy(tile => tile.XIndex).ToList();
                            Columns.AddRange(groupedTiles.Select(group => new Column { xIndex = group.Key, ColumnTileList = group.ToList() }));
                        }
                    #endregion
                    
        #endregion
    }

    [Serializable]
    public class Column
    {
        public int xIndex;
        public List<Tile> ColumnTileList;
    }
}

