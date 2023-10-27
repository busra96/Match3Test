using UnityEngine.Events;

namespace Match3Test.Actions
{
    public class TileMatchAction
    {
        public static UnityAction<Tile> UpperTileHasDropItemControl;
        public static UnityAction<Tile> MatchTile;
        public static UnityAction<Tile> DropItemDownTileCheck;

        public static UnityAction DropItemIsFalling;
    }
}

