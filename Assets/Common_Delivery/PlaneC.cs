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
        this.position = Vector3C.zero; // modificar
        this.normal = new Vector3C(a, b, c);
    }
    #endregion

    #region OPERATORS

    #endregion

    #region METHODS
    #endregion

    #region FUNCTIONS
    #endregion

}