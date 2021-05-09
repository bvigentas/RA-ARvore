using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Barracuda;
using System.Text.RegularExpressions;



public class Detector : MonoBehaviour
{
    public NNModel modelFile;
    public TextAsset labelsFile;
    public string INPUT_NAME;
    public string OUTPUT_NAME_L;
    public string OUTPUT_NAME_M;
    public int CLASS_COUNT;
    public float MINIMUM_CONFIDENCE = 0.25f;
    public int IMAGE_SIZE { get => _image_size; }

    private string[] labels;
    private const int _image_size = 416;
    public const int BOXES_PER_CELL = 3;
    public const int BOX_INFO_FEATURE_COUNT = 5;
    private IWorker worker;

    public Dictionary<string, int> params_l = new Dictionary<string, int>() { { "ROW_COUNT", 13 }, { "COL_COUNT", 13 }, { "CELL_WIDTH", 32 }, { "CELL_HEIGHT", 32 } };
    public Dictionary<string, int> params_m = new Dictionary<string, int>() { { "ROW_COUNT", 26 }, { "COL_COUNT", 26 }, { "CELL_WIDTH", 16 }, { "CELL_HEIGHT", 16 } };

    private float[] anchors = new float[]
    {
        1.46F,1.63F, 3.43F,4.17F, 5.43F,6.02F, 6.65F,1.93F, 8.04F,7.69F, 9.97F,10.43F
    };


    public void Start()
    {
        this.labels = Regex.Split(this.labelsFile.text, "\n|\r|\r\n")
            .Where(s => !String.IsNullOrEmpty(s)).ToArray();
        var model = ModelLoader.Load(this.modelFile);
        this.worker = GraphicsWorker.GetWorker(model);
    }

    public IEnumerator Detect(Color32[] picture, System.Action<IList<BoundingBox>> callback)
    {
        using (var tensor = BarracudaHelper.CreateTensorFromPicuture(picture, IMAGE_SIZE, IMAGE_SIZE))
        {
            var inputs = new Dictionary<string, Tensor>();
            inputs.Add(INPUT_NAME, tensor);
            yield return StartCoroutine(worker.StartManualSchedule(inputs));
            var output_l = worker.PeekOutput(OUTPUT_NAME_L);
            var output_m = worker.PeekOutput(OUTPUT_NAME_M);

            var results_l = ParseOutputs(output_l, MINIMUM_CONFIDENCE, params_l);
            var results_m = ParseOutputs(output_m, MINIMUM_CONFIDENCE, params_m);
            var results = results_l.Concat(results_m).ToList();

            var boxes = FilterBoundingBoxes(results, 1, MINIMUM_CONFIDENCE);
            callback(boxes);
        }
    }


