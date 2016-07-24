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
  //private float playerOrientation = 0.0f;
  private float playerOrientation; // self defined - 0.0 indicates ABSOLUTE UP
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
    float absOrientation = Mathf.Abs(playerOrientation);

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


    ////Up and down
    //if (playerOrientation == 0.0f)
    //  Physics2D.gravity = new Vector2(0.0f, -1.0f);
    //else if (playerOrientation == 180.0f || playerOrientation == -180.0f)
    //  Physics2D.gravity = new Vector2(0.0f, 1.0f);
    ////Left and Right
    //else if(playerOrientation == 90.0f || playerOrientation == -270.0f)
    //  Physics2D.gravity = new Vector2(1.0f, .0f);
    //else if(playerOrientation == -90.0f || playerOrientation == 270.0f)
    //  Physics2D.gravity = new Vector2(-1.0f, .0f);
  }

  void Rotate(float orientationChange)
  {
    playerOrientation += orientationChange;
    this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
    cooldown = Rotation_Cooldown;

    if (Mathf.Abs(playerOrientation - -360) < FLOAT_DELTA || Mathf.Abs(playerOrientation - 360) < FLOAT_DELTA)
    {
      playerOrientation = 0.0f;
    }

    camera.rotation = Quaternion.Slerp(camera.rotation, this.transform.rotation, Rotation_Speed * .01f);
    SetWorldGravity();
  }

  void Start()
  {
    camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    /*
      Mike's Notes:
        A wonderful thing they do at IW is they just hard assert if something's wrong. 
        That way if an assumed condition is every failed, shit breaks and you HAVE to fix it before shipping.
    */
    //Debug.Assert( camera != null , "No camera attached to object with WorldRotation component. Attach camera and try again." );

    /*
      Mike's Notes:
        Justification for wanting to do a comparison and then a flop in the update.
        Left it here so you can check it out if you'd like.
    */
    DO_TEST();

    // Again; already set to default float value.
    //cooldown = 0.0f;
  }

  // Update is called once per frame
  void Update()
  {
    /*
      Mike's Notes:
        Save them flops.
    */
    
    if (0.0f < cooldown)
    {
      cooldown -= Time.deltaTime;
    }
    else
    {
      /*
        Mike's Notes:
          Save them flops.
      */

      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
        Rotate(90.0f);
        //playerOrientation += 90.0f;
        //this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
        //cooldown = Rotation_Cooldown;
      }
      else if (Input.GetKeyDown(KeyCode.RightArrow))
      {
        Rotate(-90.0f);
        //playerOrientation -= 90.0f;
        //this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
        //cooldown = Rotation_Cooldown;
      }
    }

    //cooldown -= Time.deltaTime;
    //if (cooldown < 0)
    //{
    //  if (Input.GetKeyDown(KeyCode.LeftArrow))
    //  {
    //    playerOrientation += 90.0f;
    //    this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
    //    cooldown = Rotation_Cooldown;
    //  }
    //  else if (Input.GetKeyDown(KeyCode.RightArrow))
    //  {
    //    playerOrientation -= 90.0f;
    //    this.transform.eulerAngles = new Vector3(0.0f, 0.0f, playerOrientation);
    //    cooldown = Rotation_Cooldown;
    //  }
    //}


    /*
      Mike's Notes:
        Same kind fo stuff as in SetWorldGravity(). Float comparisons and etc.
    */
    //if (Mathf.Abs(playerOrientation - -360) < FLOAT_DELTA || Mathf.Abs(playerOrientation - 360) < FLOAT_DELTA)
    //{
    //  playerOrientation = 0.0f;
    //}

    //if (playerOrientation == -360 || playerOrientation == 360)
    //  playerOrientation = 0;

    //camera.rotation = Quaternion.Slerp(camera.rotation, this.transform.rotation, Rotation_Speed * .01f);
    //SetWorldGravity();
  }

  void DO_TEST()
  {
    System.Console.WriteLine("DOING: Compare tests");
    System.Console.WriteLine("\t" + TEST_COMPARE());
    System.Console.WriteLine("\t" + TEST_COMPARE());
    System.Console.WriteLine("\t" + TEST_COMPARE());
    System.Console.WriteLine("\t" + TEST_COMPARE());
    System.Console.WriteLine("\t" + TEST_COMPARE());

    System.Console.WriteLine("DOING: Flop tests");
    System.Console.WriteLine("\t" + TEST_FLOP());
    System.Console.WriteLine("\t" + TEST_FLOP());
    System.Console.WriteLine("\t" + TEST_FLOP());
    System.Console.WriteLine("\t" + TEST_FLOP());
    System.Console.WriteLine("\t" + TEST_FLOP());
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
        time = 0.0f;
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
