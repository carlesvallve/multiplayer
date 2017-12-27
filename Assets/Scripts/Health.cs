using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour {

  public const int maxHealth = 100;

  public bool destroyOnDeath;

  [SyncVar(hook = "OnChangeHealth")]
  public int currentHealth = maxHealth;

  public RectTransform healthBar;

  private NetworkStartPosition[] spawnPoints;

  void Start() {
    if (!isLocalPlayer) {
      spawnPoints = FindObjectsOfType<NetworkStartPosition>();
    }
  }

  public void TakeDamage(int amount) {
    if (!isServer) { return; }

    currentHealth -= amount;
    if (currentHealth <= 0) {

      if (destroyOnDeath) {
        Destroy(gameObject);
      } else {
        currentHealth = maxHealth;
        // called on the Server, but invoked on the Clients
        RpcRespawn();
      }
    }
  }

  void OnChangeHealth (int health) {
    healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
  }

  // If the Server simply sets the Player GameObject’s position back to origin
  // when the player’s currentHealth reaches 0,
  // the Client would override the Server as the Client has authority.
  // To avoid this, the Server instructs the owning Client to move the player's GameObject
  // to the restart position as a ClientRpc call.
  // This position is then synchronized across all of the Clients
  // because of the player GameObject's NetworkTransform.
  [ClientRpc]
  void RpcRespawn() {
    if (isLocalPlayer) {
      // Set the spawn point to origin as a default value
      Vector3 spawnPoint = Vector3.zero;

      // If there is a spawn point array and the array is not empty, pick a spawn point at random
      if (spawnPoints != null && spawnPoints.Length > 0) {
        spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
      }

      // Set the player’s position to the chosen spawn point
      transform.position = spawnPoint;
    }
  }
}
