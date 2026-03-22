using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    [Header("Propiedades físicas")]
    public double mass = 1.0;
    public bool isStatic = false;
    public Vector3 initialVelocity = Vector3.zero;

    [Header("Posición inicial")]
    public float initialDistance = 0f;

    [Header("Rotación axial")]
    public bool hasAxialRotation = true;
    public float rotationSpeed = 30f;
    public Vector3 rotationAxis = Vector3.up;

    [HideInInspector] public Vector3 position;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 acceleration;

    public const float SCALE_FACTOR = 5f;

    void Awake()
    {
        position = initialDistance != 0f ? new Vector3(initialDistance, 0f, 0f) : Vector3.zero;
        velocity = initialVelocity;
        acceleration = Vector3.zero;
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
        transform.position = position * SCALE_FACTOR;
    }
}