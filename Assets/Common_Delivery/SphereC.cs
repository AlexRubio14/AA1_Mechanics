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

    public Vector3C NearestPoint()
    {
        Vector3C nearestPoint = Vector3C.zero;



        return nearestPoint;
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