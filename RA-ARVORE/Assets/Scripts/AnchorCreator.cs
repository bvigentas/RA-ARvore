using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator : MonoBehaviour
{
    [SerializeField]
    List<GameObject> m_Prefab;

    List<string> dicPreFab = new List<string>();

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
    public ARPlaneManager m_planeManager;

    public List<GameObject> prefab
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private UIManager m_uiManager;

    void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {
        if (!aRCamera.localization)
        {
            return;
        }
        if (m_planeManager)
        {
            if (m_uiManager.PlanesFound())
            {
                foreach (var planeFound in m_planeManager.trackables)
                    planeFound.gameObject.SetActive(false);

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
                UpdateInfoPanel(aRCamera.foundedLeafString);
                return true;
            }

            return false;
        }
        return false;
    }

    private void UpdateInfoPanel(string formatoFolha)
    {
        TextMeshPro tipoFolha = GameObject.Find("Texto_Forma").GetComponent<TextMeshPro>();
        TextMeshPro informacoesFolha = GameObject.Find("Texto_Descricao").GetComponent<TextMeshPro>();
        TextMeshPro arvoresFolha = GameObject.Find("Texto_Arvore").GetComponent<TextMeshPro>();

        FolhaService service = GameObject.Find("WebService").GetComponent<FolhaService>();
        Folha arvore = service.getArvoreInfo(formatoFolha);

        tipoFolha.SetText(arvore.tipo_folha);
        informacoesFolha.SetText(arvore.informacoes_folha);
        arvoresFolha.SetText(arvore.arvores_folha);
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor = null;

        if (hit.trackable is ARPlane plane)
        {
            if (m_planeManager)
            {
                var oldPrefab = m_AnchorManager.anchorPrefab;
                m_AnchorManager.anchorPrefab = prefab[dicPreFab.IndexOf(aRCamera.foundedLeafString)];
                anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
                m_AnchorManager.anchorPrefab = oldPrefab;
                //UpdateInfoPanel(aRCamera.foundedLeafString);
                return anchor;
            }
        }
        var gameObject = Instantiate(prefab[dicPreFab.IndexOf(aRCamera.foundedLeafString)], hit.pose.position, hit.pose.rotation);

        anchor = gameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = gameObject.AddComponent<ARAnchor>();
        }

        return anchor;
    }
}