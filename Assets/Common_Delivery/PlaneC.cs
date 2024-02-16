using System;

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
        this.position = pointA;
        this.normal = Vector3C.Cross(pointB - pointA, pointC - pointA);
    }
    public PlaneC(float a, float b, float c, float d)
    {
        if(a != 0)
            this.position = new Vector3C(d / a, 0, 0); 
        else if(b != 0)
            this.position = new Vector3C(0, d / b, 0);
        else if (c != 0)
            this.position = new Vector3C(0, 0, d / c);
        else 
            this.position = Vector3C.zero;

        this.normal = new Vector3C(a, b, c);
    }
    #endregion

    #region OPERATORS

    #endregion

    #region METHODS
    
    #endregion

    #region FUNCTIONS
    //ToEquation(Ax + By + Cz + D = 0)
    //NearestPoint
    //Intersection
    //Equals
    #endregion

}