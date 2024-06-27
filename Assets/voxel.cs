using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public bool isActive;
    public Vector3 position;    

    public Voxel(bool? isActive, Vector3 position)
    {
        this.isActive = isActive ?? true;
        this.position = position;
    }   
}
