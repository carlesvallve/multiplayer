// https://unity3d.com/learn/tutorials/topics/multiplayer-networking/introduction-simple-multiplayer-example?playlist=29690

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public enum PlayerState {
  Idle = 0,
  Run = 1,
  Shoot = 2,
  Burst = 3
}

public class PlayerController : NetworkBehaviour {

  public GameObject bulletPrefab;
  public Transform bulletSpawn;
  public GameObject mesh;

  private Anim anim;
  public PlayerState state = PlayerState.Idle;

  private Vector2 input;
  private bool moving = false;
  private float speed = 3.0f;


  void Start() {
    transform.SetParent(GameObject.Find("Players").transform);
  }

  public override void OnStartLocalPlayer() {
    mesh.GetComponent<SkinnedMeshRenderer>().material.color = Color.cyan;
    anim = GetComponent<Anim>();
    Camera.main.GetComponent<ThirdPersonCamera>().target = transform;
    InitJoystickManager();
  }

  void Update() {
    if (!isLocalPlayer) {
      return;
    }

    // rotate
    Vector3 targetDirection = GetMovementVectorRelativeToCamera(input.x, input.y);
    if (targetDirection != Vector3.zero) {
      transform.rotation = Quaternion.LookRotation (targetDirection);
    }

    // translate
    if (moving) {
      transform.Translate(0, 0, 1f * Time.deltaTime * speed);
      state = PlayerState.Run;
    } else {
      state = PlayerState.Idle;
    }

    // var z = input.y * Time.deltaTime * 3.0f;
    // var x = input.x * Time.deltaTime * (360f * (1-Mathf.Abs(z))); //150.0f;
    // transform.Rotate(0, x, 0);
    // transform.Translate(0, 0, z);
    // if (Mathf.Abs(z) > 0.01f) {
    //   state = PlayerState.Run;
    // } else {
    //   state = PlayerState.Idle;
    // }
    // if (Input.GetKeyDown(KeyCode.Space)) {
    //   StartCoroutine(Fire());
    // }

    // animate
    anim.UpdateAnimations(state);
  }


  private Vector3 GetMovementVectorRelativeToCamera (float h, float v) {
		// Forward vector relative to the camera along the x-z plane
		Transform cameraTransform = Camera.main.transform;
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;

		// Right vector relative to the camera, orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		// Target direction relative to the camera
		Vector3 targetDirection = h * right + v * forward;
		return targetDirection;
	}

  private IEnumerator Fire() {
    CmdFire();
    state = PlayerState.Shoot;
    yield return new WaitForSeconds(0.5f);
    state = PlayerState.Idle;
  }

  // This [Command] code is called on the Client …
  // … but it is run on the Server!
  [Command]
  void CmdFire() {
    // Create the Bullet from the Bullet Prefab
    var bullet = (GameObject)Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

    // Spawn the bullet on the Clients
    NetworkServer.Spawn(bullet);

    // Destroy the bullet after 2 seconds
    Destroy(bullet, 2.0f);
  }

  // ========================================================
	// Input
	// ========================================================

  void InitJoystickManager () {
    // NOTE: We could do it using Kichen events, but prefer a dependency-free implementation since is already done
    JoystickManager joystickManager = FindObjectOfType<JoystickManager>();
    joystickManager.onDirection += SetDirection;
    joystickManager.onButtonADown += SetButtonADown;
    joystickManager.onButtonBDown += SetButtonBDown;
    joystickManager.onButtonCDown += SetButtonCDown;
    joystickManager.onButtonAUp += SetButtonAUp;
    joystickManager.onButtonBUp += SetButtonBUp;
    joystickManager.onButtonCUp += SetButtonCUp;
	}

  void SetDirection(JoystickAction joystickAction) {
    // velocity always forced to 1f
    // input = new Vector2(
    //   joystickAction.direction.x == 0 ? 0 : Mathf.Sign(joystickAction.direction.x),
    //   joystickAction.direction.y == 0 ? 0 : Mathf.Sign(joystickAction.direction.y)
    // );

    // velocity smoothed to joystick distance
    input = joystickAction.direction;
  }

  // Buttons Down

  void SetButtonADown(JoystickAction joystickAction) {
    //Debug.Log("A");
    moving = true;
  }

  void SetButtonBDown(JoystickAction joystickAction) {
    //Debug.Log("B");
    StartCoroutine(Fire());
  }

  void SetButtonCDown(JoystickAction joystickAction) {
    //Debug.Log("C");
  }


  // Buttons Up

  void SetButtonAUp(JoystickAction joystickAction) {
    //Debug.Log("A UP");
    moving = false;
  }

  void SetButtonBUp(JoystickAction joystickAction) {
    //Debug.Log("B UP");
  }

  void SetButtonCUp(JoystickAction joystickAction) {
    //Debug.Log("C UP");
  }
}
