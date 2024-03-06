using System;

[System.Serializable]
public class AA1_ParticleSystem
{
    [System.Serializable]
    public struct Settings
    {
        public uint objectPoolingParticles;
        public uint poolCount;
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
        public bool active; 

        public float mass;
        public float size;
        public float lifeTime; 

        public Vector3C force;
        
        public Vector3C position;
        public Vector3C velocity;
        public Vector3C aceleration;
    }
    Random rnd = new Random();

    float timer = 0.0f;
    float spawnTime = 0.0f; 
    Particle[] particles = null;

    public Particle[] Update(float dt)  
    {

        if(timer == 0.0f)
        {
            InitPoolingParticles();
        }
        timer += dt;

        CreateParticle(dt); 

        for (int i = 0; i < particles.Length; ++i)
        {
            if (particles[i].active)
            {
                // Apply forces
                particles[i].force += settings.gravity;

                // Calculate acceleration, velocity and position
                particles[i].aceleration = particles[i].force / particles[i].mass;
                particles[i].velocity = particles[i].velocity + particles[i].aceleration * dt;
                particles[i].position = particles[i].position + particles[i].velocity * dt;
                particles[i].force = Vector3C.zero;

                // Clean forces
                particles[i].force = Vector3C.zero;
            }
            else
            {
                // Calculate acceleration, velocity and position
                particles[i].aceleration = Vector3C.zero;
                particles[i].velocity = Vector3C.zero;
                particles[i].position = Vector3C.one * 100;
                particles[i].force = Vector3C.zero;
            }
        }
        return particles;
    }

    public void InitPoolingParticles()
    {
        settings.poolCount = 0;
        particles = new Particle[settings.objectPoolingParticles];
    }

    public int NumParticlesToSpawn()
    {

        switch (emissionMode.mode)
        {
            case EmissionMode.Emission.CASCADE:
                return rnd.Next((int)settingsCascade.minParticlesPerSecond, (int)settingsCascade.maxParticlesPerSecond);
            case EmissionMode.Emission.CANNON:
                return rnd.Next((int)settingsCannon.minParticlesPerSecond, (int)settingsCannon.maxParticlesPerSecond);
            default:
                return 0;
        }
    }

    public void CreateParticle(float dt)
    {
        spawnTime += NumParticlesToSpawn() * dt;
        if (spawnTime < 1.0f)
        {
            return;
        }
        spawnTime -= 1.0f;

        settings.poolCount++;
        if(settings.poolCount >= settings.objectPoolingParticles) 
        {
            settings.poolCount = 0;
        }

        if (particles[settings.poolCount].active) { return; }

        particles[settings.poolCount].active = true;

        particles[settings.poolCount].size = settingsParticle.size;
        particles[settings.poolCount].mass = settingsParticle.mass;

        switch (emissionMode.mode)
        {
            case EmissionMode.Emission.CASCADE:

                particles[settings.poolCount].lifeTime = rnd.Next((int)settingsCascade.minParticlesLifeTime, (int)settingsCascade.maxParticlesLifeTime);

                LineC lineBetweenCascades = LineC.CreateLineFromTwoPoints(settingsCascade.PointA, settingsCascade.PointB);
                // EQ. PARAMETRICA: r(x) = B + x * direction x = 0..1
                particles[settings.poolCount].position = lineBetweenCascades.origin + (lineBetweenCascades.direction * (float)rnd.NextDouble());

                particles[settings.poolCount].force = new Vector3C
                    (rnd.Next((int)(settingsCascade.Direction.x * settingsCascade.minImpulse), (int)(settingsCascade.Direction.x * settingsCascade.maxImpulse)),
                    rnd.Next((int)(settingsCascade.Direction.y * settingsCascade.minImpulse), (int)(settingsCascade.Direction.y * settingsCascade.maxImpulse)),
                    rnd.Next((int)(settingsCascade.Direction.z * settingsCascade.minImpulse), (int)(settingsCascade.Direction.z * settingsCascade.maxImpulse)));
                break;
            case EmissionMode.Emission.CANNON:

                particles[settings.poolCount].lifeTime = rnd.Next((int)settingsCannon.minParticlesLifeTime, (int)settingsCannon.maxParticlesLifeTime);

                particles[settings.poolCount].position = settingsCannon.Start;

                particles[settings.poolCount].force = Vector3C.zero;

                break;
            default:
                break;
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
