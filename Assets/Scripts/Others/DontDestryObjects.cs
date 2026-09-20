using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestryObjects : MonoBehaviour
{

    public GameObject[] dontDestryObs;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in dontDestryObs)
        {
            DontDestroyOnLoad(obj);
        }
    }
}
