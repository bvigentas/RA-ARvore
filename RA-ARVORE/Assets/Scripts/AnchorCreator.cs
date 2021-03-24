using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator : MonoBehaviour
{
    [SerializeField]
    GameObject m_Prefab;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    IDictionary<ARAnchor, Detector.BoundingBox> anchorDic = new Dictionary<ARAnchor, Detector.BoundingBox>();
    private List<Detector.BoundingBox> boxSavedOutlines;
    private float shiftX;
    private float shiftY;
    private float scaleFactor;

    public ARCamera aRCamera;
    public ARRaycastManager m_RaycastManager;
    public TextMesh anchorObj_mesh;
    public ARAnchorManager m_AnchorManager;

    public GameObject prefab
    {
        get => m_Prefab;
        set => m_Prefab = value;
    }

    public void RemoveAllAnchors()
    {
        Debug.Log($"DEBUG: Removing all anchors ({anchorDic.Count})");
        foreach (var anchor in anchorDic)
        {
            Destroy(anchor.Key.gameObject);
        }
        s_Hits.Clear();
        anchorDic.Clear();
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        GameObject cameraImage = GameObject.Find("Camera");
        aRCamera = cameraImage.GetComponent<ARCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update in action");
        if (!aRCamera.localization)
        {
            return;
        }

        boxSavedOutlines = aRCamera.boxSavedOutlines;
        shiftX = aRCamera.shiftX;
        shiftY = aRCamera.shiftY;
        scaleFactor = aRCamera.scaleFactor;

        if (anchorDic.Count != 0)
        {
            foreach (KeyValuePair<ARAnchor, Detector.BoundingBox> pair in anchorDic)
            {
                if (!boxSavedOutlines.Contains(pair.Value))
                {
                    anchorDic.Remove(pair.Key);
                    m_AnchorManager.RemoveAnchor(pair.Key);
                    s_Hits.Clear();
                }
            }
        }

        if (boxSavedOutlines.Count == 0)
        {
            return;
        }

        foreach (var outline in boxSavedOutlines)
        {
            if (outline.Used)
            {
                continue;
            }

            var xMin = outline.Dimensions.X * this.scaleFactor + this.shiftX;
            var width = outline.Dimensions.Width * this.scaleFactor;
            var yMin = outline.Dimensions.Y * this.scaleFactor + this.shiftY;
            yMin = Screen.height - yMin;
            var height = outline.Dimensions.Height * this.scaleFactor;

            float center_x = xMin + width / 2f;
            float center_y = yMin - height / 2f;

            if (Pos2Anchor(center_x, center_y, outline))
            {
                outline.Used = true;


            }
        }
    }

    private bool Pos2Anchor(float x, float y, Detector.BoundingBox outline)
    {
        const TrackableType trackableTypes =
            TrackableType.FeaturePoint |
            TrackableType.PlaneWithinPolygon;

        if (m_RaycastManager.Raycast(new Vector2(x, y), s_Hits, trackableTypes))
        {
            var hit = s_Hits[0];
            var anchor = CreateAnchor(hit);
            if (anchor)
            {
                // Remember the anchor so we can remove it later.
                anchorDic.Add(anchor, outline);
                return true;
            }

            return false;
        }
        return false;
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor = null;

        if (hit.trackable is ARPlane plane)
        {
            var planeManager = GetComponent<ARPlaneManager>();
            if (planeManager)
            {
                var oldPrefab = m_AnchorManager.anchorPrefab;
                m_AnchorManager.anchorPrefab = prefab;
                anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
                m_AnchorManager.anchorPrefab = oldPrefab;
                return anchor;
            }
        }
        var gameObject = Instantiate(prefab, hit.pose.position, hit.pose.rotation);

        anchor = gameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = gameObject.AddComponent<ARAnchor>();
        }

        return anchor;
    }
}