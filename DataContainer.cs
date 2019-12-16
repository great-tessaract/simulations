using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer : MonoBehaviour
{
    public float speed;
    public float sight;
    // Start is called before the first frame update
    public void SetGene(float speed, float sight)
    {
        this.speed = speed;
        this.sight = sight;
    }
    public float GetSpeed()
    {
        return this.speed;
    }
    public float GetSight()
    {
        return this.sight;
    }
}
