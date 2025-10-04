using UnityEngine;

namespace App
{
    public enum TileType
    {
        Floor = 0,
        Collectible = 1,
    }
    
    public class TileMatcher: MonoBehaviour
    {
        public TileType tileType;
    }
}