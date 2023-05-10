using Common.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HomeTabButton : UITabButton
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Image labelImage;

        [Header("Sprites")]
        [SerializeField] private Sprite selectedBgSprite;
        [SerializeField] private Sprite selectedLabelSprite;
        [SerializeField] private Sprite unselectedBgSprite;
        [SerializeField] private Sprite unselectedLabelSprite;
        
        public override void Select()
        {
            bgImage.sprite = selectedBgSprite;
            bgImage.SetNativeSize();

            labelImage.sprite = selectedLabelSprite;
            labelImage.SetNativeSize();
        }

        public override void Deselect()
        {
            bgImage.sprite = unselectedBgSprite;
            bgImage.SetNativeSize();

            labelImage.sprite = unselectedLabelSprite;
            labelImage.SetNativeSize();
        }
    }
}
