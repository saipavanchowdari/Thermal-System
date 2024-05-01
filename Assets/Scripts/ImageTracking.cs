using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private GameObject spawnedPrefab;

    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            if (spawnedPrefab != null)
            {
                spawnedPrefab.SetActive(false);
            }
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        if (spawnedPrefab == null)
        {
            spawnedPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            spawnedPrefab.name = prefab.name;
        }

        Vector3 position = trackedImage.transform.position;
        spawnedPrefab.transform.position = position;
        spawnedPrefab.SetActive(true);
    }
}