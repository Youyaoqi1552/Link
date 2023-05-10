using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class UIToggle : Toggle
    {
        [SerializeField] private GameObject m_SelectObject = null;
        [SerializeField] private GameObject m_UnSelectObject = null;

        private bool m_IsListenerAdded;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (!m_IsListenerAdded)
            {
                onValueChanged.AddListener(_onValueChanged);
                m_IsListenerAdded = true;
            }
        }

        private void _onValueChanged(bool on)
        {
            if (m_SelectObject != null)
            {
                m_SelectObject.SetActive(on);
            }
            if (m_UnSelectObject != null)
            {
                m_UnSelectObject.SetActive(!on);
            }
        }
    }
}