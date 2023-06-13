using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneDetectionController : MonoBehaviour
{
    [SerializeField] private GameObject planePrefab;

    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private Dictionary<TrackableId, GameObject> visualizedPlanes = new Dictionary<TrackableId, GameObject>();

    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        // Perform plane detection
        if (arRaycastManager.Raycast(new Vector2(Screen.width / 2f, Screen.height / 2f), raycastHits, TrackableType.PlaneWithinPolygon))
        {
            // Instantiate or update visualized planes
            foreach (var hit in raycastHits)
            {
                if (!visualizedPlanes.ContainsKey(hit.trackableId))
                {
                    // Instantiate new plane prefab
                    var planeObject = Instantiate(planePrefab, hit.pose.position, hit.pose.rotation);
                    visualizedPlanes.Add(hit.trackableId, planeObject);
                }
                else
                {
                    // Update existing plane prefab
                    var planeObject = visualizedPlanes[hit.trackableId];
                    planeObject.transform.position = hit.pose.position;
                    planeObject.transform.rotation = hit.pose.rotation;
                }
            }
        }

        // Remove visualized planes that are no longer detected
        var planeIdsToRemove = new List<TrackableId>();
        foreach (var planeId in visualizedPlanes.Keys)
        {
            if (!ContainsTrackableId(raycastHits, planeId))
            {
                Destroy(visualizedPlanes[planeId]);
                planeIdsToRemove.Add(planeId);
            }
        }

        foreach (var planeId in planeIdsToRemove)
        {
            visualizedPlanes.Remove(planeId);
        }
    }

    private bool ContainsTrackableId(List<ARRaycastHit> hits, TrackableId id)
    {
        foreach (var hit in hits)
        {
            if (hit.trackableId == id)
            {
                return true;
            }
        }
        return false;
    }
}

