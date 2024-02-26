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
        public uint objectPoolingParticles; 
        public Vector3C gravity;
        public float bounce;
    }
    public Settings settings;

    [System.Serializable]
    public struct SettingsCascade
    {
        public Vector3C PointA;
        public Vector3C PointB;
        public Vector3C Direction;
        public bool randomDirection;

        public float minImpulse;
        public float maxImpulse;

        public float minParticlesPerSecond;
        public float maxParticlesPerSecond;

        public float minParticlesLifeTime;
        public float maxParticlesLifeTime;
    }
    public SettingsCascade settingsCascade;

    [System.Serializable]
    public struct SettingsCannon
    {
        public Vector3C Start;
        public Vector3C Direction;
        public float openingAngle;

        public float minImpulse;
        public float maxImpulse;

        public float minParticlesPerSecond;
        public float maxParticlesPerSecond;

        public float minParticlesLifeTime;
        public float maxParticlesLifeTime;
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

            force = Vector3C.zero; 
            //force = InitForce(settings);  
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
        private static Vector3C InitForce(SettingsCascade sCascade)
        {
            Random rnd = new Random();
            return new Vector3C 
                (rnd.Next((int)(sCascade.Direction.x * sCascade.minImpulse), (int)(sCascade.Direction.x * sCascade.maxImpulse)), 
                rnd.Next(100, 200), 
                rnd.Next(100, 200));    
        }

    }

    bool created = false;
    float timeTospawn = 5.0f; 
    Particle[] particles = new Particle [100]; 

    public Particle[] Update(float dt)  
    {
        if(!created)
        {
            CreateParticles(particles);
            created = true; 
        }
        else
        {
            timeTospawn += dt;
            if(timeTospawn > 1)
            {
                timeTospawn = 0; 
                created = false;
            }
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
        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i] = new Particle(settingsParticle, settingsCascade, settings);
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
