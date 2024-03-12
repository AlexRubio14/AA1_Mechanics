using System;

[System.Serializable]
public struct SphereC
{
    #region FIELDS
    public Vector3C position;
    public float radius;
    #endregion

    #region PROPIERTIES
    public SphereC unitary
    {
        get
        {
            radius = 1.0f;
            return new SphereC(position, radius);
        }
    }
    #endregion

    #region CONSTRUCTORS
    public SphereC(Vector3C position, float radius)
    {
        this.position = position; this.radius = radius;
    }
    #endregion

    #region METHODS

    public bool IsInside(Vector3C point)    
    {
        Vector3C distance = Vector3C.CreateVector3(point, position);

        if (distance.magnitude <= radius)
            return true;
        
        return false;
    }

    public Vector3C NearestPoint(Vector3C point)
    {
        Vector3C nearestPoint = Vector3C.CreateVector3(position, point).normalized;

        nearestPoint = nearestPoint * radius;

        return nearestPoint;
    }

    public Vector3C IntersectionWithLine(LineC line)
    {
        Vector3C L = line.origin - position;

        float A = Vector3C.Dot(line.direction, line.direction);
        float B = Vector3C.Dot((line.direction * 2), L);
        float C = Vector3C.Dot(L, L) - (radius * radius); 

        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
            return Vector3C.zero;

        discriminant = (float)MathF.Sqrt(discriminant);

        float t1 = (-B + discriminant) / (2 * A);
        float t2 = (-B - discriminant) / (2 * A);

        Vector3C intersectionPoint1 = line.origin + line.direction * t1;
        Vector3C intersectionPoint2 = line.origin + line.direction * t2;

        if(discriminant == 0)
        {
            return intersectionPoint1;
        }
        else
        {
            if(intersectionPoint1.magnitude <= intersectionPoint2.magnitude)
                return intersectionPoint1;
            else 
                return intersectionPoint2;
        }
    }


    public override bool Equals(object obj)
    {
        if (obj is SphereC)
        {
            SphereC other = (SphereC)obj;
            return other.position == this.position && other.radius == radius;
        }
        return false;
    }
    #endregion

    #region FUNCTIONS
    #endregion

}