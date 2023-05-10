using Common.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Link
{
    public class LinkTile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform hintTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private PlayerController owner;
        private RectTransform selfRectTransform;
        public TileData tileData;
        [HideInInspector] public TileState state;
        private Vector2 currentSize;
        private LinkTileSelection tileSelection;
        private int address = -1;

        public void SetContext(PlayerController owner, Vector2 size, float scale)
        {
            this.owner = owner;
            currentSize = size;
            content.localScale = Vector3.one * scale;
            selfRectTransform = GetComponent<RectTransform>();
            
            state = TileState.Normal;
            canvasGroup.alpha = 1;
            
            TryUpdateCellBg();
        }
        
        public int GetAddress()
        {
            return address;
        }
        
        public void SetAddress(int address)
        {
            this.address = address;
        }

        public void SetData(TileData tileData)
        {
            this.tileData = tileData;
            
            icon.sprite = LinkAssetsManager.Current.GetThemeSprite(tileData.tag);
            icon.SetNativeSize();
        }
        
        public void OnThemeChanged()
        {
            icon.sprite = LinkAssetsManager.Current.GetThemeSprite(tileData.tag);
            icon.SetNativeSize();

            TryUpdateCellBg();
        }

        private void TryUpdateCellBg()
        {
            var bgSprite = LinkAssetsManager.Current.GetThemeSprite("cell_bg");
            if (null == bgSprite)
            {
                bgImage.sprite = LinkAssetsManager.Current.GetAtlasSprite("cell_bg");
            }
            else
            {
                bgImage.sprite = bgSprite;
            }
            bgImage.SetNativeSize();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            owner.OnTileSelected(this);
        }

        public void OnSelected()
        {
            canvasGroup.alpha = 1;
            tileSelection = LinkAssetsManager.Current.AllocateTileSelection(owner.selectionLayer);
            var selectionRect = tileSelection.GetComponent<RectTransform>();
            selectionRect.anchoredPosition = selfRectTransform.anchoredPosition;
            tileSelection.Init(currentSize);
        }
        
        public void OnDeselected()
        {
            LinkAssetsManager.Current.ReleaseTileSelection(tileSelection);
            tileSelection = null;
            canvasGroup.alpha = 1;
        }
        
        public void OnHint(bool hint)
        {
            if (hint)
            {
                canvasGroup.alpha = 0.6f;
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }
        
        public void OnShuffle()
        {
            canvasGroup.alpha = 1f;
        }
        
        public void OnPairSuccess()
        {
            if (null != tileSelection)
            {
                LinkAssetsManager.Current.ReleaseTileSelection(tileSelection);
                tileSelection = null;
            }
        }
    }
}
