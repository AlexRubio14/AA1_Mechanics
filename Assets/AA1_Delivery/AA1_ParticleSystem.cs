using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

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

    [System.Serializable]
    public struct SettingsParticle
    {
        public float size;
        public float mass;
        public int maxNumParticles;
    }
    public SettingsParticle settingsParticle;

    public struct Particle
    {
        public float mass;
        public float size;

        public Vector3C force;
        
        public Vector3C position;
        public Vector3C velocity;
        public Vector3C aceleration;

        public Particle(SettingsParticle settingsParticle, SettingsCascade settingsCascade)
        {

            size = settingsParticle.size;   

            force = InitForce();  
            mass = settingsParticle.mass;   
            velocity = Vector3C.zero;   
            aceleration = Vector3C.zero;    

            position = RandomPosCascade(settingsCascade.PointA, settingsCascade.PointB);
        }

        private static Vector3C RandomPosCascade(Vector3C pointA, Vector3C pointB)
        {
            Random rnd = new Random();
            
            LineC lineBetweenCascades = LineC.CreateLineFromTwoPoints(pointA, pointB);

            // EQ. PARAMETRICA: r(x) = B + x * direction x = 0..1
            return lineBetweenCascades.origin + (lineBetweenCascades.direction * (float)rnd.NextDouble());
        }
        private static Vector3C InitForce()
        {
            Random rnd = new Random();
            return new Vector3C (rnd.Next(10,20), rnd.Next(5,20), rnd.Next(10, 20));    
        }

    }

    bool created = false;   
    Particle[] particles = new Particle[100]; 
    public Particle[] Update(float dt)  
    {
        if(!created)
        {
            CreateParticles(particles);
            created = true; 
        }

        for (int i = 0; i < particles.Length; ++i)
        {
            // Apply forces
            particles[i].force += settings.gravity;

            // Calculate acceleration, velocity and position
            particles[i].aceleration = particles[i].force / particles[i].mass;
            particles[i].velocity = particles[i].velocity + particles[i].aceleration * dt;
            particles[i].position = particles[i].position + particles[i].velocity * dt;
        }
        return particles;
    }

    public void CreateParticles(Particle[] particles)
    {
        for(int i = 0; i < particles.Length; ++i)
        {
            particles[i] = new Particle(settingsParticle, settingsCascade);
        }
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
