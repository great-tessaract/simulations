using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predator : MonoBehaviour
{
    public float speed;
    public float Food = 410;
    public float Water = 400;
    public float happines = -22;
    private Vector2 terriory;
    private Transform target;
    private GameObject Enemy;
    public Rigidbody2D rb;
    public float distancepredetor;
    public float livetime;
    public GameObject deadcreature;

    public GameObject spawning;


    // Start is called before the first frame update
    void Start()
    {
        livetime = 270f;
        rb = GetComponent<Rigidbody2D>();
        terriory = new Vector2(Random.Range(2f, -2f), Random.Range(2f, -2f));
    }

    // Update is called once per frame
    void Update()
    {
        livetime -= Time.deltaTime;
        Food += Time.deltaTime * -1f;
        Water += Time.deltaTime * -2f;
        if (Water < 0 || Food < 0 || livetime < 0 )
        {
            Instantiate(deadcreature, transform.position + new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            Destroy(gameObject);
        }
        else if (Water < 200f)
        {
            target = GameObject.FindGameObjectWithTag("water").GetComponent<Transform>();
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            RotateTowards(target.position);
        }
        else if (Food < 400f)
        {

            if (GameObject.FindGameObjectWithTag("herbivore") == true)
            {
                findclosest();
            }
            else if (GameObject.FindGameObjectWithTag("molard") == true)
            {
                target = GameObject.FindGameObjectWithTag("molard").GetComponent<Transform>();
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                RotateTowards(target.position);
            }

        }
        else if (Food > 500f && Water > 500f)
        {
            happines += 1;

        }
        else if (happines > 50f)
        {
            if (GameObject.FindGameObjectWithTag("predator") == true)
            {
                target = GameObject.FindGameObjectWithTag("predator").GetComponent<Transform>();
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                RotateTowards(target.position);
            }

        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, terriory, speed * Time.deltaTime);
        }

    }
    private void RotateTowards(Vector2 target)
    {
        var offset = 270f;
        Vector2 direction = target - (Vector2)transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }

    void findclosest()
    {
        float distancetoclosest = Mathf.Infinity;
        creature closest = null;
        creature[] allplants = GameObject.FindObjectsOfType<creature>();

        foreach (creature currentplant in allplants)
        {
            float distanceToPlant = (currentplant.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToPlant < distancetoclosest)
            {
                distancetoclosest = distanceToPlant;
                closest = currentplant;
            }
        }
        Debug.DrawLine(this.transform.position, closest.transform.position);
        transform.position = Vector2.MoveTowards(this.transform.position, closest.transform.position, speed * Time.deltaTime);
        RotateTowards(closest.transform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("water"))
        {
            Water = 600f;

        }
        if (other.gameObject.CompareTag("herbivore"))
        {
            Food = 600f;
            Destroy(other.gameObject);

        }
        if (other.gameObject.CompareTag("molard"))
        {
            Food = 200f;
            Destroy(other.gameObject);

        }
        if ((other.gameObject.CompareTag("predator")) && happines > 200)
        {
            StartCoroutine(mate());
            happines = 0;
            Food = Food * 0.07f;
            Water = Water * 0.02f;


        }
    }
    IEnumerator mate()
    {


        yield return new WaitForSeconds(3);
        Instantiate(spawning, transform.position + new Vector3(-0.5f, 0.5f, 0.0f), Quaternion.identity);


    }
}