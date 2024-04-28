using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstructionController : MonoBehaviour
{
    [SerializeField] Text txtMessage;
    [SerializeField] float duration;
    Color startColor;
    Color endColor;
   
    void Start()
    {
        startColor = new Color(67 / 255.0f, 34 / 255.0f, 103 / 255.0f);
        endColor = Color.white;      
        StartCoroutine("ChangeColor");
    }

    IEnumerator ChangeColor()
    {
        while (true)
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;                
                txtMessage.color = Color.Lerp(startColor, endColor, t / duration);
                yield return null;
            }

            //intercambiamos los colores
            Color temColor=startColor;
            startColor=endColor;
            endColor=temColor;
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Nivel1");
        }

    }
}
