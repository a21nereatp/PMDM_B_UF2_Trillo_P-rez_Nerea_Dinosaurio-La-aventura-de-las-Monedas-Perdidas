using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("VALORES CONFIGURABLES")]
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [SerializeField] GameObject checkPoint;
    [Header("SONIDOS")]
    [SerializeField] AudioClip sfxJump;
    [SerializeField] AudioClip sfxHit;
    [SerializeField] AudioClip sfxDeath;
    [SerializeField] AudioClip sfxDestroy;

    GameManager gm;

    Rigidbody2D rb;
    AudioSource sfx;
    Collider2D col;
    Animator anim;
    SpriteRenderer sprite;
    Color original;

    float moveX;

    bool jump;
    public static bool isClimbing = false;
    bool hitBack = false;

    Vector3 posIni;

    public Vector3 GetInitialPosition() => posIni;
    private void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        gm = GameManager.Instance;
        posIni = transform.position;
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        original = sprite.color;
        gm.ReadPreferences();
    }
    public void AdjustVolume(float volume)
    {
        sfx.volume = volume;
    }


    void Update()
    {
        if (!gm.isGameOver() && !gm.isFading())// Verifica si el juego no está en modo "game over" y el fondo no se está desvaneciendo
        {
            moveX = Input.GetAxisRaw("Horizontal");

            if (!jump && Input.GetButtonDown("Jump"))
            {
                jump = true;
            }
            if (isClimbing)
            {
                float inputVertical = Input.GetAxis("Vertical");
                rb.velocity = new Vector2(0, inputVertical * 3f);
                if (inputVertical != 0)
                {
                    anim.Play("ClimLadder");
                }
                else
                {
                    anim.Play("StillLadder");
                }

            }
        }

    }

    private void FixedUpdate()
    {
        if (!gm.isGameOver() && !gm.isFading())// Verifica si el juego no está en modo "game over" y el fondo no se está desvaneciendo
        {
            Walk();
            Flip();
            Jump();
        }
    }

    private void Jump()
    {
        if (!jump) { return; }
        jump = false;
        if (!col.IsTouchingLayers(LayerMask.GetMask("Terreno", "Plataformas"))) { return; }
        rb.velocity = new Vector2(0, jumpSpeed);
        anim.SetTrigger("isJumping");
        sfx.clip = sfxJump;
        sfx.Play();
    }

    public void Flip()
    {
        float vx = rb.velocity.x;
        if (Mathf.Abs(vx) > Mathf.Epsilon)
        {
            transform.localScale = new Vector2(Mathf.Sign(vx), 1);
        }
    }

    void Walk()
    {
        if (!hitBack)
        {
            Vector2 vel = new Vector2(moveX * speed * Time.fixedDeltaTime, rb.velocity.y);
            rb.velocity = vel;

            //miramos si la velocidad es mayor que 0
            anim.SetBool("isWalking", Mathf.Abs(rb.velocity.x) > Mathf.Epsilon);
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "MobilePlatform")
        {
            //cuando el jugador está en la plataforma movil pasa a ser parte de esta plataforma, por lo 
            //que cuando se mueve la plataforma el jugador se mueve con ella.
            transform.parent = other.transform;
        }
        if (other.gameObject.tag == "Enemy")
        {
            EnemyDamage(other.transform.position.x);
        }
        if (!hitBack && other.gameObject.tag == "Back")
        {
            rb.velocity = Vector2.zero;
            //para que el jugador rebote encima el enemigo
            rb.AddForce(new Vector2(0.0f, 0.9f), ForceMode2D.Impulse);
            Destroy(other.gameObject);
            sfx.clip = sfxDestroy;
            sfx.Play();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "MobilePlatform")
        {   //para que cuando el jugador salga de plataforma deje de depender de ningun objeto
            transform.parent = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Spikes")
        {

            sfx.clip = sfxDeath;
            sfx.Play();
            gm.LoseLife();
            Dead();
        }
        if (other.gameObject.tag == "Fall")
        {
            sfx.clip = sfxDeath;
            sfx.Play();
            gm.LoseLife();
            if (!gm.isGameOver())
            {
                if (!gm.isChecking())
                {
                    StartCoroutine(RespawnAfterAnimation());
                }
                else if (gm.isChecking())
                {
                    StartCoroutine(RespawnPlayer());
                }
            }
        }
        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = 2;
            anim.Play("Quieto");
            rb.velocity = new Vector2(rb.velocity.x, 0);

        }
    }

    private void EnemyDamage(float posX)
    {
        if (!gm.isGameOver())
        {
            //creamos un color para que cuando el jugador se haga daño se muestre de ese color
            //los valores los hay que normalizarlos en el rango de 0 a 1.  
            //por eso divide entre 255 que es el rango máximo
            Color nuevoColor = new Color(255f / 255f, 100f / 255f, 100f / 255f);
            sprite.color = nuevoColor;
            sfx.clip = sfxHit;
            sfx.Play();
            if (!hitBack)
            {
                // Calcula la dirección del enemigo en relación con el jugador
                float direction = Mathf.Sign(posX - transform.position.x);
                rb.velocity = Vector2.zero;

                // Se aplica una fuerza de retroceso al jugador cuando toca a un enemigo
                //la dirección es negativa para que el jugador salte hacia atrás
                rb.AddForce(new Vector2(0.5f * -direction, 0.5f), ForceMode2D.Impulse);

                hitBack = true;
                gm.LoseLife();
                if (gm.GetLives() > 0)
                {
                    StartCoroutine(ResetHitBack());
                }
                else
                {
                    Dead();
                }

            }
        }
    }

    IEnumerator ResetHitBack()
    {
        yield return new WaitForSeconds(0.5f);
        hitBack = false;
        // Restablece el color del sprite al color original
        sprite.color = original;
    }


    public void Dead()
    {
        // Desactiva los controles del personaje
        this.enabled = false;

        // Reproducir la animación DeadPlayer
        anim.Play("DeadPlayer");

        rb.velocity = Vector2.zero;
        //darle un saltito
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

        //y que se caiga hacia abajo
        col.enabled = false;
        if (!gm.isGameOver() && !gm.isChecking())
        {
            StartCoroutine(RespawnAfterAnimation());
        }
        else if (gm.isChecking())
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    IEnumerator RespawnAfterAnimation()
    {
        yield return new WaitForSeconds(2.5f);
        gm.LoadScene();
    }
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(3f);
        //reactivamod los controles del personaje 
        this.enabled = true;
        //reactivar el collider y la animacion inicial del jugaor
        anim.Play("Quieto");
        col.enabled = true;
        // Reiniciar la posición del jugador al checkpoint y la velocidad
        transform.position = checkPoint.transform.position;
        rb.velocity = Vector2.zero;
    }
}


