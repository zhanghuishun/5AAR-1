using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID
        RectTransform rectTransorm = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;

        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransorm.anchorMin = minAnchor;
        rectTransorm.anchorMax = maxAnchor;
#endif
    }
}
