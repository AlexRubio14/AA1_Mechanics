using System;
using System.Drawing;

[System.Serializable]
public class AA1_ParticleSystem
{
   

    [System.Serializable]
    public struct Settings
    {
        public Vector3C gravity;
        public float bounce;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCascade
    {
        public Vector3C PointA;
        public Vector3C PointB;
        public float particlesPerSecond;
    }
    public SettingsCascade settingsCascade;

    [System.Serializable]
    public struct SettingsCannon
    {
        public Vector3C Start;
        public Vector3C Direction;
        public float angle;
        public float particlesPerSecond;
    }
    public SettingsCannon settingsCannon;

    [System.Serializable]
    public struct SettingsCollision
    {
        public PlaneC[] planes;
        public SphereC[] spheres;
        public CapsuleC[] capsules;
    }
    public SettingsCollision settingsCollision;

    public struct Particle
    {
        public float mass;
        public float size;

        public Vector3C force;
        
        public Vector3C position;
        public Vector3C velocity;
        public Vector3C aceleration;

        public Particle(float _size, float _mass)
        {
            Random rnd = new Random();

            position = new Vector3C((float)rnd.NextDouble() - 0.5f, (float)rnd.NextDouble() - 0.5f, (float)rnd.NextDouble() - 0.5f);
            size = _size;

            force = Vector3C.zero;  
            mass = _mass;    
            velocity = Vector3C.zero;   
            aceleration = Vector3C.zero;    
        }

    }
    [System.Serializable]
    public struct SettingsParticle
    {
        public float size;
        public float mass;
    }
    public SettingsParticle settingsParticle;


    Particle[] particles = new Particle[100];

    
    public Particle[] Update(float dt)
    {
        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].size = 0.1f;
            particles[i].force = new Vector3C(0, -9.8f, 0);
            particles[i].mass = 1.0f;
            particles[i].aceleration = particles[i].force / particles[i].mass;
            particles[i].velocity = particles[i].velocity + particles[i].aceleration * dt;
            particles[i].position = particles[i].position + particles[i].velocity * dt;

            //particles[i].position = ;
        }
        return particles;
    }

    public void Debug()
    {
        foreach (var item in settingsCollision.planes)
        {
            item.Print(Vector3C.red);
        }
        foreach (var item in settingsCollision.capsules)
        {
            item.Print(Vector3C.green);
        }
        foreach (var item in settingsCollision.spheres)
        {
            item.Print(Vector3C.blue);
        }
    }
}
