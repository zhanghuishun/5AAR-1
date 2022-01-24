using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

    /// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
    /// and overlays some information as well as the source Texture2D on top of the
    /// detected image.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ImageTrackingEventManager : MonoBehaviour
    {
        
        ARTrackedImageManager m_TrackedImageManager;
        public GameObject ARController;
        void Awake()
        {
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        void OnEnable()
        {
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        void OnDisable()
        {
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
        //eventFind: CA raise the words about "already find the sign" when find the sign
        //eventLost: jump to next scene, like go inside the shop(after tabacchi sign) or get a seat on the bus (validator)
        //TODO: use a TimeCounter to count how long does user switch between the states(find, regen, lost) to recognize their state

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (ARTrackedImage trackedImg in eventArgs.added)
            {
                if (trackedImg.trackingState == TrackingState.Tracking) {
                Debug.Log("find the sign");
                ARController.GetComponent<LogicFunctions>().ticketChecked = true;
                }
            }
            // foreach (ARTrackedImage trackedImg in eventArgs.updated)
            // {
            //     if (trackedImg.trackingState == TrackingState.Tracking) {
            //         if(ARController.GetComponent<LogicFunctions>().ticketChecked == false){
            //             Debug.Log();
            //         }
            //     }else{
            //         Debug.Log("arrow lost");
            //     }
            // }
            
        }
    }