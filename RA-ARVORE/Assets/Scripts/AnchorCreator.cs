using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Barracuda;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator : MonoBehaviour
{
    public ARCamera aRCamera;
    public ARRaycastManager m_RaycastManager;
    public TextMesh anchorObj_mesh;
    public ARAnchorManager m_AnchorManager;
    public ARPlaneManager m_planeManager;

    private List<BoundingBox> boxSavedOutlines;
    private float shiftX;
    private float shiftY;
    private float scaleFactor;
    private bool modelAlreadyRendered;
    private UIManager m_uiManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<string> dicPreFab = new List<string>();
    IDictionary<ARAnchor, BoundingBox> anchorDic = new Dictionary<ARAnchor, BoundingBox>();

    [SerializeField] List<GameObject> m_Prefab;
    public List<GameObject> prefab
    {
        get => m_Prefab;
        set => m_Prefab = value;
    }

    public void RemoveAllAnchors()
    {
        modelAlreadyRendered = false;
        foreach (var anchor in anchorDic)
        {
            Destroy(anchor.Key.gameObject);
        }
        s_Hits.Clear();
        anchorDic.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    void Awake()
    {
        modelAlreadyRendered = false;
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        GameObject cameraImage = GameObject.Find("Camera");
        aRCamera = cameraImage.GetComponent<ARCamera>();
        m_planeManager = GetComponent<ARPlaneManager>();

        GameObject canvas = GameObject.Find("Canvas");
        m_uiManager = canvas.GetComponent<UIManager>();

        dicPreFab.Add("trofolio");
        dicPreFab.Add("tripinulada");
        dicPreFab.Add("bipinulada");
        dicPreFab.Add("codiforme");
        dicPreFab.Add("flabelada");
        dicPreFab.Add("orbicular");
        dicPreFab.Add("romboide");
        dicPreFab.Add("multilobada");
        dicPreFab.Add("deltoide");
        dicPreFab.Add("lobada");
        dicPreFab.Add("cuneiforme");
        dicPreFab.Add("linear");
        dicPreFab.Add("ovada");
        dicPreFab.Add("pinulada");
        dicPreFab.Add("espalmada");
        dicPreFab.Add("acicular");

    }

    void Update()
    {
        if (NotDetectedYet())
        {
            return;
        }

        if (PlanesFoundAndModelNotRenderedYet())
        {
            //foreach (var planeFound in m_planeManager.trackables)
            //    planeFound.gameObject.SetActive(false);

            boxSavedOutlines = aRCamera.boxSavedOutlines;
            shiftX = aRCamera.shiftX;
            shiftY = aRCamera.shiftY;
            scaleFactor = aRCamera.scaleFactor;

            RemoveOldAnchorsNotBeingUsed();

            if (NoBoudingBoxesFound())
            {
                return;
            }

            foreach (var outline in boxSavedOutlines)
            {
                if (outline.Used)
                {
                    continue;
                }

                var tuple = FindCenterXandY(outline);

                if (CreateAnchorInPosition(tuple.Item1, tuple.Item2, outline))
                {
                    outline.Used = true;
                }

                break;
            }
        }
    }


    private Tuple<float, float> FindCenterXandY(BoundingBox outline)
    {
        var xMin = outline.Dimensions.X * this.scaleFactor + this.shiftX;
        var width = outline.Dimensions.Width * this.scaleFactor;
        var yMin = outline.Dimensions.Y * this.scaleFactor + this.shiftY;
        yMin = Screen.height - yMin;
        var height = outline.Dimensions.Height * this.scaleFactor;

        var center_x = xMin + width / 2f;
        var center_y = yMin - height / 2f;

        return Tuple.Create(center_x, center_y);
    }

    private void RemoveOldAnchorsNotBeingUsed()
    {
        if (anchorDic.Count != 0)
        {
            foreach (KeyValuePair<ARAnchor, BoundingBox> pair in anchorDic)
            {
                if (!boxSavedOutlines.Contains(pair.Value))
                {
                    anchorDic.Remove(pair.Key);
                    m_AnchorManager.RemoveAnchor(pair.Key);
                    s_Hits.Clear();
                }
            }
        }
    }

    private bool NoBoudingBoxesFound()
    {
        return boxSavedOutlines.Count == 0;
    }

    private bool NotDetectedYet()
    {
        return !aRCamera.localization;
    }

    private bool PlanesFoundAndModelNotRenderedYet()
    {
        return m_planeManager && m_uiManager.PlanesFound() && !modelAlreadyRendered;
    }

    private bool CreateAnchorInPosition(float x, float y, BoundingBox outline)
    {
        const TrackableType trackableTypes = TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon;

        if (VerifyIfPointIntersectsWithPlanes(x, y, trackableTypes))
        {
            var hit = s_Hits[0];
            var anchor = CreateAnchor(hit);
            if (anchor)
            {
                SaveAnchor(outline, anchor);
                StartCoroutine(UpdateInfoPanel(aRCamera.foundedLeafString));
                return true;
            }

            return false;
        }
        return false;
    }

    private bool VerifyIfPointIntersectsWithPlanes(float x, float y, TrackableType trackableTypes)
    {
        return m_RaycastManager.Raycast(new Vector2(x, y), s_Hits, trackableTypes);
    }

    private void SaveAnchor(BoundingBox outline, ARAnchor anchor)
    {
        anchorDic.Add(anchor, outline);
    }

    private IEnumerator UpdateInfoPanel(string leafFormat)
    {
        var leafType = GameObject.Find("Texto_Forma").GetComponent<TextMeshPro>();
        var leafInformation = GameObject.Find("Texto_Descricao").GetComponent<TextMeshPro>();
        var treesWithThisLeaf = GameObject.Find("Texto_Arvore").GetComponent<TextMeshPro>();

        var service = GameObject.Find("WebService").GetComponent<FolhaService>();
        var tree = service.getArvoreInfo(leafFormat);
        yield return null;

        leafType.SetText(tree.tipo_folha);
        leafInformation.SetText(tree.informacoes_folha);
        treesWithThisLeaf.SetText(tree.arvores_folha);

        
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor = null;

        if (hit.trackable is ARPlane plane)
        {
            if (m_planeManager)
            {
                return UppdatePrefabAndAttachAnchorToPlane(hit, out anchor, plane);
            }
        }
        return CreateAnchorWithoutBeingAttachToPlanes(hit, out anchor);
    }

    private ARAnchor UppdatePrefabAndAttachAnchorToPlane(ARRaycastHit hit, out ARAnchor anchor, ARPlane plane)
    {
        var oldPrefab = m_AnchorManager.anchorPrefab;
        m_AnchorManager.anchorPrefab = prefab[dicPreFab.IndexOf(aRCamera.foundedLeafString)];
        anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
        m_AnchorManager.anchorPrefab = oldPrefab;
        modelAlreadyRendered = true;
        return anchor;
    }

    private ARAnchor CreateAnchorWithoutBeingAttachToPlanes(ARRaycastHit hit, out ARAnchor anchor)
    {
        var gameObject = Instantiate(prefab[dicPreFab.IndexOf(aRCamera.foundedLeafString)], hit.pose.position, hit.pose.rotation);

        anchor = gameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            modelAlreadyRendered = true;
            anchor = gameObject.AddComponent<ARAnchor>();
        }

        return anchor;
    }
}