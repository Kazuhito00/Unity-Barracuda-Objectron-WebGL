using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Unity.Barracuda;

public class WebCamController : MonoBehaviour
{
    int width = 640;
    int height = 480;
    int fps = 30;
    WebCamTexture webcamTexture;

    // Objectronモデル
    public NNModel modelAsset;    
    private Objectron objectron;
    
    Texture2D texture;
    Color32[] cameraBuffer = null;

    // 推論結果描画用テキスト
    public Text text;
    private readonly FPSCounter fpsCounter = new FPSCounter();

    void Start() 
    {
        // Webカメラ準備
        WebCamDevice[] devices = WebCamTexture.devices;
        webcamTexture = new WebCamTexture(devices[0].name, this.width, this.height, this.fps);
        webcamTexture.Play();
        
        // Objectron推論用クラス
        objectron = new Objectron(modelAsset);
        StartCoroutine(WebCamTextureInitialize());
    }

    IEnumerator WebCamTextureInitialize()
    {
        while (true) {
            if (webcamTexture.width > 16 && webcamTexture.height > 16) {
                GetComponent<Renderer>().material.mainTexture = webcamTexture;
                cameraBuffer = new Color32[webcamTexture.width * webcamTexture.height];
                texture = new Texture2D(webcamTexture.width, webcamTexture.height);
                break;
            }
            yield return null;
        }
    }
    
    void Update()
    {
        fpsCounter.Update();

        // 入力用テクスチャ準備
        webcamTexture.GetPixels32(cameraBuffer);
        texture.SetPixels32(cameraBuffer);
        texture.Apply();
        
        // 推論
        var results = objectron.Inference(texture);

        var score = results[0];

        List<Vector2> pointList = new List<Vector2>();
        for (int i = 1; i < results.Length; i = i + 2)
        {
            var pointX = (int)((results[i] / 224) * webcamTexture.width);
            var pointY = (int)((results[i+1] / 224) * webcamTexture.height);
            pointX = (int)(Mathf.Clamp(pointX, 0, webcamTexture.width));
            pointY = (int)(Mathf.Clamp(pointY, 0, webcamTexture.height));

            Vector2 point = new Vector2(pointX, pointY);
            pointList.Add(point);
        }

        DrawPoint(pointList[0], new Color32(255, 255, 255, 0), 10.0f);  // 重心(Center of gravity)
        DrawPoint(pointList[1], new Color32(0, 0, 255, 0), 10.0f);  // 後01(Back01)
        DrawPoint(pointList[2], new Color32(0, 255, 0, 0), 10.0f);  // 前01(Front01)
        DrawPoint(pointList[3], new Color32(0, 0, 255, 0), 10.0f);  // 後02(Back02)
        DrawPoint(pointList[4], new Color32(0, 255, 0, 0), 10.0f);  // 前02(Front02)
        DrawPoint(pointList[5], new Color32(0, 0, 255, 0), 10.0f);  // 後03(Back03)
        DrawPoint(pointList[6], new Color32(0, 255, 0, 0), 10.0f);  // 前03(Front03)
        DrawPoint(pointList[7], new Color32(0, 0, 255, 0), 10.0f);  // 後04(Back04)
        DrawPoint(pointList[8], new Color32(0, 255, 0, 0), 10.0f);  // 前04(Front04)

        DrawLine(pointList[1], pointList[2], new Color32(255, 255, 255, 0));
        DrawLine(pointList[3], pointList[4], new Color32(255, 255, 255, 0));
        DrawLine(pointList[5], pointList[6], new Color32(255, 255, 255, 0));
        DrawLine(pointList[7], pointList[8], new Color32(255, 255, 255, 0));
        
        DrawLine(pointList[1], pointList[5], new Color32(0, 0, 255, 0));
        DrawLine(pointList[3], pointList[7], new Color32(0, 0, 255, 0));
        DrawLine(pointList[1], pointList[3], new Color32(0, 0, 255, 0));
        DrawLine(pointList[5], pointList[7], new Color32(0, 0, 255, 0));
        
        DrawLine(pointList[2], pointList[6], new Color32(0, 255, 0, 0));
        DrawLine(pointList[4], pointList[8], new Color32(0, 255, 0, 0));
        DrawLine(pointList[2], pointList[4], new Color32(0, 255, 0, 0));
        DrawLine(pointList[6], pointList[8], new Color32(0, 255, 0, 0));

        texture.SetPixels32(cameraBuffer);
        texture.Apply();
        GetComponent<Renderer>().material.mainTexture = texture;
    
        // 描画用テキスト構築
        string resultText = "";
        resultText = "FPS:" + fpsCounter.FPS.ToString("F2") + "\n";
        resultText = resultText + "Score:" + score.ToString("F2");
#if UNITY_IOS || UNITY_ANDROID
        resultText = resultText + SystemInfo.graphicsDeviceType;
#endif
        // テキスト画面反映
        text.text = resultText;
    }

    private void DrawPoint(Vector2 point, Color32 color, double brushSize = 1.5f)
    {
        point.x = (int)point.x;
        point.y = (int)point.y;

        int start_x = Mathf.Max(0, (int)(point.x - (brushSize - 1)));
        int end_x = Mathf.Min(webcamTexture.width, (int)(point.x + (brushSize + 1)));
        int start_y =  Mathf.Max(0, (int)(point.y - (brushSize - 1)));
        int end_y = Mathf.Min(webcamTexture.height, (int)(point.y + (brushSize + 1)));

        for (int x = start_x; x < end_x; x++) {
            for (int y = start_y; y < end_y; y++) {
                double length = Mathf.Sqrt(Mathf.Pow(point.x - x, 2) + Mathf.Pow(point.y - y, 2));
                if (length < brushSize) {
                    cameraBuffer.SetValue(color, (webcamTexture.width - x) + (webcamTexture.width * (webcamTexture.height - y)));
                }
            }
        }
    }

    public void DrawLine(Vector2 point1, Vector2 point2, Color color, int lerpNum = 100)
    {
        for(int i=0; i < lerpNum + 1; i++) {
            var point = Vector2.Lerp(point1, point2, i * (1.0f / lerpNum));
            DrawPoint(point, color, 1.5f);
        }
    }
}