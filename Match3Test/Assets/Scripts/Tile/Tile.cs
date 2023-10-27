using System.Collections;
using Enums;
using Match3Test.Actions;
using UnityEngine;

namespace Match3Test
{
    public class Tile : TileBase
    {
        public AllItemContainer AllItemContainer;
        private bool IsHorizontalMatch, IsVerticalMatch;
        private void OnEnable()
        {
            TileMatchAction.MatchTile += TileMatch;
            TileMatchAction.UpperTileHasDropItemControl += TileDropItemFallDown;
            TileMatchAction.DropItemDownTileCheck += DropItemDownCheck;
        }
        private void OnDisable()
        {
            TileMatchAction.MatchTile -= TileMatch;
            TileMatchAction.UpperTileHasDropItemControl -= TileDropItemFallDown;
            TileMatchAction.DropItemDownTileCheck -= DropItemDownCheck;
        }

        #region Set And Remove Drop Item 
            public void SetDropItem(DropItem dropItem)
            {
                DropItem = dropItem;
                TileEmptyOrFull = TileEmptyOrFull.Full;
            }

            public void RemoveDropItem()
            {
                DropItem = null;
                TileEmptyOrFull = TileEmptyOrFull.Empty;
            }
        #endregion

        #region Tile Explosion And Drop Item Falling
        
            private void TileMatch(Tile tile)
            {
                StartCoroutine(DestroyTile(tile));
            }

            IEnumerator DestroyTile(Tile tile)
            {
                if(tile == this && tile.TileState == TileState.None)
                {
                    TileState = TileState.Destroy;
                    
                    DropItem.DropItemExplosion();
                    RemoveDropItem();

                    yield return new WaitForSeconds(.25f);
                    
                    TileState = TileState.None;
                   // Debug.Log(" Tile Match ");
                    FindUpperNeighbourTileAndDropItemCheck();
                }
            }
            
            private void FindUpperNeighbourTileAndDropItemCheck() //find the upper tile and check
            {
                if (TileState == TileState.None) //If tile are not exploding or falling
                {
                    Tile upperTile = GetNeighbourTile(Direction.Up); //find the upper tile
                    if (upperTile != null && upperTile.TileState == TileState.None) //If tile has upper tile and upper tile state is none so send event. Check if there is an drop item on the upper tile
                    {
                        //Debug.Log("- look at up tile ");
                        TileMatchAction.UpperTileHasDropItemControl?.Invoke(upperTile);
                    }
                    else //if tile has not upper tile, this tile is the top tile
                    {
                        if (CanSpawnDrop == TileCanDropSpawn.CanDropSpawn ) 
                        {
                            if (TileState == TileState.None)
                            {
                                //Debug.Log("tis tile upper tile so can spawn");
                                SpawnDropItemAndFallDown(this); //spawn drop item
                            }
                        }
                        else if(CanSpawnDrop == TileCanDropSpawn.CantDropSpawn)
                        {
                            TileMatchAction.DropItemIsFalling?.Invoke();
                        }
                    }
                }
            }
            
            private void TileDropItemFallDown(Tile targetTile)
            {
                if(targetTile == this && TileState == TileState.None)
                {
                    if (TileEmptyOrFull == TileEmptyOrFull.Empty)
                    {
                        FindUpperNeighbourTileAndDropItemCheck();
                    }
                    else if(TileEmptyOrFull == TileEmptyOrFull.Full)
                    {
                        Tile downTile = GetNeighbourTile(Direction.Down);
                        if (downTile != null && downTile.TileEmptyOrFull == TileEmptyOrFull.Empty && downTile.TileState == TileState.None)
                        {
                            DropItem.DropItemLandsOnTheTargetTile(downTile);
                            RemoveDropItem();

                            FindUpperNeighbourTileAndDropItemCheck();
                        }
                    }
                }
            }
            
            private void DropItemDownCheck(Tile tile)
            {
                //if drop item has this tile and this tile state is none, tile find the down tile and check if there is a drop item on the down tile
                if (tile == this && tile.TileState == TileState.None)
                {
                    if (tile.DropItem != null)
                    {
                        DownTileHasDropItemCheck();
                    }
                }
            }

            public void DownTileHasDropItemCheck()
            {
                Tile downTile = GetNeighbourTile(Direction.Down);
                if (downTile == null) 
                {
                    //Debug.Log("haven't got down tile -  match control ");
                    StartCoroutine(TileMatchControl());
                    return;
                }

                if (downTile.TileState == TileState.None)
                {
                    if (downTile.TileEmptyOrFull == TileEmptyOrFull.Empty) //drop item  will drop
                    {
                        DropItem.DropItemLandsOnTheTargetTile(downTile);
                        RemoveDropItem();

                        FindUpperNeighbourTileAndDropItemCheck();
                    }
                    else if (downTile.TileEmptyOrFull == TileEmptyOrFull.Full)
                    {
                        //Debug.Log("Down Tile is full - Match control ");
                        StartCoroutine(TileMatchControl());
                    }
                }
            }
        #endregion

        #region Spawn Drop Item

            private DropItem SpawnRandomDropItem()
            {
                GameObject randomItem  = AllItemContainer.DropItemList[Random.Range(0, AllItemContainer.DropItemList.Count)].gameObject;
                DropItem dropItem = ObjectPoolManager.SpawnObject(randomItem,ObjectPoolManager.PoolType.DropItemObj).GetComponent<DropItem>();
                dropItem.transform.localScale = Vector3.one;
                dropItem.gameObject.SetActive(true);
                return dropItem;
            }
            
