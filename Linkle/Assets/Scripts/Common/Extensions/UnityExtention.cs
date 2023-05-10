using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Extensions
{
    public static class UnityExtension
    {
        #region Active

        public static void SetActive(this Transform trans, bool isActive)
        {
            trans.gameObject.SetActive(isActive);
        }

        #endregion

        #region LocalPosition
        
        public static void GetLocalPosition(this Transform trans, out float x, out float y, out float z)
        {
            var pos = trans.localPosition;
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }
        
        public static void SetLocalPositionX(this Transform trans, float x)
        {
            var pos = trans.localPosition;
            pos.x = x;
            trans.localPosition = pos;
        }

        public static void SetLocalPositionY(this Transform trans, float y)
        {
            var pos = trans.localPosition;
            pos.y = y;
            trans.localPosition = pos;
        }

        public static void SetLocalPositionZ(this Transform trans, float z)
        {
            var pos = trans.localPosition;
            pos.z = z;
            trans.localPosition = pos;
        }
        
        public static void SetLocalPositionXY(this Transform trans, float x, float y)
        {
            var pos = trans.localPosition;
            pos.x = x;
            pos.y = y;
            trans.localPosition = pos;
        }

        public static void SetLocalPosition(this Transform trans, float x, float y, float z)
        {
            var pos = trans.localPosition;
            pos.Set(x, y, z);
            trans.localPosition = pos;
        }
        
        public static void SetLocalPosition(this Transform trans, Vector3 pos)
        {
            trans.localPosition = pos;
        }

        public static void AddLocalPositionX(this Transform trans, float delta)
        {
            var pos = trans.localPosition;
            pos.x += delta;
            trans.localPosition = pos;
        }
        
        public static void AddLocalPositionY(this Transform trans, float delta)
        {
            var pos = trans.localPosition;
            pos.y += delta;
            trans.localPosition = pos;
        }
        
        public static void AddLocalPositionZ(this Transform trans, float delta)
        {
            var pos = trans.localPosition;
            pos.z += delta;
            trans.localPosition = pos;
        }
        
        public static void AddLocalPositionXY(this Transform trans, float deltaX, float deltaY)
        {
            var pos = trans.localPosition;
            pos.x += deltaX;
            pos.y += deltaY;
            trans.localPosition = pos;
        }
        
        public static void AddLocalPosition(this Transform trans, float deltaX, float deltaY, float deltaZ)
        {
            var pos = trans.localPosition;
            pos.x += deltaX;
            pos.y += deltaY;
            pos.z += deltaZ;
            trans.localPosition = pos;
        }

        public static void AddLocalPosition(this Transform trans, Vector3 delta)
        {
            var pos = trans.localPosition;
            pos.x += delta.x;
            pos.y += delta.y;
            pos.z += delta.z;
            trans.localPosition = pos;
        }
        
        #endregion

        #region anchoredPosition

        public static void GetAnchoredPosition(this Transform trans, out float x, out float y)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                x = 0;
                y = 0;
                return;
            }

            var anchoredPosition = rectTrans.anchoredPosition;
            x = anchoredPosition.x;
            y = anchoredPosition.y;
        }
        
        public static float GetAnchoredPositionX(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 0;
            }
            return rectTrans.anchoredPosition.x;
        }

        public static float GetAnchoredPositionY(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 0;
            }
            return rectTrans.anchoredPosition.y;
        }

        public static void SetAnchoredPositionX(this Transform trans, float x)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.x = x;
            rectTrans.anchoredPosition = anchoredPosition;
        }
        
        public static void SetAnchoredPositionY(this Transform trans, float value)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.y = value;
            rectTrans.anchoredPosition = anchoredPosition;
        }
        
        public static void SetAnchoredPosition(this Transform trans, float x, float y)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.Set(x, y);
            rectTrans.anchoredPosition = anchoredPosition;
        }
        
        public static void SetAnchoredPosition(this Transform trans, Vector2 anchoredPosition)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            rectTrans.anchoredPosition = anchoredPosition;
        }

        public static void AddAnchoredPositionX(this Transform trans, float delta)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.x += delta;
            rectTrans.anchoredPosition = anchoredPosition;
        }
        
        public static void AddAnchoredPositionY(this Transform trans, float delta)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.y += delta;
            rectTrans.anchoredPosition = anchoredPosition;
        }
        
        public static void AddAnchoredPosition(this Transform trans, float deltaX, float deltaY)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.x += deltaX;
            anchoredPosition.y += deltaY;
            rectTrans.anchoredPosition = anchoredPosition;
        }
        
        public static void AddAnchoredPosition(this Transform trans, Vector2 delta)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.x += delta.x;
            anchoredPosition.y += delta.y;
            rectTrans.anchoredPosition = anchoredPosition;
        }

        #endregion

        #region Position

        public static void GetPosition(this Transform trans, out float x, out float y, out float z)
        {
            var pos = trans.position;
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public static float GetPositionX(this Transform trans)
        {
            return trans.position.x;
        }
        
        public static float GetPositionY(this Transform trans)
        {
            return trans.position.y;
        }
        
        public static float GetPositionZ(this Transform trans)
        {
            return trans.position.z;
        }

        public static void SetPositionX(this Transform trans, float x)
        {
            var pos = trans.position;
            pos.x = x;
            trans.position = pos;
        }

        public static void SetPositionY(this Transform trans, float y)
        {
            var pos = trans.position;
            pos.y = y;
            trans.position = pos;
        }
        
        public static void SetPositionZ(this Transform trans, float z)
        {
            var pos = trans.position;
            pos.z = z;
            trans.position = pos;
        }
        
        public static void SetPositionXY(this Transform trans, float x, float y)
        {
            var pos = trans.position;
            pos.x = x;
            pos.y = y;
            trans.position = pos;
        }
        
        public static void SetPosition(this Transform trans, float x, float y, float z)
        {
            var pos = trans.position;
            pos.Set(x, y, z);
            trans.position = pos;
        }
        
        public static void SetPosition(this Transform trans, Vector3 pos)
        {
            trans.position = pos;
        }

        public static void AddPositionX(this Transform trans, float delta)
        {
            var pos = trans.position;
            pos.x += delta;
            trans.position = pos;
        }
        
        public static void AddPositionY(this Transform trans, float delta)
        {
            var pos = trans.position;
            pos.y += delta;
            trans.position = pos;
        }
        
        public static void AddPositionZ(this Transform trans, float delta)
        {
            var pos = trans.position;
            pos.z += delta;
            trans.position = pos;
        }
        
        public static void AddPositionXY(this Transform trans, float deltaX, float deltaY)
        {
            var pos = trans.position;
            pos.x += deltaX;
            pos.y += deltaY;
            trans.position = pos;
        }
        
        public static void AddPosition(this Transform trans, float deltaX, float deltaY, float deltaZ)
        {
            var pos = trans.position;
            pos.x += deltaX;
            pos.y += deltaY;
            pos.z += deltaZ;
            trans.position = pos;
        }
        
        public static void AddPosition(this Transform trans, Vector3 delta)
        {
            var pos = trans.position;
            pos.x += delta.x;
            pos.y += delta.y;
            pos.z += delta.z;
            trans.position = pos;
        }
        
        #endregion

        #region Size

        public static float GetSizeX(this RectTransform trans)
        {
            GetSize(trans, out var width, out var _);
            return width;
        }

        public static float GetSizeY(this RectTransform trans)
        {
            GetSize(trans, out var _, out var height);
            return height;
        }
        
        public static void GetSize(this RectTransform trans, out float width, out float height)
        {
            if (!trans)
            {
                width = 0;
                height = 0;
                return;
            }

            var rect = trans.rect;
            width = rect.width;
            height = rect.height;
        }
        
        public static void SetSizeX(this RectTransform trans, float width)
        {
            GetSize(trans, out var _, out var height);
            SetSize(trans, width, height);
        }

        public static void SetSizeY(this RectTransform trans, float height)
        {
            GetSize(trans, out var width, out var _);
            SetSize(trans, width, height);
        }

        public static void SetSize(this RectTransform trans, float width, float height)
        {
            GetSize(trans.parent as RectTransform, out var parentWidth, out var parentHeight);
            
            var acMin = trans.anchorMin;
            var acMax = trans.anchorMax;

            var acMinX = acMin.x;
            var acMinY = acMin.y;
            var acMaxX = acMax.x;
            var acMaxY = acMax.y;
            
            if (!acMinX.Equals(acMaxX))
            {
                width -= parentWidth * (acMaxX - acMinX); // 向内是负的，向外是正的
            }
            if (!acMinY.Equals(acMaxY))
            {
                height -= parentHeight * (acMaxY - acMinY); // 向内是负的，向外是正的
            }
            trans.sizeDelta = new Vector2(width, height);
        }

        #endregion

        #region Scale

        public static float GetLocalScaleX(this Transform trans)
        {
            return trans.localScale.x;
        }

        public static float GetLocalScaleY(this Transform trans)
        {
            return trans.localScale.y;
        }

        public static float GetLocalScaleZ(this Transform trans)
        {
            return trans.localScale.z;
        }
        
        public static void GetLocalScale(this Transform trans, out float x, out float y, out float z)
        {
            var localScale = trans.localScale;
            x = localScale.x;
            y = localScale.y;
            z = localScale.z;
        }
        
        public static void SetLocalScaleX(this Transform trans, float value)
        {
            var localScale = trans.localScale;
            localScale.x = value;
            trans.localScale = localScale;
        }

        public static void SetLocalScaleY(this Transform trans, float value)
        {
            var localScale = trans.localScale;
            localScale.y = value;
            trans.localScale = localScale;
        }

        public static void SetLocalScaleZ(this Transform trans, float value)
        {
            var localScale = trans.localScale;
            localScale.z = value;
            trans.localScale = localScale;
        }
        
        public static void SetLocalScaleXY(this Transform trans, float scaleX, float scaleY)
        {
            var localScale = trans.localScale;
            localScale.x = scaleX;
            localScale.y = scaleY;
            trans.localScale = localScale;
        }
        
        public static void SetLocalScale(this Transform trans, float value)
        {
            trans.localScale = value * Vector3.one;
        }
        
        public static void SetLocalScale(this Transform trans, float x, float y, float z)
        {
            trans.localScale = new Vector3(x, y, z);
        }
        
        public static void SetLocalScale(this Transform trans, Vector3 scale)
        {
            trans.localScale = scale;
        }

        #endregion

        #region Anchors
        
        public static float GetAnchorsMinX(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 0;
            }
            return rectTrans.anchorMin.x;
        }
        
        public static void SetAnchorsMinX(this Transform trans, float value)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var pos = rectTrans.anchorMin;
            pos.x = value;
            rectTrans.anchorMin = pos;
        }

        public static float GetAnchorsMinY(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 0;
            }
            return rectTrans.anchorMin.y;
        }
        
        public static void SetAnchorsMinY(this Transform trans, float value)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var pos = rectTrans.anchorMin;
            pos.y = value;
            rectTrans.anchorMin = pos;
        }

        public static float GetAnchorsMaxX(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 1;
            }
            return rectTrans.anchorMax.x;
        }
        
        public static void SetAnchorsMaxX(this Transform trans, float value)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var pos = rectTrans.anchorMax;
            pos.x = value;
            rectTrans.anchorMax = pos;
        }

        public static float GetAnchorsMaxY(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 1;
            }
            return rectTrans.anchorMax.y;
        }
        
        public static void SetAnchorsMaxY(this Transform trans, float value)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var pos = rectTrans.anchorMax;
            pos.y = value;
            rectTrans.anchorMax = pos;
        }
        
        public static void GetAnchorsMinXY(this Transform trans, out float minX, out float minY)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                minX = 1;
                minY = 1;
                return;
            }

            var anchorMax = rectTrans.anchorMin;
            minX = anchorMax.x;
            minY = anchorMax.y;
        }
        
        public static void SetAnchorsMinXY(this Transform trans, float minX, float minY)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            
            var min = rectTrans.anchorMin;
            min.Set(minX, minY);
            rectTrans.anchorMin = min;
        }
        
        public static void GetAnchorsMaxXY(this Transform trans, out float maxX, out float maxY)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                maxX = 1;
                maxY = 1;
                return;
            }

            var anchorMax = rectTrans.anchorMax;
            maxX = anchorMax.x;
            maxY = anchorMax.y;
        }
        
        public static void SetAnchorsMaxXY(this Transform trans, float maxX, float maxY)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var max = rectTrans.anchorMax;
            max.Set(maxX, maxY);
            rectTrans.anchorMax = max;
        }

        public static void SetAnchors(this Transform trans, float minX, float minY, float maxX, float maxY)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var min = rectTrans.anchorMin;
            min.Set(minX, minY);
            rectTrans.anchorMin = min;

            var max = rectTrans.anchorMax;
            max.Set(maxX, maxY);
            rectTrans.anchorMax = max;
        }

        #endregion

        #region pivot

        public static float GetPivotX(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 0.5f;
            }
            return rectTrans.pivot.x;
        }
        
        public static float GetPivotY(this Transform trans)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return 0.5f;
            }
            return rectTrans.pivot.y;
        }
        
        public static void GetPivot(this Transform trans, out float x, out float y)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                x = y = 0.5f;
                return;
            }

            var pivot = rectTrans.pivot;
            x = pivot.x;
            y = pivot.y;
        }
        
        public static void SetPivotX(this Transform trans, float value)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var pos = rectTrans.pivot;
            pos.x = value;
            rectTrans.pivot = pos;
        }

        public static void SetPivotY(this Transform trans, float value)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }

            var pos = rectTrans.pivot;
            pos.y = value;
            rectTrans.pivot = pos;
        }

        public static void SetPivot(this Transform trans, float x, float y)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            rectTrans.pivot = new Vector2(x, y);
        }
        
        public static void SetPivot(this Transform trans, Vector2 pivot)
        {
            var rectTrans = trans as RectTransform;
            if (rectTrans == null)
            {
                return;
            }
            rectTrans.pivot = pivot;
        }

        #endregion

        #region EulerRotation

        public static float GetLocalEulerRotationX(this Transform trans)
        {
            return trans.localRotation.eulerAngles.x;
        }

        public static float GetLocalEulerRotationY(this Transform trans)
        {
            return trans.localRotation.eulerAngles.y;
        }

        public static float GetLocalEulerRotationZ(this Transform trans)
        {
            return trans.localRotation.eulerAngles.z;
        }
        
        public static void GetLocalEulerRotation(this Transform trans, out float x, out float y, out float z)
        {
            var eulerAngles = trans.localRotation.eulerAngles;
            x = eulerAngles.x;
            y = eulerAngles.y;
            z = eulerAngles.z;
        }
        
        public static void SetLocalEulerRotationX(this Transform trans, float x)
        {
            var eulerAngles = trans.localRotation.eulerAngles;
            eulerAngles.x = x;
            trans.localRotation = Quaternion.Euler(eulerAngles);
        }

        public static void SetLocalEulerRotationY(this Transform trans, float y)
        {
            var eulerAngles = trans.localRotation.eulerAngles;
            eulerAngles.y = y;
            trans.localRotation = Quaternion.Euler(eulerAngles);
        }

        public static void SetLocalEulerRotationZ(this Transform trans, float z)
        {
            var eulerAngles = trans.localRotation.eulerAngles;
            eulerAngles.z = z;
            trans.localRotation = Quaternion.Euler(eulerAngles);
        }
        
        public static void SetLocalEulerRotationXY(this Transform trans, float x, float y)
        {
            var eulerAngles = trans.localRotation.eulerAngles;
            eulerAngles.x = x;
            eulerAngles.y = y;
            trans.localRotation = Quaternion.Euler(eulerAngles);
        }
        
        public static void SetLocalEulerRotation(this Transform trans, float x, float y, float z)
        {
            var eulerAngles = trans.localRotation.eulerAngles;
            eulerAngles.Set(x, y, z);
            trans.localRotation = Quaternion.Euler(eulerAngles);
        }
        
        public static void SetLocalEulerRotation(this Transform trans, Vector3 eulerAngles)
        {
            trans.localRotation = Quaternion.Euler(eulerAngles);
        }

        #endregion
        
        #region Image
        
        public static float GetAlpha(this Image image)
        {
            return image.color.a;
        }
        
        public static void SetAlpha(this Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
        
        #endregion

        #region RawImage

        public static float GetAlpha(this RawImage image)
        {
            return image.color.a;
        }
        
        public static void SetAlpha(this RawImage rawImage, float alpha)
        {
            var color = rawImage.color;
            color.a = alpha;
            rawImage.color = color;
        }

        #endregion
        
        #region Text
        
        public static float GetAlpha(this Text textComp)
        {
            return textComp.color.a;
        }
        
        public static void SetAlpha(this Text textComp, float alpha)
        {
            var color = textComp.color;
            color.a = alpha;
            textComp.color = color;
        }
        
        public static float GetAlpha(this TextMeshProUGUI textComp)
        {
            return textComp.color.a;
        }
        
        public static void SetAlpha(this TextMeshProUGUI textComp, float alpha)
        {
            var color = textComp.color;
            color.a = alpha;
            textComp.color = color;
        }

        #endregion
        
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}