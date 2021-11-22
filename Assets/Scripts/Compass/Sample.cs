using UnityEngine;

namespace Sample
{
    public class Sample : MonoBehaviour
    {
        [SerializeField] private UnityARCompass.ARCompassIOS arCompassIOS;
        [SerializeField] private Transform compassObject;
        public GameObject ARCamera;
        private int forwardOffset = 1;
        private int upOffset = 1;

        private void Start()
        {
        }
        private void Update()
        {
            compassObject.rotation = arCompassIOS.TrueHeadingRotation;
            //the position is always on the front of camera with a offset
            compassObject.position = ARCamera.transform.position + ARCamera.transform.forward * forwardOffset;

        }
    }
}