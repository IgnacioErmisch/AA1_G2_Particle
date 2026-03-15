using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
[RequireComponent(typeof(TrailRenderer))]
public class OrbitTrail : MonoBehaviour
{
    [Header("Configuración del rastro")]
    [Tooltip("Duración del rastro en segundos reales.")]
    public float trailTime = 10f;

    [Tooltip("Ancho del rastro en unidades de Unity.")]
    public float trailWidth = 0.1f;

    [Tooltip("Color inicial del rastro (extremo más reciente).")]
    public Color trailColorStart = Color.white;

    [Tooltip("Color final del rastro (extremo más antiguo).")]
    public Color trailColorEnd = new Color(1f, 1f, 1f, 0f);

    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = 0f;

       
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(trailColorStart, 0f),
                new GradientColorKey(trailColorEnd,   1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(trailColorStart.a, 0f),
                new GradientAlphaKey(0f,                1f)
            }
        );
        trail.colorGradient = gradient;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
    }
}