using System;
using UnityEngine.Rendering;

[System.Serializable]
public class AA1_ParticleSystem
{
    Random rnd = new Random();

    [System.Serializable]
    public struct Settings
    {
        public uint objectPoolingParticles;
        public uint poolIndex;
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
        public bool hasCollisioned;

        public float mass;
        public float size;
        public float lifeTime; 

        public Vector3C force;
        
        public Vector3C position;
        public Vector3C velocity;
        public Vector3C aceleration;

        public void InitParticle(SettingsParticle settings)
        {
            this.active = true;

            this.size = settings.size;
            this.mass = settings.mass;

            this.aceleration = Vector3C.zero;
            this.velocity = Vector3C.zero;
        }

        public void InitParticleInCascade(SettingsCascade settingsCascade, SettingsParticle settingsParticle)
        {
            Random rnd = new Random();

            this.InitParticle(settingsParticle);

            this.lifeTime = rnd.Next((int)settingsCascade.minParticlesLifeTime, (int)settingsCascade.maxParticlesLifeTime);

            LineC lineBetweenCascades = LineC.CreateLineFromTwoPoints(settingsCascade.PointA, settingsCascade.PointB);
            // EQ. PARAMETRICA: r(x) = B + x * direction x = 0..1
            this.position = lineBetweenCascades.origin + (lineBetweenCascades.direction * (float)rnd.NextDouble());

            this.force = new Vector3C
                (rnd.Next((int)(settingsCascade.Direction.x * settingsCascade.minImpulse), (int)(settingsCascade.Direction.x * settingsCascade.maxImpulse)),
                rnd.Next((int)(settingsCascade.Direction.y * settingsCascade.minImpulse), (int)(settingsCascade.Direction.y * settingsCascade.maxImpulse)),
                rnd.Next((int)(settingsCascade.Direction.z * settingsCascade.minImpulse), (int)(settingsCascade.Direction.z * settingsCascade.maxImpulse)));
        }

        public void InitParticleInCannon(SettingsCannon settingsCannon, SettingsParticle settingsParticle)
        {
            Random rnd = new Random();

            this.InitParticle(settingsParticle);

            this.lifeTime = rnd.Next((int)settingsCannon.minParticlesLifeTime, (int)settingsCannon.maxParticlesLifeTime);

            this.position = settingsCannon.Start;

            this.force = Vector3C.zero;
        }

        public bool CheckLifeTime()
        {
            if(this.lifeTime < 0.0f)
            {
                this.active = false;
                return true;
            }
            return false;
        }

        public void Euler(Settings settings, float dt)
        {
            // Apply forces
            this.force += settings.gravity;

            // Calculate acceleration, velocity and position
            this.aceleration = this.force / this.mass;
            this.velocity = this.velocity + this.aceleration * dt;
            this.position = this.position + this.velocity * dt;
            this.force = Vector3C.zero;

            // Clean forces
            this.force = Vector3C.zero;
        }

        public void CheckCollisions(SettingsCollision settingsCollision, Settings settings)
        {
            if (CheckPlanesCollision(settingsCollision.planes, settings)) { return; }

            //if (CheckSpheresCollision(settingsCollision.spheres, settings)) { return; }

            //if (CheckCapsulesCollision(settingsCollision.capsules, settings)) { return; }
        }

        public bool CheckPlanesCollision(PlaneC[] planes, Settings settings)
        {
            
            for (int j = 0; j < planes.Length; j++)
            {
                float distance = planes[j].DistanceToPoint(position);

                if (distance <= size + 0.05f && !hasCollisioned)
                {
                    hasCollisioned = true;
                    CollisionPlaneReaction(planes[j], settings);
                    return true;
                }
            }
            hasCollisioned = false;
            return false;
        }

        //public bool CheckSpheresCollision(SphereC[] spheres, Settings settings)
        //{
        //    return true;
        //}

        //public bool CheckCapsulesCollision(CapsuleC[] capsules, Settings settings)
        //{
        //    return true;
        //}

        public void CollisionPlaneReaction(PlaneC plane, Settings settings)
        {
            // 1 calcular Vn (contrario a la normal)
            if (velocity.magnitude == 0.0f)
                return;

            float projection = Vector3C.Dot(plane.normal, velocity) / MathF.Abs(velocity.magnitude);

            Vector3C Vn = velocity.normalized * projection;

            // 2 Calcular rebote
            velocity = (-Vn + (velocity - Vn)) * settings.bounce;
        }
    }

    bool start = true;

    float timerOneSecond; 
    float spawnTime, lastTimeSpawned;

    Particle[] particles = null;

    public Particle[] Update(float dt)  
    {
        if(start)
            Start();

        if (lastTimeSpawned > spawnTime)
            SpawnParticle(dt);

        for (int i = 0; i < particles.Length; ++i)
        {
            if (particles[i].active)
            {
                // 1. Comprobar el tiempo de vida
                if (particles[i].CheckLifeTime()) 
                {
                    // Spawnear fuera de la pantalla
                    particles[i].position = Vector3C.one * 100;
                    continue;
                }
                particles[i].lifeTime -= dt;

                // 2. Calcular si hay colision
                particles[i].CheckCollisions(settingsCollision, settings);

                // 3. Calcular euler
                particles[i].Euler(settings, dt);
            }
        }

        // Si ha pasado 1 segundo cambia el numero de particulas a spawnear por segundo
        if (timerOneSecond > 1.0f)
        {
            timerOneSecond -= 1.0f;
            spawnTime = NewSpawnTime(); 
        }

        // Sumamos los timers
        timerOneSecond += dt;
        lastTimeSpawned += dt;

        return particles;
    }

    public void Start()
    {
        start = false;

        settings.poolIndex = 0;
        particles = new Particle[settings.objectPoolingParticles];

       spawnTime = NewSpawnTime();
    }

    public float NewSpawnTime()
    {
        int particlesPorSecond = RandomParticlesToSpawn(); 

        if(particlesPorSecond != 0)
            return 1.0f / particlesPorSecond;

        return 1.0f;
    }


    public int RandomParticlesToSpawn()
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

    public void SpawnParticle(float dt)
    {
        // 1. Tiene que spawner alguna particula
        lastTimeSpawned -= spawnTime;

        // 2. Comprobar que no esta activa
        if (particles[settings.poolIndex].active) { return; }

        // 3. Si no esta activa la seteamos
        switch (emissionMode.mode)
        {
            case EmissionMode.Emission.CASCADE:
                particles[settings.poolIndex].InitParticleInCascade(settingsCascade, settingsParticle); 
                break;
            case EmissionMode.Emission.CANNON:
                particles[settings.poolIndex].InitParticleInCannon(settingsCannon, settingsParticle);
                break;
            default:
                break;
        }
        settings.poolIndex++;

        // 4. Comprobar que el pool count no sobrepase la array
        if (settings.poolIndex >= settings.objectPoolingParticles) { settings.poolIndex = 0; }
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
