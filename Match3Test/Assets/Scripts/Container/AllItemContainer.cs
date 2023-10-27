using System.Collections.Generic;
using UnityEngine;

namespace Match3Test
{
    [CreateAssetMenu]
    public class AllItemContainer : ScriptableObject
    {
        public Tile Tile;

        [Space] [Header("Drop Items")]
        public List<DropItem> DropItemList;
        
    }
}
