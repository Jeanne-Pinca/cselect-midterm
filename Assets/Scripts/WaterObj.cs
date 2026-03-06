using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterObj : MonoBehaviour
{
    ParticleSystem ps;
    [SerializeField] List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnParticleTrigger() 
    {
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

        // iterate
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enterList[i];
            p.remainingLifetime = 0f;
            enterList[i] = p;

            Debug.Log("Fill!");
        }

        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
	}
}
