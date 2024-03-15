
[System.Serializable]
public struct PlaneC
{
    #region FIELDS
    public Vector3C position;
    public Vector3C normal;
    #endregion

    #region PROPIERTIES
    public static PlaneC right {  get { return new PlaneC(Vector3C.right, Vector3C.zero); } }
    public static PlaneC up {  get { return new PlaneC(Vector3C.up, Vector3C.zero); } }
    public static PlaneC forward {  get { return new PlaneC(Vector3C.forward, Vector3C.zero); } }
    #endregion

    #region CONSTRUCTORS
    public PlaneC(Vector3C position, Vector3C normal)
    {
        this.position = position;
        this.normal = normal;
    }
    public PlaneC(Vector3C pointA, Vector3C pointB, Vector3C pointC)
    {
        position = pointA;
        normal = Vector3C.Cross(pointB - pointA, pointC - pointA);
    }
    
    public PlaneC(Vector3C n, float D)
    {
        float x, y, z;
        x = -D / -n.x;
        y = -D / -n.y;
        z = -D / -n.z;

        this.position = new Vector3C(x, y, z);
        this.normal = n;
    }
    #endregion

    #region METHODS
    //ToEquation(Ax + By + Cz + D = 0)
    public (float A, float B, float C, float D) ToEquation()
    {
        float d = (- (normal.x * position.x) - (normal.y * position.y) - (normal.z * position.z));

        return (normal.x, normal.y, normal.z, d);
    }

    public override bool Equals(object obj)
    {
        if (obj is PlaneC)
        {
            PlaneC other = (PlaneC)obj;
            return other.normal == this.normal;
        }
        return false;
    }

    public float DistanceToPoint(Vector3C point)
    {
        return Vector3C.Dot(normal, (point - position));
    }
    public Vector3C IntersectionWithLine(LineC line)
    {
        float distance = Vector3C.Dot(normal, position - line.origin) / Vector3C.Dot(normal, line.direction);
        return line.origin + line.direction * distance;
    }
    #endregion

    #region FUNCTIONS


    #endregion

}