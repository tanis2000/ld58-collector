using UnityEngine;

namespace App
{
    public enum TileType
    {
        Floor = 0,
    }
    
    public class TileMatcher: MonoBehaviour
    {
        public TileType tileType;
    }
}