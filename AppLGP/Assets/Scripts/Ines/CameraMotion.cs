using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // Mouse Scroll
    public GameObject character;
    public bool canRotate = true;
    public bool canZoom = true;
    // public EditorInterface editorManager;
    public Camera mainCamera;
    public Camera previewCamera;
    public Transform cameraFromAbove;
    public Camera sideCamera;

    private bool hovering = true;
    private float initialY;
    private Vector3 originalCameraPos;
    private Vector3 originalCameraRot;

    // FOV
    float minFov = 10f;
    float maxFov = 60f;
    public float yPanRange = 0.6f;

    public Transform bezierControlPoint;

    public float cameraRotation = 0;
    public float cameraRotationVertical = 0;

    public Vector3 cameraRotationVector = new Vector3(1, 0, 0);
    public Vector3 cameraRotationVectorVertical = new Vector3(0, 0, 1);

    private Vector3 originalRotation;
    private Vector3 originalPosition;

    private void Start()
    {
        originalCameraPos = mainCamera.transform.position;
        originalCameraRot = mainCamera.transform.localEulerAngles;
    }

    void Update () {
        if (hovering)
        {
            if (canZoom)
            {
                /*float fov = mainCamera.fieldOfView;
                fov -= Input.GetAxis("Mouse ScrollWheel") * Preferences.singleton.mouseSensitivity * 10;
                fov = Mathf.Clamp(fov, minFov, maxFov);
                
                mainCamera.fieldOfView = fov;*/

                float valueChange = mainCamera.fieldOfView;

                valueChange -= Input.GetAxis("Mouse ScrollWheel") * 10f;
                valueChange = Mathf.Clamp(valueChange, minFov, maxFov);
                Camera.main.fieldOfView = valueChange;
                
                // Input.GetAxis("Mouse ScrollWheel") * 10 / (maxFov - minFov);
                // Debug.Log(valueChange);
                // ZoomSliderDrag(valueChange);
            }
        }

        if (Input.GetMouseButton(0)) OnDrag();
    }

    public void ZoomSliderDrag(float sliderValue)
    {
        mainCamera.fieldOfView = Mathf.Lerp(maxFov, minFov, sliderValue);
    }

    // public void PanSliderDrag(float sliderValue)
    // {
    //     if (panSlider != null)
    //         yPan = Mathf.Lerp(initialY + yPanRange, initialY - yPanRange, sliderValue);
    // }

    public void VerticalRotationSliderDrag(float sliderValue)
    {
        if (cameraFromAbove != null) {
            // mainCamera.transform.position = MathExtensions.BezierQuadratic(originalCameraPos, bezierControlPoint.position, cameraFromAbove.transform.position, sliderValue);
            mainCamera.transform.localEulerAngles = Vector3.Lerp(originalCameraRot, cameraFromAbove.transform.localEulerAngles, sliderValue);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("ENTROUUU");
        hovering = true;
    }

    public bool IsHovering()
    {
        return hovering;
    }

    public void OnDrag()
	{
        // Debug.Log("ENTROUUUU");
        float rotationSpeed = 10f;
		float XaxisRotation = Input.GetAxis("Mouse X")*rotationSpeed;
		// float YaxisRotation = Input.GetAxis("Mouse Y")*rotationSpeed;
		// select the axis by which you want to rotate the GameObject
		character.transform.Rotate(Vector3.down, XaxisRotation);
		// character.transform.RotateAround (Vector3.right, YaxisRotation);
	}

    // public void OnMouseDrag(PointerEventData eventData)
    // {
    //     Debug.Log("ENTROUUU");
    //     float x = -Input.GetAxis("Mouse X") * 10; //default is 0.1
    //     AddCameraRotation(x, 0);

    //     // if (editorManager != null)
    //     //     editorManager.EndPreview();
    // }

    public void AddCameraRotation(float angle, float angleVertical)
    {
        SetCameraRotation(cameraRotation + angle, cameraRotationVertical + angleVertical);
    }

    public void SetCameraRotation(float angle, float angleVertical)
    {
        cameraRotation = angle;
        cameraRotationVertical = angleVertical;
        Vector3 change = angle * cameraRotationVector + angleVertical * cameraRotationVectorVertical;
        character.transform.localEulerAngles = new Vector3(originalRotation.x, originalRotation.y, originalRotation.z) + change;
        //Debug.Log(GetSubjectRotatable().localEulerAngles);
        // RecenterPosition();
    }

    // private void RecenterPosition()
    // {

    //     if (GetSubjectStationary() != null)
    //         GetSubjectStationary().root.position += originalPosition + new Vector3(0, yPan, 0) - GetSubjectStationary().position;
    // }

}
