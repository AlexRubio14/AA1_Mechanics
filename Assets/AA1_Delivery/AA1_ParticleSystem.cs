using System;
using static AA1_ParticleSystem;

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

        public Vector3C lastPos;
        public Vector3C position;
        public Vector3C velocity;
        public Vector3C aceleration;

        public void InitParticle(SettingsParticle settings)
        {
            active = true;

            size = settings.size;
            mass = settings.mass;

            aceleration = Vector3C.zero;
            velocity = Vector3C.zero;
        }

        public void InitParticleInCascade(SettingsCascade settingsCascade, SettingsParticle settingsParticle)
        {
            Random rnd = new Random();

            InitParticle(settingsParticle);

            lifeTime = rnd.Next((int)settingsCascade.minParticlesLifeTime, (int)settingsCascade.maxParticlesLifeTime);

            LineC lineBetweenCascades = LineC.CreateLineFromTwoPoints(settingsCascade.PointA, settingsCascade.PointB);
            // EQ. PARAMETRICA: r(x) = B + x * direction x = 0..1
            position = lineBetweenCascades.origin + (lineBetweenCascades.direction * (float)rnd.NextDouble());

            Vector3C cascadeDirection = settingsCascade.Direction.normalized;

            force = new Vector3C
                (rnd.Next((int)(cascadeDirection.x * settingsCascade.minImpulse), (int)(cascadeDirection.x * settingsCascade.maxImpulse)),
                rnd.Next((int)(cascadeDirection.y * settingsCascade.minImpulse), (int)(cascadeDirection.y * settingsCascade.maxImpulse)),
                rnd.Next((int)(cascadeDirection.z * settingsCascade.minImpulse), (int)(cascadeDirection.z * settingsCascade.maxImpulse)));
        }

        public void InitParticleInCannon(SettingsCannon settingsCannon, SettingsParticle settingsParticle)
        {
            Random rnd = new Random();

            InitParticle(settingsParticle);

            lifeTime = rnd.Next((int)settingsCannon.minParticlesLifeTime, (int)settingsCannon.maxParticlesLifeTime);

            position = settingsCannon.Start;

            Vector3C direction;
            float dot; 
            do
            {
                if(settingsCannon.openingAngle <= 0)
                {
                    settingsCannon.openingAngle = 1;
                }
                else if(settingsCannon.openingAngle > 360)
                {
                    settingsCannon.openingAngle %= 360;
                }

                direction = new Vector3C(RandomRangeFloats(-1, 1), RandomRangeFloats(-1, 1), RandomRangeFloats(-1, 1)).normalized;
                dot = Vector3C.Dot(settingsCannon.Direction.normalized, direction);

            } while (dot < (1 - settingsCannon.openingAngle / 360));

            force = direction * RandomRangeFloats(settingsCannon.minImpulse, settingsCannon.maxImpulse); ;
        }
        private static float RandomRangeFloats(float min, float max)
        {
            Random rnd = new Random();
            return (float)rnd.NextDouble() * (max - min) + min;
        }

        public bool CheckLifeTime()
        {
            if(lifeTime < 0.0f)
            {
                active = false;
                return true;
            }
            return false;
        }

        public void Euler(Settings settings, float dt)
        {
            lastPos = position;

            // Apply forces
            force += settings.gravity;

            // Calculate acceleration, velocity and position
            aceleration = force / mass;
            velocity = velocity + aceleration * dt;
            position = position + velocity * dt;

            // Clean forces
            force = Vector3C.zero;
        }

        public void CheckCollisions(SettingsCollision settingsCollision, Settings settings)
        {
            if (CheckPlanes(settingsCollision.planes, settings)) { return; }

            if (CheckSpheres(settingsCollision.spheres, settings)) { return; }

            if (CheckCapsules(settingsCollision.capsules, settings)) { return; }
        }

        public bool CheckPlanes(PlaneC[] planes, Settings settings)
        {
            foreach(PlaneC plane in planes)
            {
                if(CollisionPlane(plane, settings))
                    return true;
            }
            return false;
        }

        public bool CollisionPlane(PlaneC plane, Settings settings)
        {
            // 1. Distance
            double distance = plane.DistanceToPoint(position);

            if (distance < 0.0f)
            {
                // 2. Recolocamos la particula 
                position = plane.IntersectionWithLine(new LineC(lastPos, position));

                // 3. Colision
                CollisionPlaneReaction(plane, settings);
                return true;
            }
            return false;
        }

        public bool CheckSpheres(SphereC[] spheres, Settings settings)
        {
            for(int i = 0; i < spheres.Length; i++)
            {
                // Find the plane
                Vector3C normalizedDistance = (position - spheres[i].position).normalized;
                PlaneC plane = new PlaneC(spheres[i].position + normalizedDistance * spheres[i].radius, normalizedDistance);

                if (CollisionPlane(plane, settings))
                    return true;
            }
            return false;
        }

        public bool CheckCapsules(CapsuleC[] capsules, Settings settings)
        {
            for (int i = 0; i < capsules.Length; i++)
            {
                Vector3C capsuleDistance = capsules[i].positionB - capsules[i].positionA;
                Vector3C particleDistance = position - capsules[i].positionA;

                float projection = Vector3C.Dot(capsuleDistance.normalized, particleDistance);

                Vector3C collisionPoint = capsules[i].positionA + capsuleDistance.normalized * projection;

                Vector3C a = position - collisionPoint;

                PlaneC plane = new PlaneC(collisionPoint + a.normalized * capsules[i].radius, a.normalized);

                if(CollisionPlane(plane, settings))
                    return true;
            }
            return false;
        }

        public void CollisionPlaneReaction(PlaneC plane, Settings settings)
        {
            Vector3C Vn = plane.normal.normalized * Vector3C.Dot(velocity, plane.normal);
            velocity = ((velocity - Vn) - Vn) * settings.bounce;
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

                // 2. Calculate new position
                particles[i].Euler(settings, dt);

                // 3. Check colisions
                particles[i].CheckCollisions(settingsCollision, settings);
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
