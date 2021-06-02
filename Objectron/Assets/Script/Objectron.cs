using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Barracuda;

public class Objectron
{   
    readonly IWorker worker;

    private int inputShapeX = 224;
    private int inputShapeY = 224;

    public Objectron(NNModel modelAsset)
    {
        var model = ModelLoader.Load(modelAsset);

#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("Worker:CPU");
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model); // CPU
#else
        Debug.Log("Worker:GPU");
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model); // GPU
#endif
    }

    public float[] Inference(Texture2D texture)
    {
        // テクスチャコピー
        Texture2D inputTexture = new Texture2D(texture.width, texture.height);
        var tempColor32 = texture.GetPixels32();
        inputTexture.SetPixels32(tempColor32);
        inputTexture.Apply();
        Graphics.CopyTexture(texture, inputTexture);

        // テクスチャリサイズ、およびColor32データ取得
        TextureScale.Bilinear(inputTexture, inputShapeX, inputShapeY);
        var color32 = inputTexture.GetPixels32();
        MonoBehaviour.Destroy(inputTexture);
        
        int pixelCount = 0;
        float[] floatValues = new float[inputShapeX * inputShapeY * 3];
        for (int i = (color32.Length - 1); i >= 0 ; i--) {
            var color = color32[i];
            floatValues[pixelCount * 3 + 0] = (color.r - 0) / 255.0f;
            floatValues[pixelCount * 3 + 1] = (color.g - 0) / 255.0f;
            floatValues[pixelCount * 3 + 2] = (color.b - 0) / 255.0f;
            pixelCount += 1;
        }
        var inputTensor = new Tensor(1, inputShapeY, inputShapeX, 3, floatValues);

        // 推論実行
        worker.Execute(inputTensor);

        // 出力：スコア
        var outputTensor01 = worker.PeekOutput("Identity");
        var scoreArray = outputTensor01.ToReadOnlyArray();

        // 出力：3Dバウンディングボックス
        var outputTensor02 = worker.PeekOutput("Identity_1");
        var pointArray = outputTensor02.ToReadOnlyArray();
                
        // 結果格納
        float[] results = {
            (float)(scoreArray[0]), 
            (int)(pointArray[0]),
            (int)(pointArray[1]),
            (int)(pointArray[2]),
            (int)(pointArray[3]),
            (int)(pointArray[4]),
            (int)(pointArray[5]),
            (int)(pointArray[6]),
            (int)(pointArray[7]),
            (int)(pointArray[8]),
            (int)(pointArray[9]),
            (int)(pointArray[10]),
            (int)(pointArray[11]),
            (int)(pointArray[12]),
            (int)(pointArray[13]),
            (int)(pointArray[14]),
            (int)(pointArray[15]),
            (int)(pointArray[16]),
            (int)(pointArray[17]),
        };
        
        // 解放
        inputTensor.Dispose();
        outputTensor01.Dispose();
        outputTensor02.Dispose();

        return results;
    }

    ~Objectron()
    {
        worker?.Dispose();
    }
}