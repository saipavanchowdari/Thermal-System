using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Video;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnchancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.Touch;
using System;

public class Video : MonoBehaviour
{
    public float time = 5;
    private VideoPlayer player;
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 2f;
    private bool isDragging = false;
    private GameObject[] gameObjectArray = new GameObject[4];
    private float startScale;
    private GameObject draggedObject;
    private Vector3 offset;

    private void Start()
    {
        startScale = (float)0.9999999;
        player = GetComponent<VideoPlayer>();

        if (player == null)
        {
            Debug.Log("No Player");
        }
        else
        {
            Debug.Log("Yes player is set");
        }
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                //RaycastHit();
            }

            else if(Input.touchCount == 2)
            {
                //zoom();
            }
        }
    }

    public void forward()
    {
        player.time = player.time + time;
    }

    public void backward()
    {
        player.time = player.time - time;
    }

    public void stop()
    {
        player.Pause();
    }

    public void play()
    {
        player.Play();
    }
    
}
