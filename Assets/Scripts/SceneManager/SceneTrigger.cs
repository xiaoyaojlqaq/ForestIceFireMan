using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string targetScene;
    public Vector2 changePos;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        SceneTransitionManager.Instance.LoadScene(targetScene, () =>
        {
            other.transform.position = changePos;
        });
    }
}