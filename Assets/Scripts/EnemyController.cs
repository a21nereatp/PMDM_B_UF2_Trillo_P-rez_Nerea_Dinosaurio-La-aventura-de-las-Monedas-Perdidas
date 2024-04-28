using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("VALORES CONFIGURABLES")]
    [SerializeField] float velocidad;
    [SerializeField] float distanciaRecorrida;

    Vector2 direccionMovimiento = new Vector2(-1, 0);
    private int indicePunto = 0;
    private Vector3[] puntosMov;

    void Start()
    {
        // Asignar puntos de movimiento en función de la posición actual, dirección serializada y distancia recorrida
        AssignPointsDistance();
    }

    void Update()
    {
        EnemyMove();
    }

    private void EnemyMove()
    {
        // La plataforma se mueve hacia el punto actual
        transform.position = Vector3.MoveTowards(transform.position, puntosMov[indicePunto], velocidad * Time.deltaTime);

        // Cuando se está llegando al punto
        if (Vector3.Distance(transform.position, puntosMov[indicePunto]) < 0.1f)
        {
            // Cambiar al siguiente punto
            indicePunto = (indicePunto + 1) % puntosMov.Length;

            // Voltear el enemigo según la dirección del siguiente punto
            float direccion = puntosMov[indicePunto].x - transform.position.x;
            Flip(direccion);
        }
    }

    private void AssignPointsDistance()
    {
        // Calcular el punto final basado en la posición actual, dirección serializada y distancia recorrida
        Vector3 puntoFinal = transform.position + new Vector3(direccionMovimiento.x, direccionMovimiento.y, 0) * distanciaRecorrida;

        // Asignar los puntos de movimiento
        puntosMov = new Vector3[] { transform.position, puntoFinal, transform.position };
    }

    private void Flip(float direccion)
    {
        if (Mathf.Abs(direccion) > Mathf.Epsilon)
        {
            transform.localScale = new Vector2(Mathf.Sign(-direccion), 1);
        }
    }
}
