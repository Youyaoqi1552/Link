using System.Collections.Generic;
using System.Linq;
using Game.Config;
using UnityEngine;

namespace Game.Link
{
    public class LinkLevelInfo
    {
        public int level;
        public int width;
        public int height;
        public int remainingPairs;
        public FallingConfig[] fallingConfigs;
        public HashSet<int> availableAddresses;
        public RobotData robotData;
        
        public PatternDifficultyConfig[] patternDifficultyConfigs;
        
        private Dictionary<int, int> addressToKeyMapper;
        private Dictionary<int, TileData> tiles;

        public void Pair(int addressA, int addressB)
        {
            var tileA = GetTileByAddress(addressA);
            var tileB = GetTileByAddress(addressB);
            tileA.tag = tileB.tag = -1;
            remainingPairs--;
            
            availableAddresses.Remove(addressA);
            availableAddresses.Remove(addressB);
        }
        
        public void Shuffle()
        {
            var addresses = availableAddresses.ToArray();
            var random = new System.Random();
            for (var i = addresses.Length - 1; i > 0; i--)
            {
                var idx = random.Next(i - 1);
                Swap(addresses[i], addresses[idx], true);
                (addresses[i], addresses[idx]) = (addresses[idx], addresses[i]);
            }
        }

        public void Swap(int addressA, int addressB, bool ignoreCheckAvailable = false)
        {
            if (addressA == addressB)
            {
                return;
            }
            
            (addressToKeyMapper[addressA], addressToKeyMapper[addressB]) =
                (addressToKeyMapper[addressB], addressToKeyMapper[addressA]);
            
            if (ignoreCheckAvailable)
            {
                return;
            }
            
            if (availableAddresses.Contains(addressA))
            {
                if (!availableAddresses.Contains(addressB))
                {
                    availableAddresses.Remove(addressA);
                    availableAddresses.Add(addressB);
                }
            }
            else if (availableAddresses.Contains(addressB))
            {
                availableAddresses.Remove(addressB);
                availableAddresses.Add(addressA);
            }
        }

        public void Swap(int aX, int aY, int bX, int bY)
        {
            var addressA = CalculateAddress(aX, aY);
            var addressB = CalculateAddress(bX, bY);
            Swap(addressA, addressB);
        }

        public TileData GetTileAt(int x, int y)
        {
            var address = CalculateAddress(x, y);
            tiles.TryGetValue(addressToKeyMapper[address], out var tileData);
            return tileData;
        }

        public TileData GetTileByAddress(int address)
        {
            tiles.TryGetValue(addressToKeyMapper[address], out var tileData);
            return tileData;
        }
        
        public int GetTileAddressFromKey(int key)
        {
            foreach (var pair in addressToKeyMapper)
            {
                if (pair.Value == key)
                {
                    return pair.Key;
                }
            }
            return -1;
        }
        
        public LinkLevelInfo Clone()
        {
            return this;
        }

        public static LinkLevelInfo Create(int level, BoardConfig boardConfig, RobotDifficultyConfig robotDifficultyConfig, List<PatternDifficultyConfig> patternDifficultyConfigs)
        {
            var levelInfo = new LinkLevelInfo
            {
                level = level,
                width = boardConfig.width,
                height = boardConfig.height,
                remainingPairs = boardConfig.maxPairs,
                fallingConfigs = boardConfig.fallingConfigs,
                tiles = new Dictionary<int, TileData>(),
                addressToKeyMapper = new Dictionary<int, int>(),
                availableAddresses = new HashSet<int>(),
                robotData = new RobotData {maxDuration = robotDifficultyConfig.maxReactTime, minDuration = robotDifficultyConfig.minReactTime},
                patternDifficultyConfigs = patternDifficultyConfigs.ToArray(),
            };

            var tileConfigs = boardConfig.tileConfigs;
            for (var i = 0; i < tileConfigs.Length; i++)
            {
                var col = i % boardConfig.width + 1;
                var row = i / boardConfig.width + 1;
                var tileConfig = tileConfigs[i];
                var address = CalculateAddress(col, row);
                var tileData = new TileData(address)
                {
                    tag = tileConfig.tag,
                };
                levelInfo.tiles[tileData.key] = tileData;
                levelInfo.addressToKeyMapper[address] = tileData.key;

                if (-1 != tileData.tag)
                {
                    levelInfo.availableAddresses.Add(address);
                }
            }

            var maxCol = levelInfo.width + 1;
            var maxRow = levelInfo.height + 1;
            for (var i = 0; i <= maxCol; i++)
            {
                var address = CalculateAddress(i, 0);
                var tileData = new TileData(address) {tag = -1};
                levelInfo.tiles[tileData.key] = tileData;
                levelInfo.addressToKeyMapper[address] = tileData.key;

                address = CalculateAddress(i, maxRow);
                tileData = new TileData(address) {tag = -1};
                levelInfo.tiles[tileData.key] = tileData;
                levelInfo.addressToKeyMapper[address] = tileData.key;
            }

            for (var i = 1; i < maxRow; i++)
            {
                var address = CalculateAddress(0, i);
                var tileData = new TileData(address) {tag = -1};
                levelInfo.tiles[tileData.key] = tileData;
                levelInfo.addressToKeyMapper[address] = tileData.key;

                address = CalculateAddress(maxCol, i);
                tileData = new TileData(address) {tag = -1};
                levelInfo.tiles[tileData.key] = tileData;
                levelInfo.addressToKeyMapper[address] = tileData.key;
            }
            return levelInfo;
        }

        public static int CalculateAddress(int x, int y)
        {
            return x * 100000 + y;
        }
        
        public static void GetCoordinateByAddress(int address, out int x, out int y)
        {
            x = address / 100000;
            y = address % 100000;
        }
    }
}