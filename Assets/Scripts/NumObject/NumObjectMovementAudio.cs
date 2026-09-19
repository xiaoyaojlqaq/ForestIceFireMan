using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource), typeof(NumObjectInfo))]
public sealed class NumObjectMovementAudio : MonoBehaviour
{
    private const int MetalNumberThreshold = 10;

    [SerializeField] private AudioClip metalMoveLoop;
    [SerializeField] private AudioClip woodMoveLoop;
    [SerializeField, Min(0f)] private float minMoveSpeed = 0.05f;

    private Rigidbody2D body;
    private AudioSource audioSource;
    private NumObjectInfo info;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        info = GetComponent<NumObjectInfo>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 30f;
        audioSource.Stop();
    }

    private void FixedUpdate()
    {
        AudioClip loop = info.CurrentNumber > MetalNumberThreshold ? metalMoveLoop : woodMoveLoop;
        bool isMoving = loop != null &&
                        body.velocity.sqrMagnitude > minMoveSpeed * minMoveSpeed;

        if (isMoving)
        {
            if (audioSource.clip != loop)
                audioSource.clip = loop;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void OnDisable()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    public void Setup(AudioClip metalLoop, AudioClip woodLoop)
    {
        metalMoveLoop = metalLoop;
        woodMoveLoop = woodLoop;
    }
}
