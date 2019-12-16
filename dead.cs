using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dead : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(decay());

    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForce(movement * 0.5f);

    }
    IEnumerator decay()
    {


        yield return new WaitForSeconds(15);
        Destroy(gameObject);


    }
}
