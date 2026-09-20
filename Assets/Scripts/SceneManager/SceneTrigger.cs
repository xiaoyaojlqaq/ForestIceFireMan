using UnityEngine;
using UnityEngine.Playables;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string targetScene;
    public Vector2 changePos;
    private PlayableDirector mainUI;
    private void Start()
    {
        if (mainUI == null)
        {
            mainUI=GameObject.Find("MainUICanvas")?.GetComponent<PlayableDirector>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        SceneTransitionManager.Instance.LoadScene(targetScene, () =>
        {
            mainUI.Play();
            other.transform.position = changePos;
        });
    }
}