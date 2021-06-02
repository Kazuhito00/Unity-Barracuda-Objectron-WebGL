using UnityEngine;

public sealed class FPSCounter
{
    private readonly int m_updateRate;

    private int   frameCount;
    private float deltaTime;
    private float fps;

    public float FPS => fps;

    public FPSCounter( int updateRate = 4 )
    {
        m_updateRate = updateRate;
    }

    public void Update()
    {
        deltaTime += Time.unscaledDeltaTime;

        frameCount++;

        if ( !( deltaTime > 1f / m_updateRate ) ) return;

        fps = frameCount / deltaTime;

        deltaTime  = 0;
        frameCount = 0;
    }
}