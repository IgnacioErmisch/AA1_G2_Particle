using UnityEngine;

// CONTROLES:
//   Clic derecho mantenido  → activa el modo look (rota con el ratón)
//   W / S                   → avanzar / retroceder
//   A / D                   → strafe izquierda / derecha
//   Q / E                   → bajar / subir
//   Shift                   → multiplicador de velocidad (x3)
//   Rueda del ratón         → ajuste rápido de velocidad base
//   F                       → focus: vuela suavemente hacia el Sol (o el Target)
public class SolarSystemCamera : MonoBehaviour
{
    [Header("Velocidad de movimiento")]
    public float moveSpeed = 20f;
    public float shiftMultiplier = 3f;

    [Header("Sensibilidad del ratón")]
    public float mouseSensitivity = 2f;

    [Header("Suavizado")]
    [Range(0f, 1f)]
    public float moveSmoothness = 0.12f;

    [Header("Focus (tecla F)")]
    public Transform focusTarget;
    public float focusDistance = 30f;
    public float focusSpeed = 5f;

  
    private float yaw;
    private float pitch;
    private Vector3 smoothVelocity;
    private Vector3 targetVelocity;
    private bool focusing;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.F))
            focusing = !focusing;

        if (focusing)
        {
            Vector3 targetPos = focusTarget != null ? focusTarget.position : Vector3.zero;
            Vector3 desiredPos = targetPos - transform.forward * focusDistance;
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * focusSpeed);
            
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                focusing = false;
           
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -89f, 89f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f);

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),(Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f),Input.GetAxisRaw("Vertical"));

        targetVelocity = transform.TransformDirection(input.normalized) * speed;
        smoothVelocity = Vector3.Lerp(smoothVelocity, targetVelocity, 1f - moveSmoothness);
        transform.position += smoothVelocity * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            moveSpeed = Mathf.Clamp(moveSpeed + scroll * 20f, 1f, 500f);
        }
    }
}