using Common.Extensions;
using UnityEngine;

namespace Game.Link
{
    public class LinkTileSelection : MonoBehaviour
    {
        [SerializeField] private RectTransform particleTransform;
        [SerializeField] private float speed = 100;
        
        private Vector2 halfSizeDelta;
        private Direction currentDirection;
        private Vector2 currentPosition;

        public void Init(Vector2 size)
        {
            halfSizeDelta.Set(size.x * 0.5f, size.y * 0.5f);
            currentPosition.Set(-halfSizeDelta.x, -halfSizeDelta.y);
            currentDirection = Direction.Right;
            particleTransform.SetAnchoredPosition(currentPosition.x, currentPosition.y);
            
            particleTransform.gameObject.SetActive(true);
        }

        public void OnRecycle()
        {
            particleTransform.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            var dist = speed * Time.deltaTime;
            switch (currentDirection)
            {
                case Direction.Up:
                    currentPosition.y += dist;
                    if (currentPosition.y > halfSizeDelta.y)
                    {
                        currentPosition.y = halfSizeDelta.y;
                        currentDirection = Direction.Left;
                    }
                    break;
                
                case Direction.Down:
                    currentPosition.y -= dist;
                    if (currentPosition.y < -halfSizeDelta.y)
                    {
                        currentPosition.y = -halfSizeDelta.y;
                        currentDirection = Direction.Right;
                    }
                    break;
                
                case Direction.Left:
                    currentPosition.x -= dist;
                    if (currentPosition.x < -halfSizeDelta.x)
                    {
                        currentPosition.x = -halfSizeDelta.x;
                        currentDirection = Direction.Down;
                    }
                    break;
                
                case Direction.Right:
                    currentPosition.x += dist;
                    if (currentPosition.x > halfSizeDelta.x)
                    {
                        currentPosition.x = halfSizeDelta.x;
                        currentDirection = Direction.Up;
                    }
                    break;
                
                default:
                    break;
            }
            particleTransform.SetAnchoredPosition(currentPosition.x, currentPosition.y);
        }
    }
}
