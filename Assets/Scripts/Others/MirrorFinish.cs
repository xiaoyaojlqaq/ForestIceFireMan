using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MirrorFinish : MonoBehaviour
{
    PlayableDirector playableDirector;
    // Start is called before the first frame update
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playableDirector.state == PlayState.Paused)
        {
            SceneManager.LoadScene("EndScene");
            DontDestryObjects.DestroyDontDestryObjects();
        }
    }
}
