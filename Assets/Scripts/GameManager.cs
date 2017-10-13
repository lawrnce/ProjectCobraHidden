using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum GameState {Ready, Play, Over};

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;
  public float scrollSpeed = -1.5f;
  public GameObject player;
  public GameObject instructionsText;
  public GameObject replayButton;
  public GameObject scoreText;
  [SerializeField]
  private GameState state;
  private int score;

  void Awake()
  {
    if (Instance == null)
    {
        Instance = this;
    }
    else if(Instance != this)
    {
        Destroy(gameObject);
    }
  }

  void Start ()
  {
    state = GameState.Ready;
  }

  void FixedUpdate ()
  {
    if (state == GameState.Ready)
    {
        instructionsText.SetActive(true);
        replayButton.SetActive(false);
        scoreText.SetActive(false);

        if (Input.GetMouseButtonDown(0))
        {
            instructionsText.SetActive(false);
            state = GameState.Play;
        }
    }
    else if (state == GameState.Play)
    {
        instructionsText.SetActive(false);
        replayButton.SetActive(false);
        scoreText.SetActive(true);
        scoreText.GetComponent<Text>().text = score.ToString();
    }
    else if (state == GameState.Over)
    {
        instructionsText.SetActive(false);
        replayButton.SetActive(true);
        scoreText.SetActive(false);
    }
  }

  public bool IsReady()
  {
    return (state == GameState.Ready);
  }

  public bool IsPlaying()
  {
    return (state == GameState.Play);
  }

  public bool IsOver()
  {
    return (state == GameState.Over);
  }

  public void ClearedObstacle()
  {
    score++;
  }

  public void GameOver()
  {
    state = GameState.Over;
    replayButton.SetActive(true);
    Stage.Instance.ResetStage();
  }

  public void ResetGame()
  {
    state = GameState.Ready;
    score = 0;
  }
}
