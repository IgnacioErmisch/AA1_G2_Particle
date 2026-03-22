using UnityEngine;

public class GravitySimulator : MonoBehaviour
{
    private const float G = 39.478f;

    [Header("Control de tiempo")]
    public float simulationSpeed = 1f;

    [Range(1, 100)]
    public int stepsPerFrame = 10;

    private CelestialBody[] bodies;

    [Header("Info (solo lectura)")]
    [SerializeField] private float simulatedYears = 0f;

    void Start()
    {
        bodies = FindObjectsByType<CelestialBody>(FindObjectsSortMode.None);
        ComputeAccelerations();
    }

    void Update()
    {
        float dtFrame = simulationSpeed * Time.deltaTime;
        float dt = dtFrame / stepsPerFrame;

        for (int step = 0; step < stepsPerFrame; step++)
            StepVerlet(dt);

        simulatedYears += dtFrame;

        foreach (var body in bodies)
            body.SyncTransform();
    }

    private void StepVerlet(float dt)
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            var b = bodies[i];
            b.position = b.position + b.velocity * dt + b.acceleration * (0.5f * dt * dt);
            b.velocity = b.velocity + b.acceleration * (0.5f * dt);
        }

        ComputeAccelerations();

        for (int i = 0; i < bodies.Length; i++)
        {
            var b = bodies[i];
            b.velocity = b.velocity + b.acceleration * (0.5f * dt);
        }
    }

    private void ComputeAccelerations()
    {
        for (int i = 0; i < bodies.Length; i++)
            bodies[i].acceleration = Vector3.zero;

        for (int i = 0; i < bodies.Length - 1; i++)
        {
            for (int j = i + 1; j < bodies.Length; j++)
            {
                Vector3 r = bodies[j].position - bodies[i].position;
                float distSq = r.sqrMagnitude;

                if (distSq < 1e-10f) continue;

                float dist = Mathf.Sqrt(distSq);
                float forceMag = G / distSq;
                Vector3 direction = r / dist;

                bodies[i].acceleration += direction * (forceMag * (float)bodies[j].mass);
                bodies[j].acceleration -= direction * (forceMag * (float)bodies[i].mass);
            }
        }
    }
}