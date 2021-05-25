using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARCore;

public class ARCamera : MonoBehaviour
{
    public string foundedLeafString;
    public float shiftY = 0f;
    public float shiftX = 0f;
    public float scaleFactor = 1;
    public bool localization = false;
    public bool searched = false;
    private bool isDetecting = false;
    private int staticNum = 0;

    public Detector detector;
    public List<BoundingBox> boxOutlinesFromAllFrames = new List<BoundingBox>();
    public Color colorTag = new Color(0.3843137f, 0, 0.9333333f);
    private static Texture2D boxOutlineTexture;
    private static GUIStyle labelStyle;
    private IList<BoundingBox> boxOutlinesFromThisFrame;

    public BoundingBox actualBoudingBox;
    
    private GameObject buttonInformation;
    private GameObject buttonScreenshot;
    private GameObject buttonValidate;

    Texture2D m_CameraTexture;
    [SerializeField] ARCameraManager m_CameraManager;
    public ARCameraManager cameraManager
    {
        get => m_CameraManager;
        set => m_CameraManager = value;
    }

    [SerializeField] RawImage m_RawImage;
    public RawImage rawImage
    {
        get { return m_RawImage; }
        set { m_RawImage = value; }
    }

    public string GetMostConfidentBoudingBoxName()
    {
        var box = this.boxOutlinesFromThisFrame.OrderByDescending(box => box.Confidence).First();
        return box.Label;
    }

