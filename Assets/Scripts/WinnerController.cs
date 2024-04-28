using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinnerController : MonoBehaviour
{
    [Header("VALORES CONFIGURABLES")]
    [SerializeField] float minSize = 10f;
    [SerializeField] float maxSize = 10f;
    [SerializeField] float speed = 1f;
    [SerializeField] float duration;   
    private Text textComponent;
    Color startColor;
    Color endColor;
    void Start()
    {
        textComponent = GetComponent<Text>();
        startColor=Color.white;
        endColor=Color.cyan;
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
                textComponent.color = Color.Lerp(Color.white,Color.yellow, t / duration);
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
        // Oscilación entre minSize y maxSize
        float size = (maxSize - minSize) * Mathf.Abs(Mathf.Sin(Time.time * speed)) + minSize;
        textComponent.fontSize = (int)size;
    }
}
