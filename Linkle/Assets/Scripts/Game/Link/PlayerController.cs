using System;
using System.Collections.Generic;
using Common.Extensions;
using Common.Timer;
using DG.Tweening;
using Game.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Link
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LinkPlayerIndicator playIndicator;
        [SerializeField] private Vector2 preferredTileSize;
        [SerializeField] private Vector2 preferredTileSpacing;
        [SerializeField] private LinkTile tilePrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private LinkPathDrawer pathDrawer;
        [SerializeField] private LinkComboAnimPlayer comboAnimPlayer;
        public RectTransform selectionLayer;
        
        public int totalRewardedMedals;
        public int maxComboCount;
        
        public event Action OnGameFinished;
        
        private Vector2 tileSize;
        private Vector2 tileSpacing;
        private int gridWidth;
        private int gridHeight;
        private Vector2 startPosition;
        private LinkSolver solver = new LinkSolver();
        private Dictionary<int, LinkTile> tiles = new Dictionary<int, LinkTile>();
        private LinkTile selectedTile;
        private LinkLevelInfo levelInfo;
        private HashSet<int> hintTileAddresses = new HashSet<int>();
        
        private int currentComboCount;
        private TimeRecorder comboTimeRecorder = new TimeRecorder();

        private bool isShuffling;
        private bool isPairing;

        public void Init(LinkLevelInfo levelInfo, AgentData agentData)
        {
            this.levelInfo = levelInfo;
            
            solver.Reset(levelInfo);
            playIndicator.Init(agentData.Avatar, levelInfo.remainingPairs);

            pathDrawer.SetGrid(this);
            
            gridWidth = levelInfo.width;
            gridHeight = levelInfo.height;

            var scale = 1f;
            var contentSize = content.rect.size;
            var preferredWidth = (preferredTileSize.x + preferredTileSpacing.x) * gridWidth - preferredTileSpacing.x;
            var preferredHeight = (preferredTileSize.y + preferredTileSpacing.y) * gridHeight - preferredTileSpacing.y;
            if (contentSize.x < preferredWidth || contentSize.y < preferredHeight)
            {
                scale = Mathf.Min(contentSize.x / preferredWidth, contentSize.y / preferredHeight);
                tileSize.Set(preferredTileSize.x * scale, preferredTileSize.y * scale);
                tileSpacing.Set(preferredTileSpacing.x * scale, preferredTileSpacing.y * scale);
            }
            else
            {
                tileSize.Set(preferredTileSize.x, preferredTileSize.y);
                tileSpacing.Set(preferredTileSpacing.x, preferredTileSpacing.y);
            }

            var actualWidth = (tileSize.x + tileSpacing.x) * gridWidth - tileSpacing.x;
            var actualHeight = (tileSize.y + tileSpacing.y) * gridHeight - tileSpacing.y;
            startPosition.Set((tileSize.x - actualWidth) * 0.5f, (actualHeight - tileSize.y)* 0.5f);
            var posY = startPosition.y;
            for (var row = 1; row <= gridHeight; row++)
            {
                var posX = startPosition.x;
                for (var col = 1; col <= gridWidth; col++)
                {
                    var address = LinkLevelInfo.CalculateAddress(col, row);
                    var tileData = levelInfo.GetTileByAddress(address);
                    if (-1 != tileData.tag)
                    {
                        var tile = Instantiate(tilePrefab, content);
                        var tileRect = tile.GetComponent<RectTransform>();
                        tileRect.sizeDelta = tileSize;
                        tileRect.SetAnchoredPosition(posX, posY);
                        
                        tile.SetContext(this, tileSize, scale);
                        tile.SetAddress(address);
                        tile.SetData(tileData);
                        tiles[tileData.key] = tile;
                    }
                    posX += tileSize.x + tileSpacing.x;
                }
                posY -= tileSize.y + tileSpacing.y;
            }
        }

        public void Dispose()
        {
            comboTimeRecorder.Dispose();
            comboAnimPlayer.StopAnim();
            
            pathDrawer.Dispose();
        }

        public void BeginPlay()
        {
            
        }
        
        public void EndPlay()
        {
            comboTimeRecorder.Stop();
            comboAnimPlayer.StopAnim();
        }
        
        public void OnPause(bool paused)
        {
            if (paused)
            {
                comboTimeRecorder.Pause();
            }
            else
            {
                comboTimeRecorder.Resume();
            }
        }
        
        public Vector2 GetTilePositionByAddress(int address)
        {
            LinkLevelInfo.GetCoordinateByAddress(address, out var x, out var y);
            return GetTilePositionAt(x, y);
        }
        
        public Vector2 GetTilePositionAt(int x, int y)
        {
            var offsetX = 0f;
            if (x <= 0)
            {
                offsetX += tileSize.x * 0.5f;
            }
            else if (x > gridWidth)
            {
                offsetX -= tileSize.x * 0.5f;
            }
            
            var offsetY = 0f;
            if (y <= 0)
            {
                offsetY += tileSize.y * 0.5f;
            }
            else if (y > gridHeight)
            {
                offsetY -= tileSize.y * 0.5f;
            }
            return new Vector2(startPosition.x + (x - 1) * (tileSize.x + tileSpacing.x) + offsetX,
                startPosition.y - (y - 1) * (tileSize.y + tileSpacing.y) + offsetY);
        }

        public void OnTileSelected(LinkTile tile)
        {
            if (hintTileAddresses.Count > 0)
            {
                foreach (var address in hintTileAddresses)
                {
                    tiles.TryGetValue(address, out var hintTile);
                    hintTile!.OnHint(false);
                }
                hintTileAddresses.Clear();
            }
            
            if (null == selectedTile)
            {
                selectedTile = tile;
                SelectTile(selectedTile);
            }
            else if (tile == selectedTile)
            {
                selectedTile = null;
                DeselectTile(tile);
            }
            else
            {
                SelectTile(tile);

                var startTile = selectedTile;
                var startAddress = startTile.GetAddress();
                var targetAddress = tile.GetAddress();
                var result = solver.Solve(startAddress, targetAddress);
                if (result.success)
                {
                    isPairing = true;
                    totalRewardedMedals += result.path.Length;
                    
                    tile.state = TileState.AppendingSolved;
                    startTile.state = TileState.AppendingSolved;
                    levelInfo.Pair(startAddress, targetAddress);
                    var pathId = pathDrawer.DrawPath(result.path);
                    
                    OnTilesPaired();
                    
                    if (currentComboCount > 0)
                    {
                        comboTimeRecorder.Stop();
                        if (comboTimeRecorder.ElapsedTime <= Global.GameSettingsConfig.comboJudgementInterval)
                        {
                            currentComboCount++;
                            OnComboEnter();
                        }
                        else
                        {
                            currentComboCount = 0;
                            OnComboExit();
                        }
                    }
                    else
                    {
                        currentComboCount = 1;
                    }
                    
                    var sequence = DOTween.Sequence();
                    sequence.AppendInterval(0.2f);
                    sequence.AppendCallback(() =>
                    {
                        pathDrawer.RemovePath(pathId);
                        RemoveTile(startTile);
                        RemoveTile(tile);

                        if (levelInfo.remainingPairs <= 0)
                        {
                            GameOver();
                        }
                        else if (solver.TryMove())
                        {
                            StartMoving();
                        }
                        else
                        {
                            OnTilesPairCompleted();
                        }
                    });
                }
                else
                {
                    if (currentComboCount > 1)
                    {
                        comboTimeRecorder.Stop();
                        currentComboCount = 0;
                        OnComboExit();
                    }
                    else if (currentComboCount > 0)
                    {
                        comboTimeRecorder.Stop();
                        currentComboCount = 0;
                    }

                    var sequence = DOTween.Sequence();
                    sequence.AppendInterval(0.15f);
                    sequence.AppendCallback(() =>
                    {
                        DeselectTile(startTile);
                        DeselectTile(tile);
                    });

                    var randomness = Random.Range(50, 80);
                    sequence.Append(startTile.GetComponent<RectTransform>()
                        .DOShakeAnchorPos(0.15f, 10, 20, randomness));
                    sequence.Join(tile.GetComponent<RectTransform>()
                        .DOShakeAnchorPos(0.15f, 10, 20, randomness));
                }
                selectedTile = null;
            }
        }
        
        private void OnComboEnter()
        {
            if (maxComboCount < currentComboCount)
            {
                maxComboCount = currentComboCount;
            }
            
            comboAnimPlayer.ShowCombo(currentComboCount);
        }
        
        private void OnComboExit()
        {
        }

        private void RemoveTile(LinkTile tile)
        {
            tile.OnPairSuccess();
            
            var tileData = levelInfo.GetTileByAddress(tile.GetAddress());
            tiles.Remove(tileData.key);
            GameObject.Destroy(tile.gameObject);
        }

        private void OnTilesPaired()
        {
            playIndicator.SetRemainingPairs(levelInfo.remainingPairs);
        }

        public void Hint()
        {
            var result = solver.Hint();
            if (result.success)
            {
                var startAddress = result.path[0];
                var targetAddress = result.path[result.path.Length - 1];
                hintTileAddresses.Add(startAddress);
                hintTileAddresses.Add(targetAddress);

                var startTileData = levelInfo.GetTileByAddress(startAddress);
                tiles.TryGetValue(startTileData.key, out var startTile);
                startTile!.OnHint(true);

                var targetTileData = levelInfo.GetTileByAddress(targetAddress);
                tiles.TryGetValue(targetTileData.key, out var targetTile);
                targetTile!.OnHint(true);
            }
        }

        public void Shuffle()
        {
            if (selectedTile != null)
            {
                DeselectTile(selectedTile);
                selectedTile = null;
            }
            
            isShuffling = true;
            levelInfo.Shuffle();

            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.2f);
            foreach (var pair in tiles)
            {
                var address = levelInfo.GetTileAddressFromKey(pair.Key);
                var tile = pair.Value;
                var rectTransform = tile.GetComponent<RectTransform>();
                sequence.Insert(0, rectTransform.DOScale(0, 0.1f).OnComplete(() =>
                {
                    tile.SetAddress(address);
                    rectTransform.anchoredPosition = GetTilePositionByAddress(address);
                    tile.OnShuffle();
                }));
                sequence.Insert(0.1f, rectTransform.DOScale(1, 0.1f));
            }
            sequence.OnComplete(OnShuffleCompleted);
        }

        private void StartMoving()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.5f);
            foreach (var pair in tiles)
            {
                var tile = pair.Value;
                var address = levelInfo.GetTileAddressFromKey(pair.Key);
                if (address != tile.GetAddress())
                {
                    tile.SetAddress(address);
                    
                    var position = GetTilePositionByAddress(address);
                    var rectTransform = tile.GetComponent<RectTransform>();
                    sequence.Insert(0, rectTransform.DOAnchorPos(position, 0.5f));
                }
            }
            sequence.OnComplete(OnTilesMovingCompleted);
        }

        private void OnTilesMovingCompleted()
        {
            OnTilesPairCompleted();
        }

        private void OnTilesPairCompleted()
        {
            CheckDeadLock();
            isPairing = false;
            
            if (currentComboCount >= 1)
            {
                comboTimeRecorder.Start();
            }
        }
        
        private void OnShuffleCompleted()
        {
            isShuffling = false;
        }

        public void ChangeTheme()
        {
            LinkAssetsManager.Current.ChangeTheme();
            foreach (var pair in tiles)
            {
                switch (pair.Value.state)
                {
                    case TileState.AppendingSolved:
                    case TileState.Solved:
                        break;
                    
                    default:
                        pair.Value.OnThemeChanged();
                        break;
                }
            }
        }

        private void CheckDeadLock()
        {
            var result = solver.Hint();
            if (!result.success)
            {
                Shuffle();
            }
        }
        
        private void GameOver()
        {
            OnGameFinished?.Invoke();
        }
        
        private void SelectTile(LinkTile tile)
        {
            tile.OnSelected();
        }
        
        private void DeselectTile(LinkTile tile)
        {
            tile.OnDeselected();
        }
        
        public bool CanUseItem()
        {
            return !isShuffling && !isPairing;
        }
    }
}
