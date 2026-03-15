using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [Header("Propiedades físicas")]
    [Tooltip("Masa en masas solares (M☉). Sol = 1.0")]
    public double mass = 1.0;

    [Tooltip("Si está marcado, este cuerpo NO se mueve (ideal para el Sol).")]
    public bool isStatic = false;

    [Tooltip("Velocidad inicial en UA/año en el plano XZ. Para la Tierra: (0, 0, 6.28)")]
    public Vector3 initialVelocity = Vector3.zero;

    [Header("Posición inicial")]
    [Tooltip("Distancia inicial al Sol en UA (eje X positivo). Sol = 0.")]
    public double initialDistance = 0.0;

    [Header("Rotación axial")]
    [Tooltip("Activa la rotación sobre el propio eje.")]
    public bool hasAxialRotation = true;

    [Tooltip("Velocidad de rotación en grados por segundo real.")]
    public float rotationSpeed = 30f;

    [Tooltip("Eje de rotación local. Tierra/mayoría = (0,1,0). Urano tumbado ≈ (1,0,0).")]
    public Vector3 rotationAxis = Vector3.up;

 
    [HideInInspector] public Vector3d position;
    [HideInInspector] public Vector3d velocity;
    [HideInInspector] public Vector3d acceleration;

    public const float SCALE_FACTOR = 5f;

    void Awake()
    {
        if (initialDistance != 0.0)
            position = new Vector3d(initialDistance, 0.0, 0.0);
        else
        {
            position = Vector3d.zero;
            transform.position = Vector3.zero;
        }

        velocity = new Vector3d(initialVelocity.x, initialVelocity.y, initialVelocity.z);
        acceleration = Vector3d.zero;
        SyncTransform();
    }

    void Update()
    {
        if (hasAxialRotation)
            transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.World);
    }

    public void SyncTransform()
    {
        if (isStatic) return;
        transform.position = new Vector3(
            (float)(position.x * SCALE_FACTOR),
            (float)(position.y * SCALE_FACTOR),
            (float)(position.z * SCALE_FACTOR));
    }
}