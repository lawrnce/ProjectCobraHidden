using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
  public float animationTime;
  public bool isToBecomeObstacle;
  ColumnChild topColumnChild;
  ColumnChild bottomColumnChild;

  private bool isObstacle = false;
  private int index;
  private int offset;
  private float screenHeight;

  public void SetInitialValues()
  {
    topColumnChild = gameObject.transform.GetChild(0).GetComponent<ColumnChild>();
    bottomColumnChild = gameObject.transform.GetChild(1).GetComponent<ColumnChild>();
    animationTime = 0.2f;
    isObstacle = false;
    isToBecomeObstacle = false;
  }

  public bool IsObstacle()
  {
    return isObstacle || topColumnChild.isLerping || bottomColumnChild.isLerping;
  }

  public void BecomeObstacle(float origin, float clearing)
  {
    if (isObstacle == false)
    {
        float dy = clearing / 2.0f;
        float topHeight = 2.0f * (screenHeight - (origin + dy));
        float bottomHeight = 2.0f * (origin - dy);

        topColumnChild.BecomeObstacleWithHeight(topHeight);
        bottomColumnChild.BecomeObstacleWithHeight(bottomHeight);
        isToBecomeObstacle = false;
        isObstacle = true;
    }
  }

  public void ClearObstacle()
  {
    if (isObstacle == true)
    {
        topColumnChild.ClearObstacleWithAudioBand((index + offset) % 8);
        bottomColumnChild.ClearObstacleWithAudioBand(index);
        GameManager.Instance.ClearedObstacle();
        isObstacle = false;
    }
  }

  public void SetScreenHeight(float height)
  {
    screenHeight = height;
    float yMax = screenHeight / 2.0f;
    topColumnChild.SetYPosition(yMax);
    bottomColumnChild.SetYPosition(-yMax);
  }

  public void SetWidth(float width)
  {
    topColumnChild.SetWidth(width);
    bottomColumnChild.SetWidth(width);
  }

  public void SetAmplitudesForBandWithOffset(int band, int o)
  {
    index = band % 8;
    offset = o;
    topColumnChild.SetAmplitudeForBand((index + offset) % 8);
    bottomColumnChild.SetAmplitudeForBand(index);
  }

  public void SetScrollSpeed(float speed)
  {
    GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
  }
}

