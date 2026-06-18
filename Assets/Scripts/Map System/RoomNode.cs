using UnityEngine;
using System.Collections.Generic;

namespace MapSystem
{
    public class RoomNode
    {
        // Position & Type
        public int column;
        public int level;
        public RoomType roomType = RoomType.None;

        // Parents & Children
        public List<RoomNode> parents = new();
        public List<RoomNode> children = new();

        // If the room is reachable on the map
        public bool IsReachable => level == 0 || parents.Count > 0;

        // Initialize with column and level, type is set later
        public RoomNode(int column, int level)
        {
            this.column = column;
            this.level = level;
        }
    }
}