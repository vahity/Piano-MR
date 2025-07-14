using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine;

public class ImageTracker : MonoBehaviour
{
    public GameObject prefabToPlace;
    private ARTrackedImageManager imageManager;

    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        imageManager.trackedImagesChanged += OnImageChanged;
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnImageChanged;
    }

    void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var newImage in args.added)
        {
            var obj = Instantiate(prefabToPlace, newImage.transform.position, newImage.transform.rotation);
            obj.transform.parent = newImage.transform; // ? ???????? ??: ???? ?? ?? ??? image
            spawnedObjects[newImage.referenceImage.name] = obj;
        }

        foreach (var updated in args.updated)
        {
            if (spawnedObjects.TryGetValue(updated.referenceImage.name, out GameObject obj))
            {
                obj.transform.position = updated.transform.position;
                obj.transform.rotation = updated.transform.rotation;
            }
        }

        foreach (var removed in args.removed)
        {
            if (spawnedObjects.TryGetValue(removed.referenceImage.name, out GameObject obj))
            {
                Destroy(obj);
                spawnedObjects.Remove(removed.referenceImage.name);
            }
        }
    }
}
