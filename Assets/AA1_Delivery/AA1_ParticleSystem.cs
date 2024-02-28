using JetBrains.Annotations;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;

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

    [System.Serializable]
    public struct EmissionMode
    {
        public enum Emission { CASCADE, CANNON };
        public Emission mode;
    }
    public EmissionMode emissionMode;


    public struct Particle
    {

        public float mass;
        public float size;

        public Vector3C force;
        
        public Vector3C position;
        public Vector3C velocity;
        public Vector3C aceleration;

        public Particle(SettingsParticle settingsParticle, SettingsCascade settingsCascade, SettingsCannon settingsCannon, EmissionMode emissionMode)
        {
            size = settingsParticle.size;

            force = Vector3C.zero; 
            mass = settingsParticle.mass;   
            velocity = Vector3C.zero;   
            aceleration = Vector3C.zero;    

            if(emissionMode.mode == EmissionMode.Emission.CASCADE)
            {
                position = RandomPosCascade(settingsCascade.PointA, settingsCascade.PointB);
                force = InitForce(settingsCascade, settingsCannon, emissionMode);  
            }
            else
            {
                position = settingsCannon.Start;
                force = Vector3C.zero;
            }

        }

        private static Vector3C RandomPosCascade(Vector3C pointA, Vector3C pointB)
        {
            Random rnd = new Random();
            
            LineC lineBetweenCascades = LineC.CreateLineFromTwoPoints(pointA, pointB);

            // EQ. PARAMETRICA: r(x) = B + x * direction x = 0..1
            return lineBetweenCascades.origin + (lineBetweenCascades.direction * (float)rnd.NextDouble());
        }
       
        
        private static Vector3C InitForce(SettingsCascade sCascade, SettingsCannon sCannon, EmissionMode emissionMode)
        {
            Random rnd = new Random();

            switch (emissionMode.mode)
            {
                case EmissionMode.Emission.CASCADE:
                    return new Vector3C
                        (rnd.Next((int)(sCascade.Direction.x * sCascade.minImpulse), (int)(sCascade.Direction.x * sCascade.maxImpulse)),
                        rnd.Next((int)(sCascade.Direction.y * sCascade.minImpulse), (int)(sCascade.Direction.y * sCascade.maxImpulse)),
                        rnd.Next((int)(sCascade.Direction.z * sCascade.minImpulse), (int)(sCascade.Direction.z * sCascade.maxImpulse)));

                case EmissionMode.Emission.CANNON:

                    Vector3C direction = new Vector3C
                        (rnd.Next((int)(sCannon.Direction.x * sCannon.minImpulse), (int)(sCannon.Direction.x * sCannon.maxImpulse)),
                        rnd.Next((int)(sCannon.Direction.y * sCannon.minImpulse), (int)(sCannon.Direction.y * sCannon.maxImpulse)),
                        rnd.Next((int)(sCannon.Direction.z * sCannon.minImpulse), (int)(sCannon.Direction.z * sCannon.maxImpulse)));

                    float scalar = Vector3C.Dot(direction, sCannon.Direction);
                    
                    float currentAngle = (float)Math.Acos(scalar) / (direction.magnitude * sCannon.Direction.magnitude);

                    while (currentAngle >= sCannon.openingAngle)
                    {

                    }

                    return direction;
                default:
                    return Vector3C.zero;
            }

            
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
            particles[i].force = Vector3C.zero;
        }
        return particles;
    }

    public void CreateParticles(Particle[] particles)
    {
        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i] = new Particle(settingsParticle, settingsCascade, settingsCannon, emissionMode);
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
