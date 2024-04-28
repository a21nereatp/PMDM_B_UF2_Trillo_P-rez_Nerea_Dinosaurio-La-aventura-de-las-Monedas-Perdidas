using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    [SerializeField] Sprite check;
    [SerializeField] AudioClip sfxCheck;
    [SerializeField] GameObject posCheck;

    GameObject[] coins;
    SpriteRenderer spriteRen;
    PlayerController player;
    AudioSource sfx;
    BoxCollider2D box;
    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        spriteRen = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        coins = GameObject.FindGameObjectsWithTag("Coins");
    }
    public void AdjustVolume(float volume)
    {
        sfx.volume = volume;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            box.enabled = false;
            sfx.clip = sfxCheck;
            sfx.Play();
            spriteRen.sprite = check;
            GameManager.Instance.CheckpointReached();
        }
    }
}
