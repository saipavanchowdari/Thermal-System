using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnchancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.Touch;

public class BladeHub : MonoBehaviour
{

    [SerializeField]
    private Transform placeholder;
    private int touch_count = 0;
    GameObject videoInst = null;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField]
    private GameObject[] gameObjectArray = new GameObject[4];
    [SerializeField]
    private GameObject[] videoArray = new GameObject[4];
    private Dictionary<string, GameObject> VideoInstructions;
    private Dictionary<string, Vector3> originalScales;
    private bool isDragging = false;
    private Vector3 offset;
    private GameObject draggedObject;
    private bool isZooming = false;
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 2f;
    private float scaleFactor = 1.1f;
    private float startScale = (float)0.9999999;
    private string previousHit = "";

    void Awake()
    {

        VideoInstructions = new Dictionary<string, GameObject>();
        originalScales = new Dictionary<string, Vector3>();

        int i = 0;
        foreach (GameObject obj in gameObjectArray)
        {

            string tag = obj.tag;
            VideoInstructions[tag] = videoArray[i];
            originalScales[tag] = obj.transform.localScale;
            i++;


        }

    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        RaycastHit(touch.position);
                        break;
                    case TouchPhase.Moved:
                        if (isDragging)
                            OnTouchMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                        OnTouchEnded();
                        break;
                }
            }
            else if (Input.touchCount == 2)
            {
                // Assuming you have a method to handle zooming here
                //zoom();
                // Or directly call the RotateObjectWithTwoFingers method if zooming is not a separate action
                //RotateObjectWithTwoFingers();
            }
        }
    }

    /*  private void RotateObjectWithTwoFingers()
      {
          if (isZooming) return; // Skip rotation if zooming is in progress

          Touch touch1                                              = Input.GetTouch(0);
          Touch touch2                                              = Input.GetTouch(1);

          if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
          {
              Vector2 initialPosition1                              = touch1.position - touch1.deltaPosition;
              Vector2 initialPosition2                              = touch2.position - touch2.deltaPosition;
              Vector2 currentPosition1                              = touch1.position;
              Vector2 currentPosition2                              = touch2.position;

              // Calculate the initial and current vectors
              Vector2 initialVector                                 = initialPosition2 - initialPosition1;
              Vector2 currentVector                                 = currentPosition2 - currentPosition1;

              // Calculate the angle between the initial and current vectors
              float initialAngle                                    = Mathf.Atan2(initialVector.y, initialVector.x);
              float currentAngle                                    = Mathf.Atan2(currentVector.y, currentVector.x);

              // Calculate the difference in angle
              float angleDifference                                 = currentAngle - initialAngle;

              // Rotate the object
              draggedObject.transform.Rotate(Vector3.up, angleDifference * Mathf.Rad2Deg);
          }
      }

      private void zoom()
      {
          if (Input.touchCount == 2)
          {
              isZooming                                             = true; // Set flag to true when zooming starts

              Touch touchZero                                       = Input.GetTouch(0);
              Touch touchOne                                        = Input.GetTouch(1);

              Vector2 touchZeroPrevPos                              = touchZero.position - touchZero.deltaPosition;
              Vector2 touchOnePrevPos                               = touchOne.position - touchOne.deltaPosition;

              float prevTouchDeltaMag                               = (touchZeroPrevPos - touchOnePrevPos).magnitude;
              float touchDeltaMag                                   = (touchZero.position - touchOne.position).magnitude;

              float deltaMagnitudeDiff                              = prevTouchDeltaMag - touchDeltaMag;

              // Adjust the scale of the GameObject based on pinch gesture
              float newScale                                        = startScale + deltaMagnitudeDiff * zoomSpeed;
              newScale                                              = Mathf.Clamp(newScale, minScale, maxScale);
              transform.localScale                                  = new Vector3(newScale, newScale, newScale);

              isZooming                                             = false; // Set flag to false when zooming ends
          }
      }*/



    void RaycastHit(Vector2 touchPosition)
    {
        Touch touch = Input.GetTouch(0); // Assuming only one touch for simplicity
        if (touch.phase == TouchPhase.Began)
        {
            // Update touchPosition if needed
            // touchPosition = touch.position;
        }

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            touch_count++;
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != null)
            {
                string objectTag = hitObject.tag;

                if (objectTag == "Motor" || objectTag == "Screw" || objectTag == "Bench" || objectTag == "Pipe")
                {
                    Debug.Log("Hit Machine with tag: " + objectTag);

                    if (videoInst)
                    {
                        Debug.Log("Names " + previousHit + " " + objectTag);
                        if (objectTag != previousHit)
                        {
                            //Debug.Log("CHECK");
                            Destroy(videoInst);
                            videoInst = Instantiate(VideoInstructions[objectTag], placeholder.position, Quaternion.identity);
                            //Debug.Log("Inst object is in not NULL " + VideoInstructions[objectTag].name);
                            previousHit = objectTag; 
                        }
                        else
                        {
                            //Debug.Log("Same");
                        }
                    }
                    else
                    {
                        previousHit = objectTag; 
                        videoInst = Instantiate(VideoInstructions[objectTag], placeholder.position, Quaternion.identity);
                        //Debug.Log("Inst object is " + VideoInstructions[objectTag].name);
                    }


                    hitObject.transform.localScale *= scaleFactor;
                    draggedObject = hitObject;
                    isDragging = true;

                    offset = hitObject.transform.position - GetWorldPosition(touchPosition);

                    if (touch_count == 1)
                    {
                        hitObject.transform.localScale *= scaleFactor;
                    }
                    else
                    {
                        if (hitObject.tag == "Motor" || hitObject.tag == "Screw" || hitObject.tag == "Bench" || hitObject.tag == "Pipe" || hitObject.tag == "videoInst")
                        {
                            hitObject.transform.localScale = originalScales[hitObject.tag];
                            touch_count = 0;
                        }
                    }
                }
                else if (objectTag == "videoInst")
                {
                    Debug.Log("Hit Machine with tag: " + objectTag);
                    draggedObject = hitObject;
                    isDragging = true;
                    offset = hitObject.transform.position - GetWorldPosition(touchPosition);
                }

            }
            else
            {
                Debug.Log("Ray hit a feature point but not a GameObject.");
            }
        }
        else
        {
            Debug.Log("No raycast hit.");
        }
    }

    private void OnTouchMoved(Vector2 touchPosition)
    {
        Vector3 newPosition = GetWorldPosition(touchPosition) + offset;
        draggedObject.transform.position = newPosition;
    }

    private void OnTouchEnded()
    {
        isDragging = false;
        draggedObject = null;
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 worldPosition = screenPosition;
        worldPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(worldPosition);
    }
}