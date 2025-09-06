using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineObject : MonoBehaviour
{
    public static LineObject instance;
    public GameObject[] AllDots;
    public GameObject ImageObject,DotParent;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
}
