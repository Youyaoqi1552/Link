using System.Collections.Generic;
using Common.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Game.Link
{
    public class LinkPathDrawer : MonoBehaviour
    {
        [SerializeField] private RectTransform lineContainer;
        [SerializeField] private LineRenderer linePrefab;
        
        [SerializeField] private RectTransform medalContainer;
        [SerializeField] private RectTransform medalPrefab;
        [SerializeField] private RectTransform medalTarget;
        
        private Dictionary<long, PathItem> pathItems = new Dictionary<long, PathItem>();
        private PlayerController grid;
        private long idGenerator;
        private Vector3 targetPosition;
        
        public void SetGrid(PlayerController grid)
        {
            this.grid = grid;
            targetPosition = medalTarget.position;
        }

        public void Dispose()
        {
            foreach (var pathItem  in pathItems.Values)
            {
                pathItem.tweener.Kill();
                foreach (var medal in pathItem.medals)
                {
                    GameObject.Destroy(medal.gameObject);
                }
                pathItem.medals.Clear();
            }
            pathItems.Clear();
        }
        
        public long DrawPath(int[] path)
        {
            var pathItem = new PathItem();
            
            var pathId = ++idGenerator;
            pathItem.line = Instantiate(linePrefab, lineContainer);
            var lineRect = pathItem.line.GetComponent<RectTransform>();
            lineRect.SetAnchoredPosition(0, 0);
            
            pathItem.line.positionCount = path.Length;
            pathItem.medals = new List<RectTransform>();
            
            for (var i = 0; i < path.Length; i++)
            {
                var pos = grid.GetTilePositionByAddress(path[i]);
                pathItem.line.SetPosition(i, new Vector3(pos.x, pos.y, 0));
                
                var medal = Instantiate(medalPrefab, medalContainer);
                medal.SetAnchoredPosition(pos);
                pathItem.medals.Add(medal);
            }

            pathItems[pathId] = pathItem;
            return pathId;
        }

        public void RemovePath(long pathId)
        {
            if (pathItems.TryGetValue(pathId, out var pathItem))
            {
                pathItem.line.positionCount = 0;
                GameObject.Destroy(pathItem.line.gameObject);

                var duration = 0.5f;
                var sequence = DOTween.Sequence();
                sequence.AppendInterval(duration);
                foreach (var medal in pathItem.medals)
                {
                    sequence.Insert(0, medal.DOMove(targetPosition, duration));
                }
                
                sequence.OnComplete(() =>
                {
                    pathItem.tweener.Kill();
                    foreach (var medal in pathItem.medals)
                    {
                        GameObject.Destroy(medal.gameObject);
                    }
                    pathItem.medals.Clear();
                    pathItems.Remove(pathId);
                });
                pathItem.tweener = sequence;
            }
        }

        private class PathItem
        {
            public LineRenderer line;
            public List<RectTransform> medals;
            public Sequence tweener;
        }
    }
}
