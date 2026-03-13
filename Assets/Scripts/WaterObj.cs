using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterObj : MonoBehaviour
{
    private ParticleSystem ps;
    [SerializeField] private List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();
    [SerializeField] private LevelAndProgressUI levelAndProgressUI;
    [SerializeField] private int fillPerParticle = 1;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            Debug.LogError("WaterObj requires a ParticleSystem on the same GameObject.", this);
        }

        if (levelAndProgressUI == null)
        {
            levelAndProgressUI = FindObjectOfType<LevelAndProgressUI>();
        }

        if (levelAndProgressUI == null)
        {
            Debug.LogWarning("LevelAndProgressUI reference is missing on WaterObj. Assign it in the Inspector.", this);
        }
    }

    private void OnParticleTrigger()
    {
        if (ps == null)
        {
            return;
        }

        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

        // iterate
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enterList[i];
            p.remainingLifetime = 0f;
            enterList[i] = p;

            Debug.Log("Fill!");
        }

        if (numEnter > 0 && levelAndProgressUI != null)
        {
            levelAndProgressUI.AddParticles(numEnter * fillPerParticle);
        }

        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
	}
}
