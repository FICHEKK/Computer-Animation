using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleGenerator : MonoBehaviour
{
    [SerializeField] private Particle particlePrefab;
    [SerializeField] private int particleCount = 100;
    [SerializeField] private int framesPerParticle = 5;
    [SerializeField] private float radius = 5;
    [SerializeField] private Color startColor = Color.red;
    [SerializeField] private Color endColor = Color.gray;

    private readonly List<Particle> _particles = new List<Particle>();

    private void Update()
    {
        if (_particles.Count < particleCount && Time.frameCount % framesPerParticle == 0)
        {
            _particles.Add(Instantiate(particlePrefab).GetComponent<Particle>());
        }

        MoveCircular();
        ReviveDeadParticles();
    }

    private void MoveCircular()
    {
        var t = Time.time;
        transform.position = new Vector3(Mathf.Cos(t) * radius, Mathf.Sin(t) * radius);
    }

    private void ReviveDeadParticles()
    {
        var now = DateTime.Now;

        foreach (var particle in _particles)
        {
            if (now < particle.DeathTime) continue;
            InitializeParticle(particle);
        }
    }

    private void InitializeParticle(Particle particle)
    {
        particle.transform.position = transform.position;

        particle.BirthTime = DateTime.Now;
        particle.DeathTime = DateTime.Now.AddSeconds(1 + Random.value);

        particle.StartColor = startColor;
        particle.EndColor = endColor;

        var randomX = (2 * Random.value - 1) * Random.value * 2;
        var randomY = (2 * Random.value - 1) * Random.value * 2;
        var randomZ = (2 * Random.value - 1) * Random.value * 2;
        particle.Velocity = new Vector3(randomX, randomY, randomZ);
    }
}
