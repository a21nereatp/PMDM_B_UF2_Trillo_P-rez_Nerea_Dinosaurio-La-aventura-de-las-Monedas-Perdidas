using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Header("VALORES CONFIGURABLES")]
    [SerializeField] float parallax;

    Material mat;
    Transform cam;
    Vector3 initialPos;

    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        cam = Camera.main.transform;
        initialPos = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(cam.position.x, initialPos.y, initialPos.z);
        mat.mainTextureOffset = new Vector2(parallax * cam.position.x, 0);
    }
}
