using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAndroid : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }
}
