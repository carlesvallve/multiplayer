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


  public override void OnStartLocalPlayer() {
    //Material mat = transform.Find("ToonSoldier_demo/").GetComponent<MeshRenderer>().material
    mesh.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
    anim = GetComponent<Anim>();

    if (isLocalPlayer) {
      Camera.main.GetComponent<ThirdPersonCamera>().target = transform;
    }
  }

  void Update() {
    if (!isLocalPlayer) {
      return;
    }

    var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
    var x = Input.GetAxis("Horizontal") * Time.deltaTime * (360f * (1-Mathf.Abs(z))); //150.0f;

    transform.Rotate(0, x, 0);

    // if (state == PlayerState.Shoot) {
    //   return;
    // }

    transform.Translate(0, 0, z);

    if (Mathf.Abs(z) > 0.01f) {
      state = PlayerState.Run;
    } else {
      state = PlayerState.Idle;
    }

    if (Input.GetKeyDown(KeyCode.Space)) {
      StartCoroutine(Fire());
    }

    anim.UpdateAnimations(state);
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
    var bullet = (GameObject)Instantiate (
      bulletPrefab,
      bulletSpawn.position,
      bulletSpawn.rotation
    );

    // Add velocity to the bullet
    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

    // Spawn the bullet on the Clients
    NetworkServer.Spawn(bullet);

    // Destroy the bullet after 2 seconds
    Destroy(bullet, 2.0f);
  }
}
