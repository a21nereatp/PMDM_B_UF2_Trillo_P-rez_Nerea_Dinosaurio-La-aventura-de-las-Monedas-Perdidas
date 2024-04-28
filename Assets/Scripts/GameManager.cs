using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("TEXTOS")]
    [SerializeField] Text txtMessage;
    [SerializeField] Text txtNiveles;
    [SerializeField] Text counterCoins;
    [Header("VALORES OBJETOS")]
    [SerializeField] PlayerController player;
    [SerializeField] CheckPointController checkController;
    [SerializeField] Image black;
    [Header("BARRA DE VIDA")]
    [SerializeField] GameObject imgLife;
    [SerializeField] Sprite life3, life2, life1, nolife;

    [Header("SONIDOS")]
    [SerializeField] AudioClip sfxGameOver;


    public static GameManager Instance { get; private set; }
    public static List<int> totalCoins = new List<int> { 0, 0, 35, 69, 75 };
    AudioSource sfx;

    int lives;
    int coins;
    int sceneId;
    int levelNumber;
    bool fading = false;
    bool gameOver = false;
    bool sceneRestarted = false;
    bool paused;
    static bool checkpointReached = false;

    public int GetLives() => lives;
    public int GetCoins() => coins;
    public bool isGameOver() => gameOver;
    public bool isFading() => fading;
    public bool isChecking() => checkpointReached;

    public Image GetBlack() => black;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (!Instance.isFading())
            {
                Destroy(gameObject);
            }
        }
    }

    void Start()
    {
        sceneId = SceneManager.GetActiveScene().buildIndex;
        levelNumber = sceneId - 1;
        sfx = GetComponent<AudioSource>();
        // Carga las vidas guardadas, si no hay ninguna guardada, el valor por defecto será 3
        lives = PlayerPrefs.GetInt("Lives", 3);
        // Comprueba si la escena ha cambiado
        if (PlayerPrefs.GetInt("LastScene", -1) != sceneId)
        {
            // Si la escena ha cambiado, reinicia las monedas
            coins = 0;
        }
        else
        {
            // Si la escena no ha cambiado, recupera las monedas
            coins = PlayerPrefs.GetInt("Coins", 0);
        }
        // Guarda la escena actual
        PlayerPrefs.SetInt("LastScene", sceneId);
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
        ReadPreferences();
        //si es la primera vez que se recarga la escena
        if (PlayerPrefs.GetInt("isFirstTime", 1) == 1)
        {
            fading = true;
            black.gameObject.SetActive(true);
            // Tiempo de espera en segundos
            float delay = 2f;
            Invoke("StartFadeOut", delay);
            PlayerPrefs.SetInt("isFirstTime", 0);
        }
        else
        {
            Debug.Log("FadeOut");
            fading = true;
            StartCoroutine(FadeOut());
        }
        UpdateUI();
    }

    void StartFadeOut()
    {
        txtNiveles.text = "Level   " + levelNumber;
        StartCoroutine(FadeOut());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnApplicationQuit();
        }
        else if (!gameOver && Input.GetKeyDown(KeyCode.P))
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        else if (gameOver)
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Nivel1");
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene("Menu");
            }

    }

    public void ReadPreferences()
    {
        int volumeMusic = PlayerPrefs.GetInt("MusicVolum", 5);
        int volumeSound = PlayerPrefs.GetInt("SoundVolum", 4);

        Camera.main.GetComponent<AudioSource>().volume = volumeMusic / 10F;
        sfx.volume = volumeSound / 10F;
        // Ajusta el volumen de los otros scripts

        player.AdjustVolume(volumeSound / 10F);
        checkController.AdjustVolume(volumeSound / 10F);

        GameObject allCoins = GameObject.Find("AllCoins");
        foreach (Transform child in allCoins.transform)
        {
            CoinsController coinController = child.GetComponent<CoinsController>();
            if (coinController != null)
            {
                coinController.AdjustVolume(volumeSound / 10F);
            }
        }
    }

    IEnumerator FadeIn()
    {
        fading = true;
        for (float i = 0; i <= 1; i += 1f * Time.deltaTime)
        {
            // Ajusta la transparencia del sprite, se aparece lentamente
            var color = black.color;
            color.a = i;
            black.color = color;

            yield return null;
        }
        // Comprueba si la escena que se va a cargar no es la última
        if (sceneId != SceneManager.sceneCountInBuildSettings - 1)
        {
            txtNiveles.text = "Level   " + levelNumber;
        }
        else
        {
            txtNiveles.text = "";
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneId);
    }

    IEnumerator FadeOut()
    {
        // Comprueba si la escena que se va a cargar no es la última
        if (sceneId != SceneManager.sceneCountInBuildSettings - 1)
        {
            txtNiveles.text = "Level   " + levelNumber;
        }
        Camera.main.GetComponent<AudioSource>().Play();
        black.gameObject.SetActive(true);
        for (float i = 1; i >= 0; i -= Time.deltaTime / 4)
        {
            //se desvanece
            var color = black.color;
            color.a = i;
            black.color = color;
            yield return null;
        }
        txtNiveles.text = "";
        fading = false;
    }

    public void LoseLife()
    {

        if (!gameOver)
        {
            lives--;
            //resetea vidas a las actuales
            PlayerPrefs.SetInt("Lives", lives);
            if (sceneRestarted) // Solo guardar las monedas si la escena no se ha reiniciado
            {
                PlayerPrefs.SetInt("Coins", coins);
            }

            if (lives <= 0)
            {
                Camera.main.GetComponent<AudioSource>().Stop();
                sfx.clip = sfxGameOver;
                sfx.Play();
                GameOver();
            }
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        switch (lives)
        {
            case 3:
                imgLife.GetComponent<Image>().sprite = life3;
                break;
            case 2:
                imgLife.GetComponent<Image>().sprite = life2;
                break;
            case 1:
                imgLife.GetComponent<Image>().sprite = life1;
                break;
            case 0:
                imgLife.GetComponent<Image>().sprite = nolife;
                break;
            default:
                break;
        }
        if (gameOver)
        {
            counterCoins.text = "00";
        }
        else
        {
            if (coins < 10)
            {
                counterCoins.text = "0" + coins;
            }
            else
            {
                counterCoins.text = coins.ToString();
            }
        }
        counterCoins.text += "/" + totalCoins[sceneId];
    }

    public void CheckpointReached()
    {      
        checkpointReached = true;
        sceneRestarted = false; // Restablecer sceneRestarted a false después de un checkpoint
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        gameOver = true;
        checkpointReached = false;
        //mostramos el texto de Game Over
        txtMessage.text = "GAME OVER\n PRESS <RETURN> TO RESTART \nOR\n<M> TO GO BACK TO MENU";
        // Resetear las vidas, la posición del jugador y monedas en PlayerPrefs
        PlayerPrefs.SetInt("Lives", 3);
        PlayerPrefs.SetFloat("PlayerX", player.GetInitialPosition().x);
        PlayerPrefs.SetFloat("PlayerY", player.GetInitialPosition().y);
        PlayerPrefs.SetInt("Coins", 0);
    }

    public void LoadScene(bool loadNextScene = false)
    {
        black.gameObject.SetActive(true);
        sceneRestarted = true;
        gameOver = false;
        Camera.main.GetComponent<AudioSource>().Stop();
        
        if (loadNextScene)
        {
            sceneId++;
            checkpointReached=false;
        }
        StartCoroutine(FadeIn());
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Lives");
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.DeleteKey("isFirstTime");
        Application.Quit();
    }

    private void PauseGame()
    {
        paused = true;
        //Pausamo sonido del juego y aparece texto
        Camera.main.GetComponent<AudioSource>().Stop();
        txtMessage.text = "PAUSED\nPRESS <P> TO RESUME";
        Time.timeScale = 0;

    }

    private void ResumeGame()
    {
        paused = false;
        //Pausamo sonido del juego y aparece texto
        Camera.main.GetComponent<AudioSource>().Play();
        txtMessage.text = "";
        Time.timeScale = 1;
    }

    public static void SumCoins()
    {
        Instance.coins++;
        if (Instance.coins < 10)
        {
            Instance.counterCoins.text = "0" + Instance.coins;
        }
        else
        {
            Instance.counterCoins.text = Instance.coins.ToString();
        }
        Instance.counterCoins.text += "/" + totalCoins[Instance.sceneId];

        if (Instance.coins == totalCoins[Instance.sceneId])
        {
            Instance.LoadScene(true);
        }
    }


}


