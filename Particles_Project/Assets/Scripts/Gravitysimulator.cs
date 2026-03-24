using UnityEngine;

public class GravitySimulator : MonoBehaviour
{
    private const float G = 39.478f;

    [Header("Control de tiempo")]
    public float simulationSpeed = 1f;  // Multiplicador, el tiempo corre más rápido

    [Range(1, 100)]
    public int stepsPerFrame = 50; 

    private CelestialBody[] bodies;

    [SerializeField] private float simulatedYears = 0f;  // Tiempo total transcurrido en la sim

    void Start()
    {
        bodies = FindObjectsByType<CelestialBody>(FindObjectsSortMode.None);
        ComputeAccelerations();  // Precalcula aceleraciones iniciales (necesario para Verlet)
    }

    void Update()
    {
        float dtFrame = simulationSpeed * Time.deltaTime;
        float dt = dtFrame / stepsPerFrame;  // Subdivide el frame en pasos pequeños

        for (int step = 0; step < stepsPerFrame; step++)
            StepVerlet(dt);  // Avanza la simulación paso a paso

        simulatedYears += dtFrame;

        foreach (var body in bodies)
            body.SyncTransform();  // Al final del frame, actualiza las posiciones visuales
    }

    // Integrador de Verlet: más estable que Euler para órbitas a largo plazo
    // Orden: posición -> aceleración nueva -> velocidad corregida
    private void StepVerlet(float dt)
    {
        // Paso 1: actualiza posición y hace una estimación parcial de velocidad
        for (int i = 0; i < bodies.Length; i++)
        {
            var b = bodies[i];
            b.position = b.position + b.velocity * dt + b.acceleration * (0.5f * dt * dt);
            b.velocity = b.velocity + b.acceleration * (0.5f * dt);  // mitad del paso
        }

        ComputeAccelerations();  // Paso 2: recalcula fuerzas con las nuevas posiciones

        // Paso 3: corrige la velocidad con la aceleración actualizada
        for (int i = 0; i < bodies.Length; i++)
        {
            var b = bodies[i];
            b.velocity = b.velocity + b.acceleration * (0.5f * dt);
        }
    }

    private void ComputeAccelerations()
    {
        for (int i = 0; i < bodies.Length; i++)
            bodies[i].acceleration = Vector3.zero; // Limpia las aceleraciones del paso anterior antes de recalcular

        // Bucle triangular: cada par (i,j) se procesa una sola vez -> O(n2/2)
        for (int i = 0; i < bodies.Length - 1; i++)
        {
            for (int j = i + 1; j < bodies.Length; j++)
            {
                Vector3 r = bodies[j].position - bodies[i].position;
                float distSq = r.sqrMagnitude;

                if (distSq < 1e-10f) continue;  // Evita división por cero si los cuerpos se solapan

                float dist = Mathf.Sqrt(distSq);
                float forceMag = G / distSq;          // F = G x m1 x m2 / d2 (sin las masas aún)
                Vector3 direction = r / dist;         // Vector unitario entre los dos cuerpos

                // Aplica la 3a ley de Newton: acción igual y opuesta
                bodies[i].acceleration += direction * (forceMag * (float)bodies[j].mass);
                bodies[j].acceleration -= direction * (forceMag * (float)bodies[i].mass);
            }
        }
    }
}