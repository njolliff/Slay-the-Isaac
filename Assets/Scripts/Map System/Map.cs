using UnityEngine;

namespace MapSystem
{
    public class Map
    {
        // List of all room nodes in the map
        private RoomNode[,] _grid;
        private readonly int _columns;
        private readonly int _levels;

        // Boss room accessor
        public RoomNode BossNode { get; }

        // Initialize map data (map generation done is MapGenerator)
        public Map(RoomNode[,] grid, int columns, int levels, RoomNode bossNode)
        {
            _grid = grid;
            _columns = columns;
            _levels = levels;
            BossNode = bossNode;
        }

        // TODO: ADD PUBLIC ACCESSORS
        #region Accessors

        #endregion
    }
}