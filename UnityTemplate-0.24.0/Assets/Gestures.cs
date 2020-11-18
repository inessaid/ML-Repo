using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using PubNubAPI;
public class Gestures : MonoBehaviour
{
    // PubNub Information 
    public float sendTimeController;
    public static PubNub pubnub;

    //ML Information
    public enum HandPoses { Fist, Thumb, Finger, NoPose };
    public HandPoses pose = HandPoses.NoPose;
    public Vector3[] pos;
    public GameObject sphereThumb, sphereIndex, sphereWrist;

    private MLHandTracking.HandKeyPose[] _gestures; // Holds the different hand poses we will look for.

    void Start()
    {
        //PubNub Initialization 
        PNConfiguration pnConfiguration = new PNConfiguration();
        pnConfiguration.PublishKey = "pub-c-903ac80d-16f0-4f6e-9fae-3cd0361930e1";
        pnConfiguration.SubscribeKey = "sub-c-30022d08-2225-11eb-bca3-12c13681665f";
        pnConfiguration.Secure = true;
        pubnub = new PubNub(pnConfiguration);

        //MLHandTracking Start
        MLHandTracking.Start(); // Start the hand tracking.
        _gestures = new MLHandTracking.HandKeyPose[3]; //Assign the gestures we will look for.
        _gestures[0] = MLHandTracking.HandKeyPose.Fist;
        _gestures[1] = MLHandTracking.HandKeyPose.Thumb;
        _gestures[2] = MLHandTracking.HandKeyPose.Finger;
        MLHandTracking.KeyPoseManager.EnableKeyPoses(_gestures, true, false); // Enable the hand poses.
        pos = new Vector3[3];
    }

    void OnDestroy()
    {
        MLHandTracking.Stop();
    }

    void Update()
    {
        if (sendTimeController <= Time.deltaTime)
        { // Restrict how often messages can be sent.
            if (GetGesture(MLHandTracking.Left, MLHandTracking.HandKeyPose.Thumb) || GetGesture(MLHandTracking.Right, MLHandTracking.HandKeyPose.Thumb))
            {
                pubnub.Publish()
                    .Channel("control")
                    .Message("on")
                    .Async((result, status) => {
                        if (!status.Error)
                        {
                            Debug.Log(string.Format("Publish Timetoken: {0}", result.Timetoken));
                        }
                        else
                        {
                            Debug.Log(status.Error);
                            Debug.Log(status.ErrorData.Info);
                        }
                    });
                sendTimeController = 0.1f; // Stop multiple messages from being sent.
            }
            else if (GetGesture(MLHandTracking.Left, MLHandTracking.HandKeyPose.Fist) || GetGesture(MLHandTracking.Right, MLHandTracking.HandKeyPose.Fist))
            {
                pubnub.Publish()
                    .Channel("control")
                    .Message("off")
                    .Async((result, status) => {
                        if (!status.Error)
                        {
                            Debug.Log(string.Format("Publish Timetoken: {0}", result.Timetoken));
                        }
                        else
                        {
                            Debug.Log(status.Error);
                            Debug.Log(status.ErrorData.Info);
                        }
                    });
                sendTimeController = 0.1f; // Stop multiple messages from being sent.
            }
            else if (GetGesture(MLHandTracking.Left, MLHandTracking.HandKeyPose.Finger))
            {
                pubnub.Publish()
                    .Channel("control")
                    .Message("changel")
                    .Async((result, status) => {
                        if (!status.Error)
                        {
                            Debug.Log(string.Format("Publish Timetoken: {0}", result.Timetoken));
                        }
                        else
                        {
                            Debug.Log(status.Error);
                            Debug.Log(status.ErrorData.Info);
                        }
                    });
                sendTimeController = 0.9f; // Stop multiple messages from being sent.
            }
            else if (GetGesture(MLHandTracking.Right, MLHandTracking.HandKeyPose.Finger))
            {
                pubnub.Publish()
                    .Channel("control")
                    .Message("changer")
                    .Async((result, status) => {
                        if (!status.Error)
                        {
                            Debug.Log(string.Format("Publish Timetoken: {0}", result.Timetoken));
                        }
                        else
                        {
                            Debug.Log(status.Error);
                            Debug.Log(status.ErrorData.Info);
                        }
                    });
                sendTimeController = 0.9f; // Stop multiple messages from being sent.
            }
        }
        else
        {
            sendTimeController -= Time.deltaTime; // Update the timer.
        }

        if (pose != HandPoses.NoPose) ShowPoints();
    }

    void ShowPoints()
    {
        // Left Hand Thumb tip
        pos[0] = MLHandTracking.Left.Thumb.KeyPoints[2].Position;
        // Left Hand Index finger tip 
        pos[1] = MLHandTracking.Left.Index.KeyPoints[2].Position;
        // Left Hand Wrist 
        pos[2] = MLHandTracking.Left.Wrist.KeyPoints[0].Position;
        sphereThumb.transform.position = pos[0];
        sphereIndex.transform.position = pos[1];
        sphereWrist.transform.position = pos[2];
    }

    bool GetGesture(MLHandTracking.Hand hand, MLHandTracking.HandKeyPose type)
    {
        if (hand != null)
        {
            if (hand.KeyPose == type)
            {
                if (hand.HandKeyPoseConfidence > 0.98f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}