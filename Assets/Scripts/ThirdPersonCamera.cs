using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Rewired;

public class ThirdPersonCamera : MonoBehaviour {

	//[Header("Rewired Properties")]
	//public int playerId;
	//private Player player;

	[Header("Camera Properties")]
  public float distanceAway = 5f;                  			// how far the camera is from the player.
  public float distanceUp = 0f;                    			// how high the camera is above the player
  public float smooth = 5.0f;                 					// how smooth the camera moves into place
	public float smoothCollision = 10f;
  public float rotateAround = 70f;            					// angle at which the camera is rotated around the target
	public float camRotateSpeed = 180f;										// speed at which the camera rotates around the target
	public float camZoomSpeed = 3f;												// speed at which the camera zooms towards the target

	public float cameraHeight = 90f; //55f;
  public float cameraPan = 0f;

	[Header("Player to follow")]
  public Transform target;															// the target the camera follows
	public Vector3 targetOffset = new Vector3(0, 1f, 0);	// offset from the target's transform

	[Header("Layer(s) to include")]
  public LayerMask CamOcclusion;              					// the layers that will be affected by collision
	public float collisionMargin = 0.5f;									// margin distance for camera collisions



	private RaycastHit hit;
  private Vector3 camPosition;
  private Vector3 camMask;


  void Start () {
		if (!target) {
			//Debug.LogError("ThirdPersonCamera needs a target transform. Please assign one.");
			return;
		}

		// rewired player
		//player = ReInput.players.GetPlayer(playerId);

    // the statement below automatically positions the camera behind the target.
    rotateAround = target.eulerAngles.y - 45f;
  }


  void Update() {

  }


  void LateUpdate () {
		if (!target) { return; }

		// user interaction: rotate and zoom
		//float horizontalAxis = player.GetAxis("Rotate");
		//float verticalAxis = player.GetAxis ("Zoom");
    float horizontalAxis = 0;
    float verticalAxis = Input.GetAxis("Mouse ScrollWheel");


		rotateAround += horizontalAxis * camRotateSpeed * Time.deltaTime;
		distanceAway = Mathf.Clamp(distanceAway -= verticalAxis * camZoomSpeed * Time.deltaTime, 2f, 20f);
		//distanceUp += verticalAxis * 0.25f;

		// wrap the cam orbit rotation
    if (rotateAround > 360) {
      rotateAround = 0f;
    } else if (rotateAround < 0f) {
      rotateAround = (rotateAround + 360f);
    }

		Vector3 currentTargetOffset = target.position + targetOffset;
    Quaternion rotation = Quaternion.Euler(cameraHeight, rotateAround, cameraPan);
    Vector3 vectorMask = Vector3.one;
    Vector3 rotateVector = rotation * vectorMask;

    //this determines where both the camera and it's mask will be.
    //the camMask is for forcing the camera to push away from walls.
    camPosition = currentTargetOffset + (Vector3.up * distanceUp) - (rotateVector * distanceAway);
    camMask = currentTargetOffset + (Vector3.up * distanceUp) - (rotateVector * distanceAway);

    bool colliding = occludeRay(ref currentTargetOffset);
    smoothCamMethod(colliding == true ? smoothCollision : smooth);

		// make sure that the camera is always pointing at the target
    transform.LookAt(target.transform.position + targetOffset);
  }


  private void smoothCamMethod(float currentSmooth) {
    transform.position = Vector3.Lerp (transform.position, camPosition, Time.deltaTime * currentSmooth);
  }


	// prevent walls and terrain clipping
  private bool occludeRay(ref Vector3 targetFollow){

    // declare a new raycast hit.
    RaycastHit wallHit = new RaycastHit();

    // linecast from your player (targetFollow) to the camera's mask (camMask) to find collisions.
    if (Physics.Linecast(targetFollow, camMask, out wallHit, CamOcclusion)) {

      // the x and z coordinates are pushed away from the wall by hit.normal. the y coordinate stays the same.
      camPosition = new Vector3(
				wallHit.point.x + wallHit.normal.x * collisionMargin,
				camPosition.y,
				wallHit.point.z + wallHit.normal.z * collisionMargin
			);

			// adjust the camera position to always be over the terrain
			if (Terrain.activeTerrain) {
				float currentHeight = Terrain.activeTerrain.SampleHeight(camPosition) + collisionMargin;
				if (camPosition.y < currentHeight) {
					camPosition = new Vector3(camPosition.x, currentHeight, camPosition.z);
					//camPosition = new Vector3(wallHit.point.x, currentHeight, wallHit.point.z);
				}
			}

			return true;
    }

		return false;
  }

}