            private void SpawnDropItemAndFallDown(Tile spawnTile)
            {
                float itemHeight = 2.55f;
                DropItem dropItem = SpawnRandomDropItem();
                dropItem.transform.SetParent(spawnTile.transform);
                dropItem.transform.localPosition = Vector3.zero;
                dropItem.transform.localPosition += Vector3.up * itemHeight;
                dropItem.DropItemLandsOnTheTargetTile(spawnTile); 
            }
            
        #endregion

        #region Match 
                IEnumerator TileMatchControl()
                {
                    yield return new WaitForSeconds(.1f);
                    if (IsMatchTile())
                    {
                        MatchCheck();
                    }
                }
                public void MatchCheck()
                {
                    HorizontalMatchControl();
                    VerticalMatchControl();

                    if (IsVerticalMatch || IsHorizontalMatch)
                    {
                        TileMatchAction.MatchTile?.Invoke(this);
                    }
                }

                #region Vertical Match Control
                        private void VerticalMatchControl()
                        {
                             DropItemColor myColor = DropItem.DropItemColor;
                             IsVerticalMatch = false;
                            
                            if (CalculateVerticalCount())
                            {
                                IsVerticalMatch = true;
                                
                                StartCoroutine(AddMathTileList(Direction.Down,myColor));
                                StartCoroutine(AddMathTileList(Direction.Up,myColor));
                            }
                        }
                #endregion
               
                #region Horizontal Match Control
                    private  void HorizontalMatchControl()
                    {
                        DropItemColor myColor = DropItem.DropItemColor;
                        IsHorizontalMatch = false;
                        
                        if (CalculateHorizontalCount())
                        {
                            IsHorizontalMatch = true;

                            StartCoroutine(AddMathTileList(Direction.Left,myColor));
                            StartCoroutine(AddMathTileList(Direction.Right,myColor));
                        }
                    }

                #endregion

                IEnumerator AddMathTileList(Direction dir, DropItemColor myColor)
                {
                    Tile nextTile = GetNeighbourTile(dir);
                    if (nextTile != null && nextTile.TileEmptyOrFull == TileEmptyOrFull.Full)
                    {
                        while (nextTile)
                        {
                            if (nextTile.TileEmptyOrFull == TileEmptyOrFull.Empty) break;
                            if (nextTile.DropItem.DropItemColor != myColor) break;

                            TileMatchAction.MatchTile?.Invoke(nextTile);
                            yield return new WaitForSeconds(.01f);
                            nextTile = nextTile.GetNeighbourTile(dir);
                        }
                    }
                }
        #endregion

        #region Get Neighbour Tile 
            public Tile GetNeighbourTile(Direction dir)
            {
                for (int i = 0; i < TileNeighboursList.Count; i++)
                {
                    TileNeighbours tileNeighbour = TileNeighboursList[i];
                    if (tileNeighbour.NeighboursDirection == dir)
                    {
                        return tileNeighbour.NeighbourTile;
                    }
                }
                return null;
            }
        #endregion

        #region Match Count Control

            public bool IsMatchTile()
            {
                bool isMatch = false;
                if (CalculateHorizontalCount() || CalculateVerticalCount()) isMatch = true;
                return isMatch;
            }
         
           #region Horizontal Drop Item Match Controll
                public bool CalculateHorizontalCount()
                {
                    if (GetRightSameColorMatches() + GetLeftSameColorMatches() >= 2)
                    {
                        //Match
                        return true;
                    }
                    return false;
                }
                
                private int GetRightSameColorMatches()
                {
                    return DirectionNextTileSameColorMatchCount(Direction.Right);
                }

                private int GetLeftSameColorMatches()
                {
                    return DirectionNextTileSameColorMatchCount(Direction.Left);
                }       

        #endregion  
        
           #region Vertical Drop Item Match Controll
                public bool CalculateVerticalCount()
                {
                    if (GetUpSameColorMatches() + GetDownSameColorMatches() >= 2)
                    {
                        //Match
                        return true;
                    }
                    return false;
                }
                        
                private int GetUpSameColorMatches()
                {
                    return DirectionNextTileSameColorMatchCount(Direction.Up);
                }
                        
                private int GetDownSameColorMatches()
                {
                    return DirectionNextTileSameColorMatchCount(Direction.Down);
                }       

        #endregion

           #region Set Direction Next Tile Same Color Count
                private int DirectionNextTileSameColorMatchCount(Direction direction)
                {
                    int count = 0;
                    if (TileEmptyOrFull == TileEmptyOrFull.Empty) return count;

                    DropItemColor myColor = DropItem.DropItemColor;
                    Tile nextTile = GetNeighbourTile(direction);

                    if (nextTile == null || nextTile.TileEmptyOrFull == TileEmptyOrFull.Empty) return count;

                    while (nextTile)
                    {
                        if (nextTile.TileEmptyOrFull == TileEmptyOrFull.Empty) break;
                        if (nextTile.DropItem.DropItemColor != myColor) break;

                        count++;
                        nextTile = nextTile.GetNeighbourTile(direction);
                    }

                    return count;
                }
            #endregion
        #endregion
    }
}

