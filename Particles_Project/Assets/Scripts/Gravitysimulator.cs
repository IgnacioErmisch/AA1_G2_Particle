using UnityEngine;


// UNIDADES ASTRONÓMICAS:
//   Distancia : UA  (1 UA ? 149.6 M km)
//   Tiempo    : año terrestre (365.25 días)
//   Masa      : M? (masa solar)
//   G         : 39.478 UA³/(año²·M?)   ?  de G·M? = 4?² UA³/año²

public class GravitySimulator : MonoBehaviour
{
   
    private const double G = 39.478;

    [Header("Control de tiempo")]
    [Tooltip("Años simulados por segundo real.")]
    public float simulationSpeed = 1f;

    [Tooltip("Pasos de integración por frame (mayor = más preciso pero más costoso).")]
    [Range(1, 100)]
    public int stepsPerFrame = 10;

    private CelestialBody[] bodies;

    [Header("Info (solo lectura)")]
    [SerializeField] private float simulatedYears = 0f;
    [SerializeField] private float earthDistanceUA = 0f;

    void Start()
    {
        
        bodies = FindObjectsByType<CelestialBody>(FindObjectsSortMode.None);

        if (bodies.Length == 0)
        {
            Debug.LogError("[GravitySimulator] No se encontró ningún CelestialBody en la escena.");
            enabled = false;
            return;
        }

        // Calcula la aceleración inicial (necesaria para el primer paso de Verlet)
        ComputeAccelerations();

      
    }

    
    void Update()
    {
        // dt total de este frame en años simulados
        double dtFrame = simulationSpeed * Time.deltaTime;
        // Sub-paso
        double dt = dtFrame / stepsPerFrame;

        for (int step = 0; step < stepsPerFrame; step++)
        {
            StepVerlet(dt);
        }

        simulatedYears += (float)dtFrame;

        // Sincroniza posiciones con los GameObjects
        foreach (var body in bodies)
            body.SyncTransform();

        // Debug: distancia Tierra-Sol (busca el objeto llamado "Earth")
        UpdateDebugInfo();
    }

   
    private void StepVerlet(double dt)
    {
        // 1) Actualiza posiciones y mitad de velocidad
        for (int i = 0; i < bodies.Length; i++)
        {
            var b = bodies[i];
            // x(t+dt) = x(t) + v(t)·dt + ½·a(t)·dt²
            b.position = b.position + b.velocity * dt + b.acceleration * (0.5 * dt * dt);
            // v_half = v(t) + ½·a(t)·dt  (guardamos en velocity temporalmente)
            b.velocity = b.velocity + b.acceleration * (0.5 * dt);
        }

        // 2) Recalcula aceleraciones con las nuevas posiciones
        ComputeAccelerations();

        // 3) Completa la actualización de velocidades
        for (int i = 0; i < bodies.Length; i++)
        {
            var b = bodies[i];
            // v(t+dt) = v_half + ½·a(t+dt)·dt
            b.velocity = b.velocity + b.acceleration * (0.5 * dt);
        }
    }

  
    private void ComputeAccelerations()
    {
        // Resetea aceleraciones
        for (int i = 0; i < bodies.Length; i++)
            bodies[i].acceleration = Vector3d.zero;

        // Suma las contribuciones de cada par (i, j) una sola vez ? O(n²/2)
        for (int i = 0; i < bodies.Length - 1; i++)
        {
            for (int j = i + 1; j < bodies.Length; j++)
            {
                Vector3d r = bodies[j].position - bodies[i].position; // vector i?j
                double distSq = r.SqrMagnitude;

                // Evita singularidades (cuerpos superpuestos)
                if (distSq < 1e-10) continue;

                double dist = System.Math.Sqrt(distSq);
                double forceMag = G / distSq; // fuerza por unidad de masa (ambos lados)

                Vector3d direction = r / dist; // r?

                // a_i += G·m_j / r² · r?
                bodies[i].acceleration = bodies[i].acceleration + direction * (forceMag * bodies[j].mass);
                // a_j -= G·m_i / r² · r?  (3ª ley de Newton)
                bodies[j].acceleration = bodies[j].acceleration - direction * (forceMag * bodies[i].mass);
            }
        }
    }

    private void UpdateDebugInfo()
    {
        foreach (var b in bodies)
        {
            if (b.gameObject.name == "Earth")
            {
                earthDistanceUA = (float)b.position.Magnitude;
                break;
            }
        }
    }
}