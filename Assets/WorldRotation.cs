using UnityEngine;
using System.Collections;

public class WorldRotation : MonoBehaviour
{
  /*
     Mike's Notes:
       Just cleaner code to establish these as constants.
       This isn't super good either, since the contents need to be readonly as well.
          Looking into that.
   */ 
  // Directional Constants
  readonly Vector2 GRAVITY_PULL_DOWN  = new Vector2( 0.0f, -1.0f);
  readonly Vector2 GRAVITY_PULL_UP    = new Vector2( 0.0f,  1.0f);
  readonly Vector2 GRAVITY_PULL_RIGHT = new Vector2( 1.0f,  0.0f);
  readonly Vector2 GRAVITY_PULL_LEFT  = new Vector2(-1.0f,  0.0f);

  const float FLOAT_DELTA = 0.0001f;

  //Speed of the rotation
  [Range(0.0f, 100.0f)]
  public float Rotation_Speed;

  //In seconds
  [Range(0.0f, float.MaxValue)] // recommend we don't allow negative cd.
  public float Rotation_Cooldown;

  /*
     Mike's Notes:
       C# automatically initializes members to their default values
   */
  private float cooldown;
  private float playerOrientation;  // self defined - 0.0 indicates ABSOLUTE UP
  private float absOrientation;     // For easy baby math
  private new Transform camera;


  void SetWorldGravity()
  {
    /*
       Michael's Notes
          Change code to use new constants defined above.
          Took the overhead of getting abs Orientation to reduce if/else complexity.
          This version just uses reference changes; no allocations.
          Don't allow for potential floating point error to much up comparisons.
     */

    if (Mathf.Abs(absOrientation - 0.0f) < FLOAT_DELTA)
    {
      Physics2D.gravity = GRAVITY_PULL_DOWN;
    }
    else if (Mathf.Abs(absOrientation - 90.0f) < FLOAT_DELTA)
    {
      Physics2D.gravity = GRAVITY_PULL_RIGHT;
    }
    else if (Mathf.Abs(absOrientation - 180.0f) < FLOAT_DELTA)
    {
      Physics2D.gravity = GRAVITY_PULL_UP;
    }
    else if (Mathf.Abs(absOrientation - 270.0f) < FLOAT_DELTA)
    {
      Physics2D.gravity = GRAVITY_PULL_LEFT;
    }
  }

  void Rotate(float orientationChange)
  {
    Debug.Log("Old orientation = " + playerOrientation + " (" + absOrientation + ")");
    playerOrientation += orientationChange;
    absOrientation = Mathf.Abs(playerOrientation);
    Debug.Log("New orientation = " + playerOrientation + " (" + absOrientation + ")");

    if (Mathf.Abs(absOrientation - 360.0f) < FLOAT_DELTA)
    {
      playerOrientation = 0.0f;
      absOrientation = 0.0f;
      Debug.Log("Adjusted orientation = " + playerOrientation + " (" + absOrientation + ")");
    }

    this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
    Debug.Log("Player Transform = " + this.transform.eulerAngles);

    camera.rotation = Quaternion.Slerp(camera.rotation, this.transform.rotation, Rotation_Speed * .01f);
    Debug.Log("Camera rotaiton = " + camera.rotation);

    //SetWorldGravity();

    cooldown = Rotation_Cooldown;
  }

  void Start()
  {
    camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    /*
      Mike's Notes:
        A wonderful thing they do at IW is they just hard assert if something's wrong. 
        That way if an assumed condition is every failed, shit breaks and you HAVE to fix it before shipping.
    */
    Debug.Assert( camera != null , "No camera attached to object with WorldRotation component. Attach camera and try again." );

    /*
      Mike's Notes:
        Wanted to check if it were faster to FLOP then compare, or compare then flop if needed.
        Turns out it's faster to just flop.
        Left it here so you can check it out if you'd like.
    */
    //DO_TEST();
  }

  // Update is called once per frame
  void Update()
  {
    /*
      Mike's Notes:
        Save them flops.
    */

    cooldown -= Time.deltaTime;
    if (cooldown < 0.0f)
    {
      /*
        Mike's Notes:
          Move the contents of each action out 
      */

      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
        Rotate(90.0f);
      }
      else if (Input.GetKeyDown(KeyCode.RightArrow))
      {
        Rotate(-90.0f);
      }
    }
  }

  void DO_TEST()
  {
    Debug.Log("DOING: Compare tests");
    Debug.Log("\t" + TEST_COMPARE());
    Debug.Log("\t" + TEST_COMPARE());
    Debug.Log("\t" + TEST_COMPARE());
    Debug.Log("\t" + TEST_COMPARE());
    Debug.Log("\t" + TEST_COMPARE());

    Debug.Log("DOING: Flop tests");
    Debug.Log("\t" + TEST_FLOP());
    Debug.Log("\t" + TEST_FLOP());
    Debug.Log("\t" + TEST_FLOP());
    Debug.Log("\t" + TEST_FLOP());
    Debug.Log("\t" + TEST_FLOP());
  }

  long TEST_COMPARE()
  {
    long result = 0;
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();
    float time = 1.0f;
    for (int i = 0; i < 1024 * 1024; ++i)
    {
      if (0.0f < time)
      {
        time -= Time.deltaTime;
      }
      else
      {
        time = 1.0f;
      }
    }
    sw.Stop();
    result = sw.ElapsedTicks;
    return result;
  }

  long TEST_FLOP()
  {
    long result = 0;
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();
    float time = 1.0f;
    for (int i = 0; i < 1024 * 1024; ++i)
    {
      time -= Time.deltaTime;
      if (time < 0.0f)
      {
        time = 1.0f;
      }
    }
    sw.Stop();
    result = sw.ElapsedTicks;
    return result;
  }
}
