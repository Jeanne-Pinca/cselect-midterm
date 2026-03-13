using System.Collections.Generic;
using UnityEngine;

public class WaterObj : MonoBehaviour
{
    [Header("Goal Trigger")]
    [SerializeField] private string goalObjectName = "Goal";

    private ParticleSystem ps;
    [SerializeField] private List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();
    [SerializeField] private LevelAndProgressUI levelAndProgressUI;

    [Header("Stream Detection")]
    [Tooltip("Minimum particles entering per second to count as a focused stream.")]
    [SerializeField] private int minStreamRate = 5;
    [Tooltip("How many units of fill the stream awards per second at minimum rate. Scales with stream intensity.")]
    [SerializeField] private float fillUnitsPerSecond = 8f;

    // Rolling 1-second window of particle entry timestamps.
    private readonly Queue<float> entryTimes = new Queue<float>();
    private float fillAccumulator;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
            Debug.LogError("WaterObj requires a ParticleSystem on the same GameObject.", this);

        if (levelAndProgressUI == null)
            levelAndProgressUI = FindObjectOfType<LevelAndProgressUI>();

        if (levelAndProgressUI == null)
            Debug.LogWarning("LevelAndProgressUI reference is missing on WaterObj. Assign it in the Inspector.", this);

        ConfigureGoalTrigger();
    }

    private void Update()
    {
        float now = Time.time;

        // Prune entries older than 1 second.
        while (entryTimes.Count > 0 && now - entryTimes.Peek() > 1f)
            entryTimes.Dequeue();

        int currentRate = entryTimes.Count; // particles entered in the last second

        if (currentRate < minStreamRate || levelAndProgressUI == null)
            return;

        // Scale fill rate with stream intensity above the minimum.
        float rateMultiplier = (float)currentRate / minStreamRate;
        fillAccumulator += fillUnitsPerSecond * rateMultiplier * Time.deltaTime;

        int toAward = Mathf.FloorToInt(fillAccumulator);
        if (toAward > 0)
        {
            fillAccumulator -= toAward;
            levelAndProgressUI.AddParticles(toAward);
        }
    }

    private void OnParticleTrigger()
    {
        if (ps == null) return;

        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

        float now = Time.time;
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enterList[i];
            p.remainingLifetime = 0f;
            enterList[i] = p;
            entryTimes.Enqueue(now);
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
    }

    private void ConfigureGoalTrigger()
    {
        if (ps == null)
            return;

        if (string.IsNullOrWhiteSpace(goalObjectName))
            return;

        GameObject goalObject = GameObject.Find(goalObjectName);
        if (goalObject == null)
        {
            Debug.LogWarning($"Could not find goal object named '{goalObjectName}' for particle trigger setup.", this);
            return;
        }

        Component goalTriggerCollider = goalObject.GetComponent<Collider>();
        if (goalTriggerCollider == null)
            goalTriggerCollider = goalObject.GetComponent<Collider2D>();

        if (goalTriggerCollider == null)
        {
            Debug.LogWarning("Goal object needs a Collider or Collider2D for particle trigger detection.", goalObject);
            return;
        }

        var triggerModule = ps.trigger;
        triggerModule.enabled = true;

        int emptyIndex = -1;
        for (int i = 0; i < triggerModule.maxColliderCount; i++)
        {
            Component existingCollider = triggerModule.GetCollider(i);
            if (existingCollider == goalTriggerCollider)
                return;

            if (existingCollider == null && emptyIndex < 0)
                emptyIndex = i;
        }

        if (emptyIndex >= 0)
        {
            triggerModule.SetCollider(emptyIndex, goalTriggerCollider);
            return;
        }

        triggerModule.SetCollider(0, goalTriggerCollider);
    }
}