    void OnDisable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }


    //Método responsável por resetar componentes ao restartar a aplicação.
    public void OnRefresh()
    {
        localization = false;
        searched = false;
        staticNum = 0;
        //boxOutlinesFromAllFrames.Clear();
        actualBoudingBox = null;
        boxOutlinesFromThisFrame.Clear();
        AnchorCreator anchorCreator = FindObjectOfType<AnchorCreator>();
        anchorCreator.RemoveAllAnchors();
        buttonInformation.SetActive(false);
        buttonScreenshot.SetActive(false);
        buttonValidate.SetActive(false);

        ARSession arSession = GetComponent<ARSession>();
        arSession.Reset();
    }

    //Realiza configurações iniciais ao começar a aplicação.
    void OnEnable()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.frameReceived += OnCameraFrameReceived;
        }

        boxOutlineTexture = new Texture2D(1, 1);
        boxOutlineTexture.SetPixel(0, 0, this.colorTag);
        boxOutlineTexture.Apply();
        buttonInformation = GameObject.Find("ButtonLeafInformation");
        buttonScreenshot = GameObject.Find("ButtonScreenshot");
        buttonValidate = GameObject.Find("ButtonValidar");

        labelStyle = new GUIStyle();
        labelStyle.fontSize = 50;
        labelStyle.normal.textColor = this.colorTag;

        detector = GameObject.Find("Detector").GetComponent<Detector>();

        detector.Start();

        CalculateShift(this.detector.IMAGE_SIZE);
    }

    private void CalculateShift(int inputSize)
    {
        int smallest;

        if (Screen.width < Screen.height)
        {
            smallest = Screen.width;
            this.shiftY = (Screen.height - smallest) / 2f;
        }
        else
        {
            smallest = Screen.height;
            this.shiftX = (Screen.width - smallest) / 2f;
        }

        this.scaleFactor = smallest / (float)inputSize;
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        XRCpuImage image;
        if (!cameraManager.TryAcquireLatestCpuImage(out image))
        {
            return;
        }

        PreProcessCurrentImageFrame(image);

        if (DetectionIsStable())
        {
            localization = true;
            EnableButtons();

            if (!searched && this.boxOutlinesFromThisFrame != null && this.boxOutlinesFromThisFrame.Count >= 0)
            {
                foundedLeafString = GetMostConfidentBoudingBoxName();
            }
        }
        else
        {
            DisableButtons();

            if (this.isDetecting)
            {
                return;
            }

            StartToDetect();

            GroupBoxOutlines();
        }

        m_RawImage.texture = m_CameraTexture;
    }


    //Faz alguns processamentos iniciais na imagem, convertento a imagem em uma textura RGB para  ser processada mais facilmente para a rotina de detecção.
    private unsafe void PreProcessCurrentImageFrame(XRCpuImage image)
    {
        var format = TextureFormat.RGBA32;

        if (m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height)
        {
            m_CameraTexture = new Texture2D(image.width, image.height, format, false);
        }

        var conversionParams = new XRCpuImage.ConversionParams(image, format, XRCpuImage.Transformation.None);
        var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
        try
        {
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        }
        finally
        {
            image.Dispose();
        }

        m_CameraTexture.Apply();
    }

    private bool DetectionIsStable()
    {
        return staticNum > 5;
    }

    private unsafe void EnableButtons()
    {
        buttonInformation.SetActive(true);
        buttonScreenshot.SetActive(true);
        if (Configurations.quizMode)
        {
            buttonValidate.SetActive(true);
        }
    }

    private unsafe void DisableButtons()
    {
        buttonInformation.SetActive(false);
        buttonScreenshot.SetActive(false);
        buttonValidate.SetActive(false);
    }

    private unsafe void StartToDetect()
    {
        this.isDetecting = true;
        StartCoroutine(ProcessImage(this.detector.IMAGE_SIZE, result =>
        {
            StartCoroutine(this.detector.Detect(result, boxes =>
            {
                this.boxOutlinesFromThisFrame = boxes;
                Resources.UnloadUnusedAssets();
                this.isDetecting = false;
            }));
        }));
    }

    private IEnumerator ProcessImage(int inputSize, System.Action<Color32[]> callback)
    {
        Coroutine croped = StartCoroutine(TextureTools.CropSquare(m_CameraTexture,
           TextureTools.RectOptions.Center, snap =>
           {
               var scaled = Scale(snap, inputSize);
               var rotated = Rotate(scaled.GetPixels32(), scaled.width, scaled.height);
               callback(rotated);
           }));
        yield return croped;
    }

    //Método que filtra bouding boxes adicionando e filtrando as bouding boxes de vários frames. 
    private void GroupBoxOutlines()
    {
        if (this.boxOutlinesFromThisFrame != null && this.boxOutlinesFromThisFrame.Count > 0)
        {
            actualBoudingBox = this.boxOutlinesFromThisFrame.OrderByDescending(box => box.Confidence).First();

            bool addOutline = false;

            bool unique = true;
            var boxCopy = this.boxOutlinesFromAllFrames;
            foreach (var boxSaved in boxCopy)
            {

                if (IsSameObject(actualBoudingBox, boxSaved))
                {
                    unique = false;
                    if (actualBoudingBox.Confidence > boxSaved.Confidence + 0.05F)
                    {
                        this.boxOutlinesFromAllFrames.Remove(boxSaved);
                        this.boxOutlinesFromAllFrames.Add(actualBoudingBox);
                        addOutline = true;
                        staticNum = 0;
                        break;
                    }
                }
            }

            if (unique)
            {
                addOutline = true;
                staticNum = 0;
                this.boxOutlinesFromAllFrames.Add(actualBoudingBox);
            }
            if (!addOutline)
            {
                staticNum += 1;
            }
        }
    }

    //Compara dois objetos para ver se são iguais, caso for o que tem o maior score de confidence é utilizado.
    private bool IsSameObject(BoundingBox outline1, BoundingBox outline2)
    {
        var xMin1 = outline1.Dimensions.X * this.scaleFactor + this.shiftX;
        var width1 = outline1.Dimensions.Width * this.scaleFactor;
        var yMin1 = outline1.Dimensions.Y * this.scaleFactor + this.shiftY;
        var height1 = outline1.Dimensions.Height * this.scaleFactor;
        float center_x1 = xMin1 + width1 / 2f;
        float center_y1 = yMin1 + height1 / 2f;

        var xMin2 = outline2.Dimensions.X * this.scaleFactor + this.shiftX;
        var width2 = outline2.Dimensions.Width * this.scaleFactor;
        var yMin2 = outline2.Dimensions.Y * this.scaleFactor + this.shiftY;
        var height2 = outline2.Dimensions.Height * this.scaleFactor;
        float center_x2 = xMin2 + width2 / 2f;
        float center_y2 = yMin2 + height2 / 2f;

        bool cover_x = (xMin2 < center_x1) && (center_x1 < (xMin2 + width2));
        bool cover_y = (yMin2 < center_y1) && (center_y1 < (yMin2 + height2));
        bool contain_x = (xMin1 < center_x2) && (center_x2 < (xMin1 + width1));
        bool contain_y = (yMin1 < center_y2) && (center_y2 < (yMin1 + height1));

        return (cover_x && cover_y) || (contain_x && contain_y);
    }

    public void OnGUI()
    {
        if (localization)
        {
            return;
        }

        if (this.boxOutlinesFromAllFrames != null && this.boxOutlinesFromAllFrames.Any())
        {
            var box = this.boxOutlinesFromAllFrames.OrderByDescending(box => box.Confidence).First();
            DrawLocalizingText($"Mantenha-se estável.\n Localizando {box.Label}: {(int)(box.Confidence * 100)}%");
        }
    }

    private static void DrawLocalizingText(string text)
    {
        GUI.color = Color.blue;
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
        GUI.color = Color.white;
        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.MiddleCenter;
        centeredStyle.fontSize = 50;
        GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 100, 500, 200), text, centeredStyle);
    }

    private Texture2D Scale(Texture2D texture, int imageSize)
    {
        var scaled = TextureTools.scaled(texture, imageSize, imageSize, FilterMode.Bilinear);
        return scaled;
    }

    private Color32[] Rotate(Color32[] pixels, int width, int height)
    {
        var rotate = TextureTools.RotateImageMatrix(
                pixels, width, height, 90);
        return rotate;
    }


}
