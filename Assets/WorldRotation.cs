using UnityEngine;
using System.Collections;

public class WorldRotation : MonoBehaviour
{

  //Speed of the rotation
  [Range(0.0f, 100.0f)]
  public float Rotation_Speed;

  //In seconds
  public float Rotation_Cooldown;

  private float cooldown;
  private float playerOrientation = 0.0f;
  private new Transform camera;


  void SetWorldGravity()
  {
    //Up and down
    if (playerOrientation == 0.0f)
      Physics2D.gravity = new Vector2(0.0f, -1.0f);
    else if (playerOrientation == 180.0f || playerOrientation == -180.0f)
      Physics2D.gravity = new Vector2(0.0f, 1.0f);
    //Left and Right
    else if(playerOrientation == 90.0f || playerOrientation == -270.0f)
      Physics2D.gravity = new Vector2(1.0f, .0f);
    else if(playerOrientation == -90.0f || playerOrientation == 270.0f)
      Physics2D.gravity = new Vector2(-1.0f, .0f);
  }
  void Start()
  {
    camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    cooldown = 0.0f;
  }

  // Update is called once per frame
  void Update()
  {
    cooldown -= Time.deltaTime;
    if (cooldown < 0)
    {
      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
        playerOrientation += 90.0f;
        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
        cooldown = Rotation_Cooldown;
      }
      else if (Input.GetKeyDown(KeyCode.RightArrow))
      {
        playerOrientation -= 90.0f;
        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
        cooldown = Rotation_Cooldown;
      }
      if (playerOrientation == -360 || playerOrientation == 360)
        playerOrientation = 0;

    }
    camera.rotation = Quaternion.Slerp(camera.rotation, this.transform.rotation, Rotation_Speed * .01f);
    SetWorldGravity();

  }
}
