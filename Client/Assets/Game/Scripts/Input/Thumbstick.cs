// This script controls an onscreen touch thumbstick. Most of the functionality is around
// clamping input and controlling cosmetics. Most of this code exists purely to provide
// a simple input method, and as such, won't be fully commented

using UnityEngine;
using UnityEngine.EventSystems;

public class Thumbstick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{	
	public float smoothing = 5f;		//Controls the smoothness of thumbstick inputs
	public RectTransform thumbImage;
	public float deadZone = .25f;		//Deadzone prevents very small variations

	int pointerID;
	Vector2 center;
	Vector2 direction;
	Vector2 smoothDirection;

	float minX;
	float minY;
	float maxX;
	float maxY;

	bool thumbImageMoving;

	void Awake ()
	{
		//Set initial values and determine bounds
		direction = Vector2.zero;
		pointerID = -999;
		center = transform.position;

		RectTransform rect = GetComponent<RectTransform>();
		minX = center.x - rect.rect.width / 2f;
		maxX = center.x + rect.rect.width / 2f;
		minY = center.y - rect.rect.height / 2f;
		maxY = center.y + rect.rect.height / 2f;
	}

	void Update()
	{
		//Move thumbstick back to center if not being controlled
		if (pointerID != -999 || !thumbImageMoving)
			return;

		thumbImage.position = Vector3.Lerp(thumbImage.position, center, Time.deltaTime * smoothing);

		if (Vector3.Distance(center, thumbImage.position) < .1f)
		{
			thumbImage.position = center;
			thumbImageMoving = false; ;
		}
	}
	
	//When the screen is first touched
	public void OnPointerDown (PointerEventData data)
	{
		if (pointerID != -999)
			return;
		
		pointerID = data.pointerId;
		CalculateInput(data);
	}

	//As finger drags around the screen
	public void OnDrag (PointerEventData data)
	{
		if (data.pointerId != pointerID)
			return;

		CalculateInput(data);
	}

	//When the finger leaves the screen
	public void OnPointerUp (PointerEventData data)
	{
		if (data.pointerId != pointerID)
			return;

		direction = Vector3.zero;
		pointerID = -999;
	}
	
	public Vector2 GetDirection ()
	{
		smoothDirection = Vector2.MoveTowards (smoothDirection, direction, smoothing);
		return smoothDirection;
	}

	void CalculateInput(PointerEventData data)
	{
		Vector2 directionRaw = ClampDataAndMoveImage( data.position);

		directionRaw = NormalizeToRange(directionRaw, -1f, 1f);
		
		direction = ApplyAxialDeadZone(directionRaw);
	}

	Vector2 ClampDataAndMoveImage(Vector2 data)
	{
		data.x = Mathf.Clamp(data.x, minX, maxX);
		data.y = Mathf.Clamp(data.y, minY, maxY);

		thumbImage.position = data;
		thumbImageMoving = true;

		return data;
	}

	Vector2 NormalizeToRange(Vector2 data, float newMin, float newMax)
	{
		data.x = (data.x - minX) / (maxX - minX) * (newMax - newMin) + newMin;
		data.y = (data.y - minY) / (maxY - minY) * (newMax - newMin) + newMin;
		return data;
	}

	//Providing two different deadzone algorithms. This game uses Axial but radial is more common
	//More info can be found here: http://www.third-helix.com/2013/04/12/doing-thumbstick-dead-zones-right.html
	Vector2 ApplyAxialDeadZone(Vector2 data)
	{
		float absX = Mathf.Abs(data.x);
		float absY = Mathf.Abs(data.y);

		if (absX < deadZone)
			data.x = 0f;
		else
			data.x = data.x * ((absX - deadZone) / (1 - deadZone));

		if (absY < deadZone)
			data.y = 0f;
		else
			data.y = data.y * ((absY - deadZone) / (1 - deadZone));

		return data;
	}

	Vector2 ApplyRadialDeadZone(Vector2 data)
	{
		float magnitude = data.magnitude;

		if (magnitude < deadZone)
			data = Vector2.zero;
		else
			data = data.normalized * ((magnitude - deadZone) / (1 - deadZone));

		return data;
	}

	void MoveThumbImage(Vector2 position)
	{
		position.x += center.x;
		position.y += center.y;
		thumbImage.position = position;
	}
}