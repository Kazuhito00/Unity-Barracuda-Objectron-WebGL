# Unity-Barracuda-Objectron-WebGL
Unity Barracudaで[MediaPipe Objectron(Shoe)](https://google.github.io/mediapipe/solutions/objectron)を動作させるサンプルです。<br>
また、現時点(2021/06/03)でUnityのWebGLはCPU推論のみのサポートです。<br><br>
<img src="https://user-images.githubusercontent.com/37477845/120523202-9a2b9380-c410-11eb-9589-bc30892fae3f.gif" width="60%"><br>
※上記イメージはブラウザ上でのWebGL実行(CPU推論)

# Demo
動作確認用ページは以下。<br>
[https://kazuhito00.github.io/Unity-Barracuda-Objectron-WebGL/WebGL-Build](https://kazuhito00.github.io/Unity-Barracuda-Objectron-WebGL/WebGL-Build/)

# Requirement (Unity)
* Unity 2021.1.0b6 or later
* Barracuda 1.3.0 or later

# FPS(参考値)
WebCamController.cs の Update()の呼び出し周期を計測したものです。<br>
推論に非同期処理のため、FPSは見かけ上のFPSであり、推論自体のFPSではありません。<br>
|  | Objectron(Shoe) |
| - | :- |
| WebGL<br>CPU：Core i7-8750H CPU @2.20GHz | 約2.9FPS<br>CSharpBurst |
| Unity Editor<br>GPU：GTX 1050 Ti Max-Q(4GB) | 約45FPS<br>ComputePrecompiled |

※下記イメージはUnity Editor上での実行(GPU推論)<br>
<img src="https://user-images.githubusercontent.com/37477845/120667929-ed611d00-c4c8-11eb-8347-79a958fef2fd.gif" width="60%"><br>

# Convert to ONNX
[tflite2onnx.ipynb](tflite2onnx.ipynb)を上から順に実行してください。

# Reference
* [Barracuda 1.3.0 preview](https://docs.unity3d.com/Packages/com.unity.barracuda@1.3/manual/index.html)
* [MediaPipe Objectron](https://google.github.io/mediapipe/solutions/objectron)

# ToDo
- [x] ~~worker.Execute()に渡す入力形式をWebGLビルドと通常ビルドで切り分ける<br>(WebGLビルド：float配列、通常ビルド：Texture)~~

# Author
高橋かずひと(https://twitter.com/KzhtTkhs)

# License 
Unity-Barracuda-Objectron-WebGL is under [Apache v2 License](LICENSE).
