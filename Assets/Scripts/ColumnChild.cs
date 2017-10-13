using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnChild : MonoBehaviour
{
  public bool isLerping = false;
  public bool isObstacle = false;
  public float lerpTime = 0.3f;
  public float amplitudeDrift = 0.03f;

  private Vector3 startLerp;
  private Vector3 endLerp;
  private float currentLerpTime;
  private float pixelSize = 0.0f;

  void FixedUpdate ()
  {
    if (isLerping == true)
    {
        currentLerpTime += Time.deltaTime;

        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }

        float perc = currentLerpTime / lerpTime;
        transform.localScale = Vector3.Lerp(startLerp, endLerp, perc);

        if (perc >= 1.0f)
        {
            isLerping = false;
            currentLerpTime = 0f;
        }
    }
    else if (isObstacle == true)
    {
        SetObstacleFlutter();
    }
  }

  public void SetAmplitudeForBand(int i)
  {
    transform.localScale = GetAudioBandAmplitudeScale(i);
  }

  public void SetYPosition(float y)
  {
    transform.position = new Vector3(transform.position.x, y, 0);
  }

  public void SetWidth(float width)
  {
    GetPixelSizeIfNeeded();
    float widthScale = width / pixelSize;
    float currentHeightScale = transform.localScale.y;
    transform.localScale = new Vector3(widthScale, currentHeightScale, 0);
  }

  public void BecomeObstacleWithHeight(float height)
  {
    float heightScale = height / pixelSize;
    startLerp = transform.localScale;
    endLerp = new Vector3(transform.localScale.x, heightScale, 0);
    isLerping = true;
    isObstacle = true;
  }

  public void ClearObstacleWithAudioBand(int i)
  {
    startLerp = transform.localScale;
    endLerp = GetAudioBandAmplitudeScale(i);
    isLerping = true;
    isObstacle = false;
  }

  // Helper

  private void GetPixelSizeIfNeeded()
  {
    if (pixelSize == 0.0f)
    {
        pixelSize = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
    }
  }

  private Vector3 GetAudioBandAmplitudeScale(int i)
  {
    float amplitude = AudioBands.Instance.amplitudes[i];
    float height = Stage.Instance.screenHeight * amplitude * 2.0f;
    float heightScale = height / pixelSize;
    float currentWidthScale = transform.localScale.x;
    return new Vector3(currentWidthScale, heightScale, 0);
  }

  private void SetObstacleFlutter()
  {
    float maxHeight = (endLerp.y * pixelSize) / 2.0f;
    float minHeight = (maxHeight - amplitudeDrift * Stage.Instance.screenHeight);
    float distance = Mathf.Abs(maxHeight - minHeight);
    float height = maxHeight - (distance * AudioBands.Instance.averageAmplitude);
    float heightScale = (height * 2.0f) / pixelSize;
    float currentWidthScale = transform.localScale.x;

    transform.localScale = new Vector3(currentWidthScale, heightScale, 0);
  }
}

