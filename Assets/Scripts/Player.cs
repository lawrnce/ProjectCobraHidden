using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public float upForce;
  public Vector3 startPosition;

  private Animator anim;
  private Rigidbody2D rb2d;

  void Start ()
  {
    anim = GetComponent<Animator>();
    rb2d = GetComponent<Rigidbody2D>();
  }

  void FixedUpdate ()
  {
    if (GameManager.Instance.IsReady())
    {
        rb2d.gravityScale = 0.0f;
        rb2d.velocity = Vector2.zero;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        rb2d.freezeRotation = true;
    }
    else if (GameManager.Instance.IsPlaying())
    {
        rb2d.gravityScale = 1.0f;

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Flap");
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(0, upForce));
        }
    }
    else if (GameManager.Instance.IsOver())
    {
        rb2d.freezeRotation = false;
    }
  }

  void OnCollisionEnter2D(Collision2D other)
  {
    if (other.gameObject.tag == "Column")
    {
        GameManager.Instance.GameOver();
    }
  }

  public void SetStartPosition(Vector3 position)
  {
    startPosition = position;
    transform.position = position;
  }
}
