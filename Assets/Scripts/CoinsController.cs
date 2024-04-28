using UnityEngine;

public class CoinsController : MonoBehaviour
{
    SpriteRenderer sprite;
    GameManager gameManager;
    ParticleSystem particles;
    AudioSource sfx;
    bool active = true;

    [SerializeField] AudioClip sfxCoins;

    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();
        sfx = GameObject.Find("AllCoins").GetComponent<AudioSource>();
    }
    public void AdjustVolume(float volume)
    {
        sfx.volume = volume;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && active)
        {
            GameManager.SumCoins();
            sfx.clip = sfxCoins;
            sfx.Play();
            sprite.enabled = false;
            particles.Play();
            active = false;

        }
    }
}
