using UnityEngine;

namespace Main.AI.Pathfindidng
{
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}