using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3Test
{
    public class BoardDropManager : MonoBehaviour
    {
        public AllItemContainer AllItemContainer;
        public Board Board;

        private void Start()
        {
            Invoke("FilledDropItemAllBoard", .02f);
        }

        #region Game Start Filled Board
            public void FilledDropItemAllBoard()
            {
                for (int i = 0; i < Board.Columns.Count; i++)
                {
                    Column verticalColumn = Board.Columns[i];
                    if(verticalColumn.ColumnTileList[Board.Columns.Count - 1].CanSpawnDrop == TileCanDropSpawn.CantDropSpawn) continue;
                    
                    for (int j = 0; j < verticalColumn.ColumnTileList.Count; j++)
                    {
                        Tile tile = verticalColumn.ColumnTileList[j];
                        if (tile.TileEmptyOrFull != TileEmptyOrFull.Empty) continue;

                        SpawnDropItemAndColorControl(tile);
                    }
                }
            }

            private void SpawnDropItemAndColorControl(Tile tile)
            {
                while (tile)
                {
                    DropItem dropItem = SpawnRandomDropItem();
                   
                    dropItem.GameStartSetTile(tile);
                         
                    if (tile.CalculateHorizontalCount() || tile.CalculateVerticalCount())
                    {
                        dropItem.RemoveTile();
                        tile.RemoveDropItem();
                        
                        ObjectPoolManager.ReturnObjectToPool(dropItem.gameObject,ObjectPoolManager.PoolType.DropItemObj);
                    }
                    else 
                    {
                        break;
                    }
                }
            }
        
        #endregion
        private DropItem SpawnRandomDropItem()
        {
            GameObject randomItem  = AllItemContainer.DropItemList[Random.Range(0, AllItemContainer.DropItemList.Count)].gameObject;
            DropItem dropItem = ObjectPoolManager.SpawnObject(randomItem,ObjectPoolManager.PoolType.DropItemObj).GetComponent<DropItem>();
            dropItem.transform.localScale = Vector3.one;
            dropItem.gameObject.SetActive(true);
            return dropItem;
        }
    }
}

