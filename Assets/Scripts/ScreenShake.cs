using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using EZCameraShake;

public struct ScreenShakeInfo
{
    public float shakeMag;
    public float shakeRou;
    public float shakeFadeIDur;
    public float shakeFadeODur;
    public Vector3 shakePosInfluence;
    public Vector3 shakeRotInfluence;
}

public class ScreenShake : MonoBehaviour
{
    private ScreenShakeInfo m_shakeInfo;        // Info needed for current screen shake

    private float m_startTime = 0f;             // Time screen shake started
    private float m_duration = 0f;              // Duration of current shake
    private int m_priority = -1;                // Priority of current shake (-1 if not shaking)

    public bool isShaking { get { return m_priority >= 0; } }

    void Update()
    {
        if (isShaking)
            UpdateShake();
    }

    // Will attempt to start a new screenshake, returns true if new shake has started, false otherwise
    public bool StartShaking(ScreenShakeInfo info, float duration, int priority)
    {
        if (duration <= 0f)
        {
            Debug.LogError("Duration is zero or less, expecting value greater than zero");
            return false;
        }

        if (isShaking)
        {
            if (priority < m_priority)
                return false;

            // TODO: Could have an event here for when cancelling the current screen shake
        }

        m_shakeInfo = info;
        m_duration = duration;
        m_priority = priority;

        m_startTime = Time.time;

        // Call it once with value of zero to 'reset' it
        UpdateShake(0f);

        return true;
    }

    private void UpdateShake()
    {
        Assert.IsTrue(isShaking);
        Assert.IsFalse(Mathf.Approximately(m_duration, 0f));

        float alpha = Mathf.Clamp01((Time.time - m_startTime) / m_duration);
        UpdateShake(Ease(alpha));
        
        if (alpha >= 1f)
        {
            // Shake is done, tidy up
            m_priority = -1;

            // TODO: Could have an event here for when finishing the current screen shake
        }
    }

    private void UpdateShake(float alpha)
    {
        //float example = Mathf.Lerp(m_shakeInfo.m_example1, m_shakeInfo.m_example2, alpha);

        // Shake camera, amplified by player shoot amplifiers
        CameraShaker.Instance.ShakeOnce(m_shakeInfo.shakeMag, m_shakeInfo.shakeRou,
            m_shakeInfo.shakeFadeIDur, m_shakeInfo.shakeFadeODur, m_shakeInfo.shakePosInfluence,
            m_shakeInfo.shakeRotInfluence);
    }

    private float Ease(float alpha)
    {
        // Linear iterpolation
        return alpha;
    }
}