    private IList<BoundingBox> ParseOutputs(Tensor yoloModelOutput, float threshold, Dictionary<string, int> parameters)
    {
        var boxes = new List<BoundingBox>();

        for (int cy = 0; cy < parameters["COL_COUNT"]; cy++)
        {
            for (int cx = 0; cx < parameters["ROW_COUNT"]; cx++)
            {
                for (int box = 0; box < BOXES_PER_CELL; box++)
                {
                    var channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT));
                    var bbd = ExtractBoundingBoxDimensions(yoloModelOutput, cx, cy, channel);
                    var confidence = GetConfidence(yoloModelOutput, cx, cy, channel);

                    if (confidence < threshold)
                    {
                        continue;
                    }

                    var predictedClasses = ExtractClasses(yoloModelOutput, cx, cy, channel);
                    var (topResultIndex, topResultScore) = GetTopResult(predictedClasses);
                    var topScore = topResultScore * confidence;

                    if (topScore < threshold)
                    {
                        continue;
                    }

                    var mappedBoundingBox = MapBoundingBoxToCell(cx, cy, box, bbd, parameters);
                    boxes.Add(BuildBoudingBox(topResultIndex, topScore, mappedBoundingBox));
                }
            }
        }

        return boxes;
    }

    private BoundingBoxDimensions ExtractBoundingBoxDimensions(Tensor modelOutput, int x, int y, int channel)
    {
        return new BoundingBoxDimensions
        {
            X = modelOutput[0, x, y, channel],
            Y = modelOutput[0, x, y, channel + 1],
            Width = modelOutput[0, x, y, channel + 2],
            Height = modelOutput[0, x, y, channel + 3]
        };
    }


    private float GetConfidence(Tensor modelOutput, int x, int y, int channel)
    {
        return BarracudaHelper.Sigmoid(modelOutput[0, x, y, channel + 4]);
    }


    private CellDimensions MapBoundingBoxToCell(int x, int y, int box, BoundingBoxDimensions boxDimensions, Dictionary<string, int> parameters)
    {
        return new CellDimensions
        {
            X = ((float)y + BarracudaHelper.Sigmoid(boxDimensions.X)) * parameters["CELL_WIDTH"],
            Y = ((float)x + BarracudaHelper.Sigmoid(boxDimensions.Y)) * parameters["CELL_HEIGHT"],
            Width = (float)Math.Exp(boxDimensions.Width) * anchors[6 + box * 2],
            Height = (float)Math.Exp(boxDimensions.Height) * anchors[6 + box * 2 + 1],
        };
    }


    public float[] ExtractClasses(Tensor modelOutput, int x, int y, int channel)
    {
        var predictedClasses = new float[CLASS_COUNT];
        var predictedClassOffset = channel + BOX_INFO_FEATURE_COUNT;

        for (int predictedClass = 0; predictedClass < CLASS_COUNT; predictedClass++)
        {
            predictedClasses[predictedClass] = modelOutput[0, x, y, predictedClass + predictedClassOffset];
        }

        return BarracudaHelper.Softmax(predictedClasses);
    }


    private ValueTuple<int, float> GetTopResult(float[] predictedClasses)
    {
        return predictedClasses
            .Select((predictedClass, index) => (Index: index, Value: predictedClass))
            .OrderByDescending(result => result.Value)
            .First();
    }


    private float IntersectionOverUnion(Rect boundingBoxA, Rect boundingBoxB)
    {
        float areaA = CalculateArea(boundingBoxA);

        if (areaA <= 0)
            return 0;

        var areaB = CalculateArea(boundingBoxB);

        if (areaB <= 0)
            return 0;

        float intersectionArea = CalculateIntersectionArea(boundingBoxA, boundingBoxB);
        float unionArea = CalculateUnionArea(areaA + areaB, intersectionArea);
        return intersectionArea / unionArea;
    }

    private static float CalculateArea(Rect boundingBox)
    {
        return boundingBox.width * boundingBox.height;
    }

    private static float CalculateUnionArea(float boudingBoxesArea, float intersectionArea)
    {
        return boudingBoxesArea - intersectionArea;
    }

    private static float CalculateIntersectionArea(Rect boundingBoxA, Rect boundingBoxB)
    {
        var minX = Math.Max(boundingBoxA.xMin, boundingBoxB.xMin);
        var minY = Math.Max(boundingBoxA.yMin, boundingBoxB.yMin);
        var maxX = Math.Min(boundingBoxA.xMax, boundingBoxB.xMax);
        var maxY = Math.Min(boundingBoxA.yMax, boundingBoxB.yMax);

        var intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0);
        return intersectionArea;
    }

    private IList<BoundingBox> FilterBoundingBoxes(IList<BoundingBox> boxes, int limit, float threshold)
    {
        var activeCount = boxes.Count;
        var isActiveBoxes = new bool[boxes.Count];

        for (int i = 0; i < isActiveBoxes.Length; i++)
        {
            isActiveBoxes[i] = true;
        }

        var sortedBoxes = boxes.Select((b, i) => new { Box = b, Index = i })
                .OrderByDescending(b => b.Box.Confidence)
                .ToList();

        var results = new List<BoundingBox>();

        for (int i = 0; i < boxes.Count; i++)
        {
            if (isActiveBoxes[i])
            {
                var boxA = sortedBoxes[i].Box;
                results.Add(boxA);

                if (results.Count >= limit)
                    break;

                for (var j = i + 1; j < boxes.Count; j++)
                {
                    if (isActiveBoxes[j])
                    {
                        var boxB = sortedBoxes[j].Box;

                        if (IntersectionOverUnion(boxA.Rect, boxB.Rect) > threshold)
                        {
                            isActiveBoxes[j] = false;
                            activeCount--;

                            if (activeCount <= 0)
                                break;
                        }
                    }
                }

                if (activeCount <= 0)
                    break;
            }
        }
        return results;
    }

    private BoundingBox BuildBoudingBox(int topResultIndex, float topScore, CellDimensions mappedBoundingBox)
    {
        return new BoundingBox
        {
            Dimensions = BuildBoudingBoxDimensions(mappedBoundingBox),
            Confidence = topScore,
            Label = labels[topResultIndex],
            Used = false
        };
    }

    private static BoundingBoxDimensions BuildBoudingBoxDimensions(CellDimensions mappedBoundingBox)
    {
        return new BoundingBoxDimensions
        {
            X = (mappedBoundingBox.X - mappedBoundingBox.Width / 2),
            Y = (mappedBoundingBox.Y - mappedBoundingBox.Height / 2),
            Width = mappedBoundingBox.Width,
            Height = mappedBoundingBox.Height,
        };
    }
}
