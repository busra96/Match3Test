using DG.Tweening;
using Enums;
using Match3Test.Actions;
using UnityEngine;

namespace  Match3Test
{
    public class DropItem : MonoBehaviour
    {
        public DropItemColor DropItemColor;
        public Tile Tile;

        public void GameStartSetTile(Tile targetTile)
        {
            SetData(targetTile);
            transform.localPosition = Vector3.zero;
        }

        public void SetTargetTile(Tile targetTile, int totalMoveTileCount)
        {
            SetData(targetTile);
            transform.DOLocalMove(Vector3.zero, totalMoveTileCount * .05f).SetEase(Ease.Linear);
        }

        public void DropItemLandsOnTheTargetTile(Tile targetTile)
        {
            TileMatchAction.DropItemIsFalling?.Invoke();
            
            targetTile.TileState = TileState.Falling;
            SetData(targetTile);
            transform.DOLocalMove(Vector3.zero,  .075f).SetEase(Ease.Linear).OnComplete(() =>
            {
                targetTile.TileState = TileState.None;
                TileMatchAction.DropItemDownTileCheck?.Invoke(targetTile);
            });
        }
        
        private void SetData(Tile targetTile)
        {
            Tile = targetTile;
            Tile.SetDropItem(this);
            transform.SetParent(Tile.transform);
        }

        public void DropItemMakesASmallMovementInTheDirection(Direction dir)
        {
            Vector3 localPos = transform.localPosition;
            float amount = .25f;
            switch (dir)
            {
                case Direction.Down:
                    localPos += Vector3.down * amount;
                    break;
                case Direction.Up:
                    localPos += Vector3.up * amount;
                    break;
                case Direction.Right:
                    localPos += Vector3.right * amount;
                    break;
                case Direction.Left:
                    localPos += Vector3.left * amount;
                    break;
            }
            
            transform.DOLocalMove(localPos, .1f).SetEase(Ease.Linear)
                .OnComplete(() => transform.DOLocalMove(Vector3.zero, .1f).SetEase(Ease.Linear));
        }
        
        public void DropItemExplosion()
        {
            RemoveTile();
            transform.DOScale(Vector3.zero, .25f).SetEase(Ease.Linear).OnComplete(() =>
            {
                ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.DropItemObj);
            });
        }
     
        public void RemoveTile()
        {
            Tile = null;
        }
    }

}
