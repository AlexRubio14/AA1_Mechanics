
[System.Serializable]
public struct LineC
{
    #region FIELDS
    public Vector3C origin;
    public Vector3C direction;
    #endregion

    #region PROPIERTIES
    #endregion

    #region CONSTRUCTORS
    public LineC(Vector3C origin, Vector3C direction)
    {
        this.origin = origin;
        this.direction = direction;
    }
    #endregion

    #region OPERATORS
    public static bool operator ==(LineC a, LineC b) //COMPARATOR 1
    {
        return (a.origin == b.origin) && (a.direction == b.direction);
    }

    public static bool operator !=(LineC a, LineC b) //COMPARATOR 2
    {
        return (a.origin != b.origin) || (a.direction != b.direction);
    }
    #endregion

    #region METHODS
    public Vector3C NearestPoint(Vector3C point)
    {
        Vector3C vector = point - origin;
        float dot = Vector3C.Dot(direction, vector);
        Vector3C nearestPoint = origin - direction * dot;

        return nearestPoint;
    }
    #endregion

    #region FUNCTIONS
    public static LineC CreateLineFromTwoPoints(Vector3C pointA, Vector3C pointB)
    {
        LineC lineTemp;
        lineTemp.origin = pointA;
        lineTemp.direction = pointB - pointA;

        return lineTemp;
    }
    #endregion

}