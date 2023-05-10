using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Link
{
    public class LinkSolver
    {
        private class FallingInfo
        {
            public int index;
            public int steps;
        }
        
        private static readonly SearchingOption[] SearchingOptions = new[] {
            new SearchingOption(Direction.Left, -1, 0), 
            new SearchingOption(Direction.Up, 0, -1),
            new SearchingOption(Direction.Right, 1, 0),
            new SearchingOption(Direction.Down, 0, 1),
        };
        
        private LinkLevelInfo levelInfo;
        private FallingInfo fallingInfo;
        private int gridWidth = 0;
        private int gridHeight = 0;

        public void Reset(LinkLevelInfo levelInfo)
        {
            this.levelInfo = levelInfo;
            gridWidth = levelInfo.width;
            gridHeight = levelInfo.height;
            fallingInfo = new FallingInfo();
        }

        public LinkSolveResult Solve(int startAddress, int targetAddress)
        {
            var result = new LinkSolveResult();
            result.success = FindPath(startAddress, targetAddress, out result.path);
            return result;
        }
        
        public LinkHintResult Hint()
        {
            var result = new LinkHintResult();
            var addresses = levelInfo.availableAddresses.ToArray();
            for (var i = 0; i < addresses.Length; i++)
            {
                for (var j = i + 1; j < addresses.Length; j++)
                {
                    result.success = FindPath(addresses[i], addresses[j], out result.path);
                    if (result.success)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        public bool TryMove()
        {
            if (null == levelInfo.fallingConfigs || 0 == levelInfo.fallingConfigs.Length)
            {
                return false;
            }

            var config = levelInfo.fallingConfigs[fallingInfo.index];
            var direction = (Direction) config.direction;
            if (config.step > 0)
            {
                fallingInfo.steps++;
                if (fallingInfo.steps >= config.step)
                {
                    fallingInfo.steps = 0;
                    fallingInfo.index++;
                    if (fallingInfo.index >= levelInfo.fallingConfigs.Length)
                    {
                        fallingInfo.index = 0;
                    }
                }
            }
            
            var shouldMove = false;
            switch (direction)
            {
                case Direction.Left:
                    for (var row = 1; row <= gridHeight; row++)
                    {
                        var distance = 0;
                        for (var col = 1; col <= gridWidth; col++)
                        {
                            var tileData = levelInfo.GetTileAt(col, row);
                            if (-1 == tileData.tag)
                            {
                                distance++;
                            }
                            else if (distance > 0)
                            {
                                shouldMove = true;
                                SwapTile(col, row, col - distance, row);
                            }
                        }
                    }
                    break;
                
                case Direction.Right:
                    for (var row = 1; row <= gridHeight; row++)
                    {
                        var distance = 0;
                        for (var col = gridWidth; col >= 1; col--)
                        {
                            var tileData = levelInfo.GetTileAt(col, row);
                            if (-1 == tileData.tag)
                            {
                                distance++;
                            }
                            else if (distance > 0)
                            {
                                shouldMove = true;
                                SwapTile(col, row, col + distance, row);
                            }
                        }
                    }
                    break;
                
                case Direction.Down:
                    for (var col = 1; col <= gridWidth; col++)
                    {
                        var distance = 0;
                        for (var row = gridHeight; row >= 1; row--)
                        {
                            var tileData = levelInfo.GetTileAt(col, row);
                            if (-1 == tileData.tag)
                            {
                                distance++;
                            }
                            else if (distance > 0)
                            {
                                shouldMove = true;
                                SwapTile(col, row, col, row + distance);
                            }
                        }
                    }
                    break;
                
                case Direction.Up:
                    for (var col = 1; col <= gridWidth; col++)
                    {
                        var distance = 0;
                        for (var row = 1; row <= gridHeight; row++)
                        {
                            var tileData = levelInfo.GetTileAt(col, row);
                            if (-1 == tileData.tag)
                            {
                                distance++;
                            }
                            else if (distance > 0)
                            {
                                shouldMove = true;
                                SwapTile(col, row, col, row - distance);
                            }
                        }
                    }
                    break;
                
                default:
                    break;
            }
            return shouldMove;
        }

        private void SwapTile(int aX, int aY, int bX, int bY)
        {
            levelInfo.Swap(aX, aY, bX, bY);
        }

        private bool FindPath(int startAddress, int targetAddress, out int[] path)
        {
            var startTile = levelInfo.GetTileByAddress(startAddress);
            var targetTile = levelInfo.GetTileByAddress(targetAddress);
            if (startTile.tag != targetTile.tag)
            {
                path = new[] {startAddress, targetAddress};
                return false;
            }
            
            var stack = new Stack<int>();
            var visited = new HashSet<int>();
            var found = FindPath(startAddress, targetAddress, startAddress, 0, Direction.None, visited, stack);
            visited.Clear();

            if (found)
            {
                path = stack.ToArray();
                Array.Reverse(path);
            }
            else
            {
                path = new[] {startAddress, targetAddress};
            }
            return found;
        }
        
        private bool FindPath(int start, int target, int current, int curves, Direction direction, HashSet<int> visited, Stack<int> path)
        {
            if (curves > 2)
            {
                return false;
            }
            if (current == target)
            {
                path.Push(target);
                return true;
            }
            if (visited.Contains(current))
            {
                return false;
            }
            if (current != start)
            {
                var currentTile = levelInfo.GetTileByAddress(current);
                if (!currentTile.CanReachable())
                {
                    return false;
                }
            }
            
            LinkLevelInfo.GetCoordinateByAddress(current, out var currX, out var currY);
            var found = false;
            visited.Add(current);
            foreach (var searchingOption in SearchingOptions)
            {
                var nextX = currX + searchingOption.x;
                var nextY = currY + searchingOption.y;
                if (nextX < 0 || nextY < 0 || (nextX > gridWidth + 1) || (nextY > gridHeight + 1))
                {
                    continue;
                }

                var neighbor = LinkLevelInfo.CalculateAddress(nextX, nextY);
                if (direction == searchingOption.direction || current == start)
                {
                    found = FindPath(start, target, neighbor, curves, searchingOption.direction, visited, path);
                    if (found)
                    {
                        if (current != target)
                        {
                            path.Push(current);
                        }
                        break;
                    }
                }
                else
                {
                    found = FindPath(start, target, neighbor, curves + 1, searchingOption.direction, visited, path);
                    if (found)
                    {
                        if (current != target)
                        {
                            path.Push(current);
                        }
                        break;
                    }
                }
            }
            visited.Remove(current);
            return found;
        }
    }
}
