using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
  public static Stage Instance;
  public GameObject player;
  public GameObject columnPrefab;
  public float obstacleOpeningSize = 0.6f;
  public int stageBandOffset = 0;
  public int minObstacleDistance = 3;
  public int maxObstacleDistance = 8;

  [Range(0.0f, 0.2f)]
  public float columnOriginDrift = 0.2f;
  [Range(0.0f, 0.2f)]
  public float columnOriginBound = 0.2f;
  [Range(0.0f, 0.15f)]
  public float columnClearing = 0.2f;
  public float screenHeight {get; private set;}

  private Column[] columns;
  [SerializeField]
  private int obstacleDistance;
  private float columnWidth;
  private float obstacleThreshold;
  private float repositionThreshold;
  private float screenWidth;
  private float lastOrigin = 0.5f;
  private float playerXPosition;

  void Awake()
  {
    if (Instance == null)
    {
        Instance = this;
    }
    else if (Instance != this)
    {
        Destroy(gameObject);
    }

    SetInitialValues();
    SetDistanceToNextObstacle();
  }

  void Start ()
  {
    InitializeColumns();
    SetPlayerStartPosition();
  }

  void Update ()
  {
    UpdateColumns();
  }

  public void ResetStage()
  {
    for (int i = 0; i < columns.Length; i++)
    {
        columns[i].ClearObstacle();
    }
    SetDistanceToNextObstacle();
  }

  private void SetInitialValues()
  {
    screenHeight = Camera.main.orthographicSize * 2.0f;
    screenWidth = screenHeight / Screen.height * Screen.width;
    columnWidth = screenWidth / 8.0f;
    obstacleThreshold = (screenWidth / 2.0f) * 0.7f;
    repositionThreshold = -((screenWidth / 2.0f) + columnWidth / 2.0f);
  }

  private void InitializeColumns()
  {
    columns = new Column[9];

    for (int i = 0; i < 9; i++)
    {
        GameObject columnGameObject = (GameObject) Instantiate(columnPrefab);
        Column column = columnGameObject.GetComponent<Column>();
        column.SetInitialValues();
        column.name = "Column " + i;
        column.transform.parent = this.transform;
        column.transform.position = GetStartPositionForColumn(i);
        column.SetScreenHeight(screenHeight);
        column.SetWidth(columnWidth);
        column.SetScrollSpeed(GameManager.Instance.scrollSpeed);
        columns[i] = column;
    }
  }

  private void UpdateColumns()
  {
    for (int i = 0; i < columns.Length; i++)
    {
        CheckRepositionThreshold(i);
        CheckObstacleThreshold(i);
        UpdateAmplitudeForColumn(i);
    }
  }

  private void CheckRepositionThreshold(int i)
  {
    if (columns[i].transform.position.x < repositionThreshold)
    {
        float currentX = columns[i].transform.position.x;
        float y = columns[i].transform.position.y;
        float x = currentX + columnWidth * columns.Length;

        columns[i].transform.position = new Vector2(x, y);

        if (GameManager.Instance.IsPlaying() == true)
        {
            if (--obstacleDistance == 0)
            {
                columns[i].isToBecomeObstacle = true;
                SetDistanceToNextObstacle();
            }
        }
    }
  }

  private void CheckObstacleThreshold(int i)
  {
    float position = columns[i].transform.position.x;

    if ((columns[i].IsObstacle() == true) && (position < playerXPosition))
    {
        columns[i].ClearObstacle();
    }
    else if ((columns[i].isToBecomeObstacle == true) && (position < obstacleThreshold))
    {
        float origin = DetermineColumnOrigin();
        float columnClearingHeight = columnClearing * screenHeight;
        columns[i].BecomeObstacle(origin, columnClearingHeight);
    }
  }

  private void UpdateAmplitudeForColumn(int i)
  {
    if ((columns[i].IsObstacle() == false))
    {
        columns[i].SetAmplitudesForBandWithOffset(i, stageBandOffset);
    }
  }

  // Helper

  private void SetPlayerStartPosition()
  {
    playerXPosition = (screenWidth * 0.15f) - (screenWidth / 2.0f);
    Vector3 position = new Vector3(playerXPosition, 0.0f, 0.0f);
    player.GetComponent<Player>().SetStartPosition(position);
  }

  private void SetDistanceToNextObstacle()
  {
    obstacleDistance = Random.Range(minObstacleDistance, maxObstacleDistance);
  }

  private float DetermineColumnOrigin()
  {
    float upperbound = 0.5f + columnOriginBound;
    float lowerbound = 0.5f - columnOriginBound;
    float nextOrigin = GetNextOrigin();

    while ((nextOrigin > upperbound) || (nextOrigin < lowerbound))
    {
        nextOrigin = GetNextOrigin();
    }

    lastOrigin = nextOrigin;

    return nextOrigin * screenHeight;
  }

  private Vector3 GetStartPositionForColumn(int i)
  {
    float dx = columnWidth;
    float xStart = -(screenWidth / 2.0f) + dx;
    float x = xStart + i * dx;

    return new Vector3(x, 0, -1);
  }

  private float GetNextOrigin()
  {
    return lastOrigin + Random.Range(-columnOriginDrift, columnOriginDrift);
  }
}

