using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestryObjects : MonoBehaviour
{
    public static GameObject[] statscDontDestryObs;

    public GameObject[] dontDestryObs;
    // Start is called before the first frame update
    void Start()
    {
        statscDontDestryObs = dontDestryObs;
        DontDestroyOnLoad(gameObject);
        foreach (GameObject obj in dontDestryObs)
        {
            DontDestroyOnLoad(obj);
        }
    }
    public static void DestroyDontDestryObjects()
    {
        //GameObject[] dontDestryObs = GameObject.FindGameObjectsWithTag("DontDestroy");
        foreach (GameObject obj in statscDontDestryObs)
        {
            Destroy(obj);
        }
    }
}
