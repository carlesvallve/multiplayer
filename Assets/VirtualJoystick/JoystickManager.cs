using UnityEngine;
using System.Collections;

public struct JoystickAction {
  public Vector2 direction;
  public bool buttonADown;
  public bool buttonBDown;
  public bool buttonCDown;
  public bool buttonAUp;
  public bool buttonBUp;
  public bool buttonCUp;
}

public class JoystickManager : MonoBehaviour {

  public System.Action<JoystickAction> onDirection;
  public System.Action<JoystickAction> onButtonADown;
  public System.Action<JoystickAction> onButtonBDown;
  public System.Action<JoystickAction> onButtonCDown;
  public System.Action<JoystickAction> onButtonAUp;
  public System.Action<JoystickAction> onButtonBUp;
  public System.Action<JoystickAction> onButtonCUp;
  private JoystickAction currentJoystickAction = new JoystickAction();

  private float lastFrameTime = -1; // avoid innecessary updates
  private bool isVirtual = false;   // so we dont reset direction every frame while also using the virtual jostick


	void Start () {
		if (Input.GetJoystickNames().Length == 0) {
			print("No Joysticks connected.");
		} else {
			print("Joystick connected: " + Input.GetJoystickNames()[0]);
		}
	}


	void Update () {
    if (lastFrameTime == Time.time) { return; }
		lastFrameTime = Time.time;

    UpdateDirection();
    UpdateButtons();
	}

  public void SetDirection(Vector2 direction, bool _isVirtual = false) {
    isVirtual = _isVirtual;

    // set direction data
    currentJoystickAction.direction = direction;

    // Fire event
    if (onDirection != null) {
      onDirection(currentJoystickAction);
    }
  }

  public void SetButtonDown(string buttonId, bool value) {
    //Debug.Log("SetButtonDown " + buttonId + " " + value);

    switch(buttonId) {
    case "A":
      currentJoystickAction.buttonADown = value;
      if (value && onButtonADown != null) { onButtonADown(currentJoystickAction); }
      break;
    case "B":
      currentJoystickAction.buttonBDown = value;
      if (value && onButtonBDown != null) { onButtonBDown(currentJoystickAction); }
      break;
    case "C":
      currentJoystickAction.buttonCDown = value;
      if (value && onButtonCDown != null) { onButtonCDown(currentJoystickAction); }
      break;
    }
  }

  public void SetButtonUp(string buttonId, bool value) {
    switch(buttonId) {
    case "A":
      currentJoystickAction.buttonAUp = value;
      if (value && onButtonAUp != null) { onButtonAUp(currentJoystickAction); }
      break;
      case "B":
        currentJoystickAction.buttonBUp = value;
        if (value && onButtonBUp != null) { onButtonBUp(currentJoystickAction); }
        break;
      case "C":
        currentJoystickAction.buttonCUp = value;
        if (value && onButtonCUp != null) { onButtonCUp(currentJoystickAction); }
        break;
    }
  }


  void UpdateDirection() {
    if (currentJoystickAction.direction.magnitude > 0 && isVirtual) {
      return;
    }

    // direction by joystick
    Vector2 direction = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    direction = (direction.magnitude > 1) ? direction.normalized : direction;

    // direction by keys (vertical)
    if (Input.GetKey ("up")) {
      direction.y = 1f;
		} else if (Input.GetKey ("down")) {
			direction.y = -1f;
		} else {
			direction.y = 0;
		}

    // direction by keys (horizontal)
		if (Input.GetKey ("right")){
			direction.x = 1f;
		} else if (Input.GetKey ("left")){
			direction.x = -1f;
		} else {
			direction.x = 0f;
		}

    SetDirection(direction);
  }


  void UpdateButtons() {
    // A Down
    if (Input.GetButtonDown("A")) {
      SetButtonDown("A", true);
      SetButtonUp("A", false);
    }

    // B Down
    if (Input.GetButtonDown("B")) {
      SetButtonDown("B", true);
      SetButtonUp("B", false);
    }

    // C Down
    if (Input.GetButtonDown("C")) {
      SetButtonDown("C", true);
      SetButtonUp("C", false);
    }


    // A Up
    if (Input.GetButtonUp("A")) {
      SetButtonDown("A", false);
      SetButtonUp("A", true);
    }

    // B Up
    if (Input.GetButtonUp("B")) {
      SetButtonDown("B", false);
      SetButtonUp("B", true);
    }

    // C Up
    if (Input.GetButtonUp("C")) {
      SetButtonDown("C", false);
      SetButtonUp("C", true);
    }

  }

}
