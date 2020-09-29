using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ref, short for References, is a script that stores references to commonly accessed scripts and objects
/// </summary>
public class Ref : MonoBehaviour
{
    /// <summary>
    /// The single instance of the References script
    /// </summary>
    public static Ref I { get; private set; }
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }


    [Header("Refs set before runtime")]
    public GameObject Camera;
    public GameObject TwoDButton;
    public GameObject ThreeDButton;
}
