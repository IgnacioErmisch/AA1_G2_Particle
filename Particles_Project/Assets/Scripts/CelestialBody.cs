using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [Header("Propiedades físicas")]
    public double mass = 1.0;
    public bool isStatic = false;         // Si true, el cuerpo no se mueve
    public Vector3 initialVelocity = Vector3.zero;

    [Header("Posición inicial")]
    public float initialDistance = 0f;    // Distancia al origen en el eje X al arrancar

    [Header("Rotación axial")]
    public bool hasAxialRotation = true;
    public float rotationSpeed = 30f;
    public Vector3 rotationAxis = Vector3.up;

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;

    // Separa la escala de simulación de la visual (posición real × 5 = posición en pantalla)
    public const float SCALE_FACTOR = 5f;

    void Awake()
    {
        // Coloca el cuerpo en el eje X o en el origen, luego sincroniza visualmente
        position = initialDistance != 0f ? new Vector3(initialDistance, 0f, 0f) : Vector3.zero;
        velocity = initialVelocity;
        acceleration = Vector3.zero;
        SyncTransform();
    }

    void Update()
    {
        // Rotación sobre su propio eje (independiente de la física orbital)
        if (hasAxialRotation)
            transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.World);
    }

    // Llamado por el simulador tras actualizar `position` — refleja la física en Unity
    public void SyncTransform()
    {
        if (isStatic) return;
        transform.position = position * SCALE_FACTOR;
    }
}