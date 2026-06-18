using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MapSystem
{
    public static class RoomTypeAssigner
    {
        // Levels that are locked to a specific room type
        private static readonly Dictionary<int, RoomType> LockedFloors = new()
        {
            [0] = RoomType.Monster,
            [8] = RoomType.Treasure,
            [14] = RoomType.RestSite,
        };

        // Weighted pool for randomly assigned levels
        private static readonly (RoomType Type, int Weight)[] WeightedPool =
        {
            (RoomType.Monster, 45),
            (RoomType.Mystery, 22),
            (RoomType.Elite, 12),
            (RoomType.RestSite, 12),
            (RoomType.Shop, 5),
            (RoomType.Treasure, 4)
        };

        private const int EliteMinLevel = 5; // Elites start on level 6 (0 based)
        private const int NoRestSiteFloor = 13; // No Rest sites below the guaranteed rest site floor

        public static void AssignRoomTypes(RoomNode[,] grid, int columns, int levels)
        {
            // Assign locked levels
            for (int level = 0; level < levels; level++)
            {
                if (!LockedFloors.TryGetValue(level, out RoomType lockedType))
                    continue;

                for (int col = 0; col < columns; col++)
                {
                    var node = grid[col, level];
                    if (node.IsReachable)
                        node.roomType = lockedType;
                }
            }

            // Assign random room types to all remaining reachable nodes
            for (int level = 0; level < levels; level++)
            {
                if (LockedFloors.ContainsKey(level))
                    continue;

                for (int col = 0; col < columns; col++)
                {
                    var node = grid[col, level];
                    if (node.IsReachable)
                        AssignWithRetry(node, level);
                }
            }
        }

        #region Helper Methods
        private static void AssignWithRetry(RoomNode node, int level)
        {
            const int maxAttempts = 10;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                RoomType candidate = DrawWeighted(level);

                if (IsValidPlacement(node, candidate, level))
                {
                    node.roomType = candidate;
                    return;
                }
            }

            // Fallback - monster is always safe
            node.roomType = RoomType.Monster;
        }

        private static bool IsValidPlacement(RoomNode node, RoomType candidate, int level)
        {
            // Elites can't appear before EliteMinLevel
            if (candidate == RoomType.Elite && level < EliteMinLevel)
                return false;

            // Rest sites can't appear before the rest site floor
            if (candidate == RoomType.RestSite && level == NoRestSiteFloor)
                return false;

            // A parent node with multiple children must have children with unique room types
            if (node.parents.Count > 0)
            {
                // Check each parent if node has >1
                foreach (var parent in node.parents)
                {
                    // Continue if parent has no other children
                    if (parent.children.Count < 2)
                        continue;

                    // Find any other children of the parent and check if they are the candidate type
                    bool siblingHasSameType = parent.children
                        .Where(sibling => sibling != node && sibling.roomType != RoomType.None)
                        .Any(sibling => sibling.roomType == candidate);

                    // If they are, type is not valid
                    if (siblingHasSameType)
                        return false;
                }
            }

            // Otherwise, type is valid
            return true;
        }
        private static RoomType DrawWeighted(int level)
        {
            // Get all valid type entries from the weighted pool 
            // (not elite below elite min or rest site on no-rest)
            var pool = WeightedPool
                .Where(entry => !(entry.Type == RoomType.Elite && level < EliteMinLevel))
                .Where(entry => !(entry.Type == RoomType.RestSite && level == NoRestSiteFloor))
                .ToList();

            // Get total weight of pool and pick a starting roll
            int totalWeight = pool.Sum(e => e.Weight);
            int roll = UnityEngine.Random.Range(0, totalWeight);

            // Substract the weight of each type until roll becomes negative,
            // then return that type when it does
            foreach (var (type, weight) in pool)
            {
                roll -= weight;
                if (roll < 0)
                    return type;
            }

            // Default case to satisfy compiler
            return RoomType.Monster;
        }
        #endregion
    }
}