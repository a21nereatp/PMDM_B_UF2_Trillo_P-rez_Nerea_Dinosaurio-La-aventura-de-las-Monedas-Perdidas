using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("OPCIONES GENERALES")]
    [SerializeField] float timeChange;
    [SerializeField] int volumeMusic;
    [SerializeField] int volumeSound;
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject optionsScreen;

    [Header("ELEMENTOS DEL OPCIONES")]
    [SerializeField] SpriteRenderer music;
    [SerializeField] SpriteRenderer sound;
    [SerializeField] SpriteRenderer back;

    [Header("ELEMENTOS DEL MENU")]
    [SerializeField] SpriteRenderer start;
    [SerializeField] SpriteRenderer options;
    [SerializeField] SpriteRenderer exit;

    [Header("SPRITES DEL MENU")]
    [SerializeField] Sprite start_off;
    [SerializeField] Sprite start_on;
    [SerializeField] Sprite options_off;
    [SerializeField] Sprite options_on;
    [SerializeField] Sprite exit_off;
    [SerializeField] Sprite exit_on;
    [Header("SPRITES DE OPCIONES")]
    [SerializeField] Sprite music_off;
    [SerializeField] Sprite music_on;
    [SerializeField] Sprite sound_off;
    [SerializeField] Sprite sound_on;
    [SerializeField] Sprite back_off;
    [SerializeField] Sprite back_on;
    [SerializeField] Sprite vol_on;
    [SerializeField] Sprite vol_off;
    [SerializeField] SpriteRenderer[] music_spr;
    [SerializeField] SpriteRenderer[] sound_spr;

    [Header("SONIDOS DEL MENU")]
    [SerializeField] AudioSource sfxMenu;
    [SerializeField] AudioSource sfxOptions;
    [SerializeField] AudioSource sfxSelection;

    int screen;
    int optionMenu, optionMenuAnt;
    int optionOptions, optionOptionsAnt;
    bool submit;
    float x, y;
    float timeX, timeY;

    void Awake()
    {
        screen = 0;
        timeX = timeY = 0;
        optionMenu = optionMenuAnt = 1;
        AdjustOptions();
        ReadPreferences();
    }

    private void AdjustOptions()
    {
        //nos muestra cuando se entra en opciones como está ajustado la musica y sonido
        AdjustMusic();
        AdjustSound();
    }

    void Update()
    {
        y = Input.GetAxisRaw("Vertical");
        x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonUp("Submit")) submit = false;
        if (y == 0) timeY = 0;
        if (screen == 0) Menu();
        if (screen == 1) OptionsMenu();
    }


    private void Menu()//nos movemos por el menu principal
    {
        if (y != 0)
        {
            if (timeY == 0 || timeY > timeChange)
            {
                if (y == 1 && optionMenu > 1) SelectionMenu(optionMenu - 1);
                if (y == -1 && optionMenu < 3) SelectionMenu(optionMenu + 1);
                if (timeY > timeChange) timeY = 0;
            }
            timeY += Time.deltaTime;
        }
        if (Input.GetButtonDown("Submit") && !submit)
        {
            sfxSelection.Play();
            if (optionMenu == 1) SceneManager.LoadScene("Instrucciones");
            if (optionMenu == 2) LoadScreenOptions();
            if (optionMenu == 3) Application.Quit();
        }
    }

    private void OptionsMenu()//nos movemos por el menu opciones
    {
        if (y != 0)
        {
            if (timeY == 0 || timeY > timeChange)
            {
                if (y == 1 && optionOptions > 1) SelectionOptions(optionOptions - 1);
                if (y == -1 && optionOptions < 3) SelectionOptions(optionOptions + 1);
                if (timeY > timeChange) timeY = 0;
            }
            timeY += Time.deltaTime;
        }
        if (x == 0) timeX = 0;
        else
        {//para aumentar o disminuir el volumen y el sonido
            if ((timeX == 0 || timeX > timeChange) && (optionOptions == 1 || optionOptions == 2))
            {
                if (optionOptions == 1 && ((x < 0 && volumeMusic > 0) || (x > 0 && volumeMusic < 10)))
                {
                    volumeMusic += (int)x;
                    AdjustMusic();
                }
                if (optionOptions == 2 && ((x < 0 && volumeSound > 0) || (x > 0 && volumeSound < 10)))
                {
                    volumeSound += (int)x;
                    AdjustSound();
                    sfxOptions.Play();
                }
                if (timeX > timeChange) timeX = 0;
            }
            timeX += Time.deltaTime;
        }


        if (Input.GetButtonDown("Submit") && optionOptions == 3 && !submit)
        {
            LoadPreferences();
            LoadScreenMenu();

        }
    }

    private void LoadPreferences()
    {
        PlayerPrefs.SetInt("MusicVolum", volumeMusic);
        PlayerPrefs.SetInt("SoundVolum", volumeSound);
        PlayerPrefs.Save();
    }
    void ReadPreferences()
    {
        volumeMusic = PlayerPrefs.GetInt("MusicVolum", 5);
        volumeSound = PlayerPrefs.GetInt("SoundVolum", 4);
    }

    private void AdjustMusic()
    {
        if (volumeMusic == 0) music_spr[0].enabled = true;
        else music_spr[0].enabled = false;
        for (int i = 1; i <= 10; i++)
        {
            if (i <= volumeMusic) music_spr[i].sprite = vol_on;
            else music_spr[i].sprite = vol_off;
        }
        sfxMenu.volume = volumeMusic / 10F;

    }

    private void AdjustSound()
    {
        if (volumeSound == 0) sound_spr[0].enabled = true;
        else sound_spr[0].enabled = false;
        for (int i = 1; i <= 10; i++)
        {
            if (i <= volumeSound) sound_spr[i].sprite = vol_on;
            else sound_spr[i].sprite = vol_off;
        }
        GameObject[] sounds = GameObject.FindGameObjectsWithTag("Sounds");
        foreach (GameObject sound in sounds)
        {
            sound.GetComponent<AudioSource>().volume = volumeSound / 10F;
        }

    }

    private void LoadScreenOptions()
    {
        submit = true;
        sfxSelection.Play();
        menuScreen.SetActive(false);
        screen = 1;
        optionOptions = optionOptionsAnt = 1;
        music.sprite = music_on;
        sound.sprite = sound_off;
        back.sprite = back_off;
        optionsScreen.SetActive(true);
    }
    private void LoadScreenMenu()
    {
        submit = true;
        sfxSelection.Play();
        optionsScreen.SetActive(false);
        screen = 0;
        menuScreen.SetActive(true);
    }

    private void SelectionOptions(int op)
    {
        sfxOptions.Play();
        optionOptions = op;
        if (op == 1) music.sprite = music_on;
        if (op == 2) sound.sprite = sound_on;
        if (op == 3) back.sprite = back_on;
        if (optionOptionsAnt == 1) music.sprite = music_off;
        if (optionOptionsAnt == 2) sound.sprite = sound_off;
        if (optionOptionsAnt == 3) back.sprite = back_off;
        optionOptionsAnt = op;
    }

    private void SelectionMenu(int op)
    {
        sfxOptions.Play();
        optionMenu = op;
        if (op == 1) start.sprite = start_on;
        if (op == 2) options.sprite = options_on;
        if (op == 3) exit.sprite = exit_on;
        if (optionMenuAnt == 1) start.sprite = start_off;
        if (optionMenuAnt == 2) options.sprite = options_off;
        if (optionMenuAnt == 3) exit.sprite = exit_off;
        optionMenuAnt = op;
    }
}
