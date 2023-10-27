using System.Threading.Tasks;
using Enums;
using Match3Test.Actions;
using UnityEngine;

namespace Match3Test
{
    public class SelectedManager : MonoBehaviour
    {
         private Tile firstSelectedTile;
         private Tile targetSelectedTile;

        public MouseSwipeDetection MouseSwipeDetection;
        
        private Vector3 mouseDownPosition;
        private Camera cam;
        public LayerMask SelectableLayerMask;
        
        private void OnEnable()
        {
            InputActions.OnInputMouseDown += MouseDown;
            InputActions.OnInputMouseHold += MouseHold;
            InputActions.OnInputMouseUp += MouseUp;
        }
        private void OnDisable()
        {
            InputActions.OnInputMouseDown -= MouseDown;
            InputActions.OnInputMouseHold -= MouseHold;
            InputActions.OnInputMouseUp -= MouseUp;
        }
        private void Start()
        {
            cam = Camera.main;
        }

        #region Input Mouse Down And Mouse Up
                private void MouseDown(Vector3 mousePos)
                {
                    Tile tile = GetClickedTile(mousePos);
                    if (tile != null)
                    {
                        firstSelectedTile = tile;
                    }
                    mouseDownPosition = mousePos;
                }
                private void MouseHold(Vector3 mousePos)
                {
                    if (firstSelectedTile == null || firstSelectedTile.TileEmptyOrFull == TileEmptyOrFull.Empty)
                    {
                        GameContinue();
                        return;
                    }
                    
                    if (MouseSwipeDetection.GetDirection(mouseDownPosition, mousePos) != Direction.None)
                    {
                        InputActions.GameSetInputState?.Invoke(InputState.Match);
                        
                        Tile tile = firstSelectedTile.GetNeighbourTile(MouseSwipeDetection.MouseSwipeDirection);
                        if (tile != null)
                        {
                            targetSelectedTile = tile;

                            if (firstSelectedTile != null && firstSelectedTile == targetSelectedTile)
                            {
                                GameContinue();
                                return;
                            }

                            if (targetSelectedTile == null || targetSelectedTile.TileEmptyOrFull == TileEmptyOrFull.Empty)
                            {
                                DropItemSmallBounce(firstSelectedTile.DropItem);
                                GameContinue();
                                return;
                            }
                            
                            SwitchDropItems();
                        }
                        else
                        {
                            DropItemSmallBounce(firstSelectedTile.DropItem);
                            GameContinue();
                        }
                    }
                }
                private void MouseUp(Vector3 mousePos)
                {
                    GameContinue();
                }
        #endregion
        
        private void ResetSelectedTiles()
        {
            firstSelectedTile = null;
            targetSelectedTile = null;
        }
        
        private void GameContinue()
        {
            ResetSelectedTiles();
            InputActions.GameSetInputState?.Invoke(InputState.None);
        }
        
        private void DropItemSmallBounce(DropItem dropItem)
        {
            dropItem.DropItemMakesASmallMovementInTheDirection(MouseSwipeDetection.MouseSwipeDirection);
        }

        #region Swipe And Switch Tile
             private async  void SwitchDropItems()
             {
                DropItem firstDropItem = firstSelectedTile.DropItem;
                DropItem targetDropItem = targetSelectedTile.DropItem;
                        
                firstDropItem.SetTargetTile(targetSelectedTile, 2);
                targetDropItem.SetTargetTile(firstSelectedTile , 2);

                await Task.Delay(200);
                        
                //target tile and first tile neighbours drop item control
                if (targetSelectedTile.IsMatchTile() || firstSelectedTile.IsMatchTile())
                {
                    targetSelectedTile.MatchCheck();
                    firstSelectedTile.MatchCheck();
                    ResetSelectedTiles();
                }
                else
                {
                    await Task.Delay(100);
                    //Debug.Log("- NON MATCH RETURN BACK -");
                    firstDropItem.SetTargetTile(firstSelectedTile, 1);
                    targetDropItem.SetTargetTile(targetSelectedTile, 1);
                        
                    await Task.Delay(100);
                    GameContinue();
                }
             }

        #endregion

        #region Send Raycast and Selected Tile
                private Tile GetClickedTile(Vector3 mousePos)
                {
                    RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPos(mousePos), Vector2.zero, SelectableLayerMask);
                    
                    if (hit.collider != null)
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        if (hitObject.tag == "Tile")
                        {
                            return hitObject.gameObject.GetComponent<Tile>();
                        }
                    }
                    return null;
                }
                
                private Vector3 GetMouseWorldPos(Vector3 mousePos)
                {
                    Vector2 mousePoint = mousePos;
                    if(cam == null)
                        cam = Camera.main;

                    if (cam == null)
                    {
                        return Vector3.zero;
                    }
                    return cam.ScreenToWorldPoint(mousePoint);
                }
        #endregion
        
    }
}
