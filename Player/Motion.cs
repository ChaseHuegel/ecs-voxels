using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Motion : MonoBehaviour
{
    public string name = "";

    [Header("Bone")]
	public int boneID = -1;
	public string boneName = null;


    [Header("Motion")]
    public bool playOnce = false;
    public bool baseMotion = false;
	public int frameCount = 60;
	public AnimationCurve curve;

    [Header("Constraints")]

    [Range(-180f, 180f)]
    public float angleMin = 0.0f;

    [Range(-180f, 180f)]
    public float angleMax = 0.0f;

    private bool playing = true;

    public float getValueAtTime(float _frameTime)
    {
        if (playOnce == true && _frameTime >= 0.9f)
        {
            Stop();
        }

        if (curve != null)
        {
            return curve.Evaluate(_frameTime) * (angleMax - angleMin) + angleMin;
        }
        else
        {
            return Mathf.Lerp(angleMin, angleMax, _frameTime);
		}
	}

	public int getBoneID()
	{
		if (boneID == -1) { return boneName.GetHashCode(); }
		else { return boneID; }
	}

	public bool isBaseMotion()
	{
		return baseMotion;
	}

    public bool isPlaying()
	{
		return playing;
	}

    public void Play()
    {
        playing = true;
    }

    public void Stop()
    {
        playing = false;
    }
}