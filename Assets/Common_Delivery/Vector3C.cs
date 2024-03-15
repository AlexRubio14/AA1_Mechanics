using System;

[System.Serializable]
public struct Vector3C
{
    #region FIELDS
    public float x;
    public float y;
    public float z;
    #endregion

    #region PROPIERTIES
    public float r { get => x; set => x = value; }
    public float g { get => y; set => y = value; }
    public float b { get => z; set => z = value; }
    public float magnitude { get { return (float)Math.Sqrt((x * x) + (y * y) + (z * z)); } }
    public Vector3C normalized
    {
        get
        {
            if (magnitude == 0)
                return zero;

            return new Vector3C(x, y, z) / magnitude;
        }
    }

    public static Vector3C zero { get { return new Vector3C(0, 0, 0); } }
    public static Vector3C one { get { return new Vector3C(1, 1, 1); } }
    public static Vector3C right { get { return new Vector3C(1, 0, 0); } }
    public static Vector3C up { get { return new Vector3C(0, 1, 0); } }
    public static Vector3C forward { get { return new Vector3C(0, 0, 1); } }

    public static Vector3C black { get { return new Vector3C(0, 0, 0); } }
    public static Vector3C white { get { return new Vector3C(1, 1, 1); } }
    public static Vector3C red { get { return new Vector3C(1, 0, 0); } }
    public static Vector3C green { get { return new Vector3C(0, 1, 0); } }
    public static Vector3C blue { get { return new Vector3C(0, 0, 1); } }
    #endregion

    #region CONSTRUCTORS
    public Vector3C(float x = 0, float y = 0, float z = 0)
    {
        this.x = x; this.y = y; this.z = z;
    }
    #endregion

    #region OPERATORS
    public static Vector3C operator +(Vector3C a) //POSITIVE
    {
        return a;
    }

    public static Vector3C operator -(Vector3C a) //NEGATIVE
    {
        return a * -1;
    }

    public static bool operator ==(Vector3C a, Vector3C b) //COMPARATOR 1
    {
        return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
    }

    public static bool operator !=(Vector3C a, Vector3C b) //COMPARATOR 2
    {
        return (a.x != b.x) || (a.y != b.y) || (a.z != b.z);
    }

    public static Vector3C operator +(Vector3C a, Vector3C b) //ADD
    {
        return new Vector3C(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3C operator -(Vector3C a, Vector3C b) //SUBSTRACT
    {
        return new Vector3C(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3C operator *(Vector3C a, Vector3C b) //MULTIPLY
    {
        return new Vector3C(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3C operator /(Vector3C a, Vector3C b) //DIVIDE
    {
        Vector3C temp = zero;

        if (b.x != 0)
            temp.x = a.x / b.x;
        if (b.y != 0)
            temp.y = a.y / b.y;
        if (b.z != 0)
            temp.z = a.z / b.z;

        return temp;
    }

    public static Vector3C operator *(Vector3C a, float d) //MULTIPLY NUMBER
    {
        return new Vector3C(a.x * d, a.y * d, a.z * d);
    }
    public static Vector3C operator /(Vector3C a, float d) // DIVIDE NUMBER
    {
        if (d == 0)
            return zero;

        return new Vector3C(a.x / d, a.y / d, a.z / d);
    }
    #endregion

    #region METHODS
    public void Normalize()
    {
        this = normalized;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector3C)
        {
            Vector3C other = (Vector3C)obj;
            return other == this;
        }
        return false;
    }
    #endregion

    #region FUNCTIONS
    public static float Dot(Vector3C v1, Vector3C v2)
    {
        return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z);
    }
    public static Vector3C Cross(Vector3C v1, Vector3C v2)
    {
        return new Vector3C(
            v1.y * v2.z - v1.z * v2.y,
            v1.z * v2.x - v1.x * v2.z,
            v1.x * v2.y - v1.y * v2.x
        );
    }
    public static Vector3C CreateVector3(Vector3C a, Vector3C b)
    {
        return b - a;
    } 
    #endregion

}