using Common.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    [RequireComponent(typeof(Text))]
    [ExecuteAlways]
    public class UITextShrinker : MonoBehaviour
    {
        private Text m_Text;
        private RectTransform m_RectTransform;

        [SerializeField]
        private float m_DefaultWidth = 800;

        [SerializeField]
        private int m_ShrinkLength = 12;

        private void Awake()
        {
            m_Text = GetComponent<Text>();
            m_RectTransform = GetComponent<RectTransform>();
            Refresh();
        }
        
        public void Refresh()
        {
            m_Text.horizontalOverflow = HorizontalWrapMode.Overflow;
            var length = m_Text.text.Length;
            if (length < m_ShrinkLength)
            {
                m_RectTransform.localScale = Vector3.one;
                m_RectTransform.SetWidth(m_DefaultWidth);
            }
            else
            {
                var scale = (m_ShrinkLength - 1) / (float)length;
                m_RectTransform.localScale = Vector3.one * scale;
                m_RectTransform.SetWidth(m_DefaultWidth * 1 / scale);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                Refresh();
            }
        }
#endif
    }
}