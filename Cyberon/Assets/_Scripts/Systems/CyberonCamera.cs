using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CyberonCamera : SSystem<CyberonCamera>
{
    [SerializeField] GameObject FakeSkybox = null;

    private bool isCameraFreeRoaming = false;  //TODO: Switch to state machine
    public bool IsCameraFreeRoaming { get => isCameraFreeRoaming; }

    // Targeting
    // --------------------------------------------
    [SerializeField] private GameObject currentTarget = null;
    private Vector3 targetPosition = default;


    // Focusing
    // --------------------------------------------
    [SerializeField, Range(0.5f, 1.0f)] private float focusCenteringSpeed = 0.943f;
    private Vector3 focusPosition;
    private bool isCameraFocused = false;
    public bool IsCameraFocused { get => isCameraFocused; }
    [SerializeField, Range(0.1f, 0.5f)] private float distanceDeltaToFocus = 0.3f;

    // Zoom Data
    // --------------------------------------------
    [SerializeField, Range(5.0f, 20.0f)] private float zoomDistance = 10.0f;
    [SerializeField] private float maxZoomDistance = 20.0f;
    [SerializeField] private float minZoomDistance = 5.0f;

    // Orbiting
    // --------------------------------------------
    private Vector3 orbitAngle = new Vector3(45.0f, 45.0f, 0.0f);
    private Vector3 orbitRotAngle = new Vector3(0.0f, 90.0f, 0.0f);
    private float zoomSize = 0.5f;
    private bool canRotate = true;
    private bool canZoom = false;

    // Free Roam Data
    // --------------------------------------------
    [SerializeField, Range(1, 350)] public int panSpeed = 100;
    [SerializeField, Range(0.01f, 0.1f)] private float edgeSizePercent = 0.03f;
    [SerializeField] private float playRange = 20.0f;


    [Header("Fake Skybox")]
    [SerializeField] Material skyboxMat = null;
    [SerializeField] [ColorUsage(true, true)] Color stealthColor = Color.white;
    [SerializeField] [ColorUsage(true, true)] Color huntColor = Color.white;
    

    public void ForceFastMove(Transform targetTransform)
    {
        Vector3 lookDirection = transform.forward;
        Vector3 lookPosition = targetTransform.position - lookDirection * zoomDistance;
        transform.position = lookPosition;
    }

    public void ChangeToHunt()
    {
        skyboxMat.SetColor("_SkyBoxColor", huntColor);

    }

    public void ChangeToStealth()
    {
        skyboxMat.SetColor("_SkyBoxColor", stealthColor);
    }
    public void Init()
    {
        transform.rotation = Quaternion.Euler(45.0f, 45.0f, 0.0f);
        FakeSkybox.transform.localRotation = transform.rotation;
        skyboxMat.SetColor("_Color", stealthColor);

        Debug.Log("Camera initialized: ");
    }

    void Update()
    {
        HandleInput();
    }

    // Target's position will change in Update, so camera should move focus here
    void FixedUpdate()
    {
        panSpeed = GameStateManager.Instance.CameraSensitivity;
        UpdateFocusPoint();

        zoomDistance -= Input.GetAxis("ZoomIn")*zoomSize;
        zoomDistance += Input.GetAxis("ZoomOut")*zoomSize;
    }

    // Focus objects can change in Update, so camera moves here
    void LateUpdate()
    {
       
        UpdateCameraPosition();
        //UpdateCameraRotation();
    }

    public void SetTarget(GameObject targetObject)
    {
        // Debug.Log("New Camera Target: " + targetObject);
        currentTarget = targetObject;
    }


    public void FreeRoam()
    {
        Debug.Log("Camera mode: Free roaming!");
        Cursor.visible = true;
        isCameraFreeRoaming = true;
    }

    public void Follow()
    {
        Cursor.visible = false;
        isCameraFreeRoaming = false;
        isCameraFocused = false;
        //  Debug.Log("Camera mode: Following !");

    }

    // 
    private void UpdateFocusPoint()
    {
        if (!isCameraFreeRoaming && currentTarget != null) targetPosition = currentTarget.transform.position;
        float distance = Vector3.Distance(targetPosition, focusPosition);
        // Check if we need to centre the camera
        if (distance < distanceDeltaToFocus)
            isCameraFocused = true;

        if (distance > 0.01f)
        {
            // Time.deltaTime switch to unscaledDeltaTime to cover when game is paused
            focusPosition = Vector3.Lerp(targetPosition, focusPosition, (float)System.Math.Pow((1.0f - focusCenteringSpeed), Time.deltaTime));
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 lookDirection = transform.forward;
        Vector3 lookPosition = focusPosition - lookDirection * zoomDistance;
        transform.position = lookPosition;
    }

    private void UpdateCameraRotation()
    {
        transform.rotation = Quaternion.Euler(orbitAngle);
    }

    private void HandleInput()
    {
        float edgeSizeX = Screen.width * (edgeSizePercent);
        float edgeSizeY = Screen.height * (edgeSizePercent);
        if (isCameraFreeRoaming && !IsMouseOutScreen())
        {
          
            // Position
            // --------------------------------------------
            var mousePos = Input.mousePosition;
            if (mousePos.x - (Screen.width - edgeSizeX) > 0.0f) // overlap check
            {
                MoveFreeRoamFocusX((mousePos.x - (Screen.width - edgeSizeX)), new Vector3(1.0f, 0.0f, 0.0f));
            }
            if (edgeSizeX - mousePos.x > 0.0f) // overlap check
            {
                MoveFreeRoamFocusX((edgeSizeX - mousePos.x), new Vector3(-1.0f, 0.0f, 0.0f));
            }
            if (mousePos.y - (Screen.height - edgeSizeY) > 0.0f) // overlap check
            {
                MoveFreeRoamFocusY((mousePos.y - (Screen.height - edgeSizeY)), new Vector3(0.0f, 0.0f, 1.0f));
            }
            if (edgeSizeY - mousePos.y > 0.0f) // overlap check
            {
                MoveFreeRoamFocusY((edgeSizeY - mousePos.y), new Vector3(0.0f, 0.0f, -1.0f));
            }

            // Playground checks
            if (targetPosition.x > playRange) targetPosition.x = playRange;
            if (targetPosition.x < -playRange) targetPosition.x = -playRange;
            if (targetPosition.z > playRange) targetPosition.z = playRange;
            if (targetPosition.z < -playRange) targetPosition.z = -playRange;

            // Rotation
            if (Input.GetKeyDown(KeyCode.E) && canRotate)
            {
                canRotate = false;
                StartCoroutine(CameraLerpFromToE(1f));
            }
            else if (Input.GetKeyDown(KeyCode.Q) && canRotate)
            {
                canRotate = false;
                StartCoroutine(CameraLerpFromToQ(1f));
            }

            // Zoom
            //if (Input.GetKeyDown(KeyCode.KeypadPlus) && canZoom) /*zoomDistance -= 1.0f;*/ { canZoom = false; StartCoroutine(CameraZoomFromToPlus(0.25f)); }
            //if (Input.GetKeyDown(KeyCode.KeypadMinus) && canZoom) /*zoomDistance += 1.0f;*/ { canZoom = false; StartCoroutine(CameraZoomFromToMinus(0.25f)); }

            // Zoom checks
            if (zoomDistance > maxZoomDistance) zoomDistance = maxZoomDistance;
            if (zoomDistance < minZoomDistance) zoomDistance = minZoomDistance;
        }
    }

    private bool IsMouseOutScreen()
    {
         return (0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); 
    }

    //Smooth Camera Rotation Upon pressing E (Negative Direction)
    private IEnumerator CameraLerpFromToE(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(orbitAngle), Quaternion.Euler(orbitAngle - orbitRotAngle), timer / duration);
            FakeSkybox.transform.localRotation = transform.rotation;
            yield return null;
        }
        orbitAngle -= orbitRotAngle;
        canRotate = true;
    }

    //Smooth Camera Rotation Upon pressing Q (Positive Direction)
    private IEnumerator CameraLerpFromToQ(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(orbitAngle), Quaternion.Euler(orbitAngle + orbitRotAngle), timer / duration);
            FakeSkybox.transform.localRotation = transform.rotation;
            yield return null;
        }
        orbitAngle += orbitRotAngle;
        canRotate = true;
    }

    //Smooth Zoom Upon pressing - (Negative Direction)
    private IEnumerator CameraZoomFromToPlus(float duration)
    {
        float targetDistance = zoomDistance - zoomSize;
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            if (zoomDistance - zoomSize >= minZoomDistance)
                zoomDistance = Mathf.Lerp(zoomDistance, targetDistance, timer / duration);
            yield return null;
        }
        canZoom = true;
    }

    //Smooth Zoom Upon pressing + (Positive Direction)
    private IEnumerator CameraZoomFromToMinus(float duration)
    {
        float targetDistance = zoomDistance + zoomSize;
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
           if (zoomDistance + zoomSize <= maxZoomDistance)
                zoomDistance = Mathf.Lerp(zoomDistance, targetDistance, timer / duration);
            yield return null;
        }
        canZoom = true;
    }

    private void HandleDebugInput()
    {
        // For Debug testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FreeRoam();
        }
    }

    private void MoveFreeRoamFocusX(float edgeOverLap, Vector3 axis)
    {
        float moveSize = (edgeOverLap/Screen.width) * zoomDistance * panSpeed * edgeSizePercent * 0.1f;
        Quaternion direction = Quaternion.Euler(0.0f, orbitAngle.y, 0.0f);
        Vector3 move = direction * (axis * moveSize);

        targetPosition += move;
    }   

    private void MoveFreeRoamFocusY(float edgeOverLap, Vector3 axis)
    {
        float moveSize = (edgeOverLap / Screen.height) * zoomDistance * panSpeed * edgeSizePercent * 0.1f;
        Quaternion direction = Quaternion.Euler(0.0f, orbitAngle.y, 0.0f);
        Vector3 move = direction * (axis * moveSize);

        targetPosition += move;
    }
}
