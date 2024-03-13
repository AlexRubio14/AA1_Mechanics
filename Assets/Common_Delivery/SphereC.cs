
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

   
    #endregion

    #region FUNCTIONS
    #endregion

}