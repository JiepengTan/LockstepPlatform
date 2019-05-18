// This script controls an onscreen touch button. Most of this code exists purely to provide
// a simple input method, and as such, won't be fully commented

using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	int pointerID;
	bool buttonHeld;
	bool buttonPressed;
	
	void Awake ()
	{
		pointerID = -999;
	}

	//When the screen is first touched
	public void OnPointerDown(PointerEventData data)
	{
		if (pointerID != -999)
			return;
		
		pointerID = data.pointerId;
		
		buttonHeld = true;
		buttonPressed = true;
	}

	//When the finger leaves the screen
	public void OnPointerUp (PointerEventData data)
	{
		if (data.pointerId != pointerID)
			return;
		
		pointerID = -999;
		buttonHeld = false;
		buttonPressed = false;
	}

	//Functions like Input.GetButtonDown()
	public bool GetButtonDown () 
	{
		if (buttonPressed)
		{
			buttonPressed = false;
			return true;
		}

		return false;
	}

	//Functions like Input.GetButton()
	public bool GetButton()
	{
		return buttonHeld;
	}
}