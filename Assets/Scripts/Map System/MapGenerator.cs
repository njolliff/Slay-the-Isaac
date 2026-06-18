using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MapSystem
{
    public class MapGenerator
    {
        #region Fields
        // Constraints
        public const int Columns = 7;
        public const int Levels = 15; // 0-14
        public const int PathPasses = 6;

        private const int RestSiteLevel = 14; // Level 15

        // Grid and seed
        private RoomNode[,] _grid;
        private readonly int? _seed;
        public int Seed { get; private set; }
        #endregion

        #region Constructor
        // Can optionally be initialized with a set seed
        public MapGenerator(int? seed = null)
        {
            _seed = seed;
        }
        #endregion

        #region Map Generation
        /// <summary>
        /// Generates a new map based on the configured parameters and seed.
        /// </summary>
        /// <returns></returns>
        public Map GenerateMap()
        {
            // Set seed
            Seed = _seed ?? System.Environment.TickCount;
            UnityEngine.Random.InitState(Seed);

            // Generate map
            BuildGrid();
            WeavePaths();
            PruneUnreachableNodes();
            RoomTypeAssigner.AssignRoomTypes(_grid, Columns, Levels);
            var bossNode = AttachBoss();

            // Return map
            return new Map(_grid, Columns, Levels, bossNode);
        }

        /// <summary>
        /// Builds a fresh grid of unconnected room nodes.
        /// </summary>
        private void BuildGrid()
        {
            _grid = new RoomNode[Columns, Levels];
            for (int column = 0; column < Columns; column++)
            {
                for (int level = 0; level < Levels; level++)
                {
                    _grid[column, level] = new RoomNode(column, level);
                }
            }
        }
        /// <summary>
        /// Weaves paths through the the grid 'PathPasses' times
        /// </summary>
        private void WeavePaths()
        {
            int firstStartCol = -1;

            // Walk a path PathPasses times
            for (int pass = 0; pass < PathPasses; pass++)
            {
                int startCol;

                // Get and record the starting column for the 1st pass
                if (pass == 0)
                {
                    startCol = UnityEngine.Random.Range(0, Columns);
                    firstStartCol = startCol;
                }

                // Get a different starting column for the 2nd pass
                else if (pass == 1)
                {
                    do { startCol = UnityEngine.Random.Range(0, Columns); }
                    while (startCol == firstStartCol);
                }

                // For all other passes, get any random starting column
                else
                {
                    startCol = UnityEngine.Random.Range(0, Columns);
                }

                // Walk path from picked starting column
                WalkPath(startCol);
            }
        }
        /// <summary>
        /// Safeguard that I believe does nothing. Checks for any unreachable nodes
        /// (nodes with no parents) that have children, and removes the connections with
        /// those children if found.
        /// </summary>
        private void PruneUnreachableNodes()
        {
            for (int level = 1; level < Levels; level++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    RoomNode node = _grid[col, level];
                    if (!node.IsReachable && node.children.Count > 0)
                    {
                        foreach (var child in node.children)
                            child.parents.Remove(node);
                        node.children.Clear();
                    }
                }
            }
        }
        /// <summary>
        /// Attaches a boss node above the rest site level.
        /// </summary>
        /// <returns></returns>
        private RoomNode AttachBoss()
        {
            var bossNode = new RoomNode(-1, Levels) {roomType = RoomType.Boss};

            for (int col = 0; col < Columns; col++)
            {
                RoomNode restSite = _grid[col, RestSiteLevel];
                if (restSite.IsReachable)
                {
                    restSite.children.Add(bossNode);
                    bossNode.parents.Add(restSite);
                }
            }

            return bossNode;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Walks a path from level 0 to level 14 from 'startCol''.
        /// </summary>
        private void WalkPath(int startCol)
        {
            int currentCol = startCol;

            for (int level = 0; level < Levels - 1; level++)
            {
                // Get current room node
                RoomNode current = _grid[currentCol, level];

                // Find candidates (left, middle, and right nodes on next level)
                // that do not cross an existing path
                var candidates = GetAdjacentColumns(currentCol)
                    .Where(nextCol => !WouldCross(currentCol, level, nextCol)).ToList();
                
                // If something went wrong and there are no candidates, straight up is always safe
                if (candidates.Count == 0)
                    candidates.Add(currentCol);

                // Pick a column from candidates and get the room node in that column on the next floor
                int nextCol = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                RoomNode next = _grid[nextCol, level + 1];

                // Link the two room nodes if not already linked
                if (!current.children.Contains(next))
                {
                    current.children.Add(next);
                    next.parents.Add(current);
                }

                // Set current column for next iteration
                currentCol = nextCol;
            }
        }
        /// <summary>
        /// Returns the columns to the left, straight, and right 
        /// of 'col' if they exist within the grid width.
        /// </summary>
        private IEnumerable<int> GetAdjacentColumns(int col)
        {
            // Return left column if not on left edge
            if (col > 0) yield return col - 1;

            // Return same column (straight up is always safe)
            yield return col;

            // Return right column if not on right edge
            if (col < Columns - 1) yield return col + 1;
        }
        /// <summary>
        /// Checks if an existing edge would cross the intended edge 'fromCol'->'toCol' on 'level'.
        /// </summary>
        private bool WouldCross(int fromCol, int toCol, int level)
        {
            // Straight up is always safe
            if (toCol == fromCol)
                return false;

            // The only edge that crosses fromCol -> toCol is toCol -> fromCol,
            // so get the room nodes for toCol -> fromCol and check for connection
            RoomNode mirror = _grid[toCol, level];
            RoomNode mirrorDest = _grid[fromCol, level + 1];

            // True if an edge already exists
            return mirror.children.Contains(mirrorDest);
        }
        #endregion
    }
}