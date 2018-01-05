using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

  private float speed = 8f;

  void Start() {
    transform.SetParent(GameObject.Find("Bullets").transform);

    // Add velocity to the bullet
    GetComponent<Rigidbody>().velocity = transform.forward * speed;
  }

  void OnCollisionEnter(Collision collision) {
    var hit = collision.gameObject;
    var health = hit.transform.parent.GetComponent<Health>();
    if (health != null) {
      health.TakeDamage(10);
    }

    Destroy(gameObject);
  }
}
