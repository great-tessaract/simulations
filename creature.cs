using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creature : MonoBehaviour
{
    public float speed;
    public float Food = 110;
    public float Water = 100;
    public float happines = -22;
    private Transform target;
    private Transform targrt;
    private GameObject Enemy;
    public GameObject deadcreature;
    private Vector2 movement;
    public Rigidbody2D rb;
    public float distancepredetor;
    public LayerMask predator;
    public LayerMask edibles;
    public SpriteRenderer spriteRenderer;
    private float solor;
    private bool seesfood = false;
    private Vector3 goclosest;
    public float livetime;

    public GameObject spawning;
    public float sight;
    private float size = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = GetComponent<DataContainer>().GetSpeed();
        speed = speed + Random.Range(0.5f, -0.5f);
        sight = GetComponent<DataContainer>().GetSight();
        sight = sight + Random.Range(0.2f, -0.2f);
        solor = 255f;
        size = 0.5f;
        livetime = 400f;
    }

    // Update is called once per frame
    void Update()
    {
        livetime -= Time.deltaTime;
        Food += Time.deltaTime * -((speed + sight));
        Water += Time.deltaTime * -((speed + sight));
        solor -= Time.deltaTime * 10;
        size = Time.deltaTime * 0.002f;
        gameObject.transform.localScale += new Vector3(size, size, 0f);
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, sight, predator);
        RaycastHit2D hitInfo2 = Physics2D.Raycast(transform.position, transform.up, -sight, predator);
        RaycastHit2D hitInfo3 = Physics2D.Raycast(transform.position, transform.right, sight, predator);
        RaycastHit2D hitInfo4 = Physics2D.Raycast(transform.position, transform.right, -sight, predator);
        RaycastHit2D eye1 = Physics2D.Raycast(transform.position, transform.up, sight, edibles);
        RaycastHit2D eye2 = Physics2D.Raycast(transform.position, transform.up, sight, edibles);
        RaycastHit2D eye3 = Physics2D.Raycast(transform.position, transform.right, -sight, edibles);
        RaycastHit2D eye4 = Physics2D.Raycast(transform.position, transform.right, sight, edibles);
        Debug.DrawLine(transform.position, transform.position + transform.up * sight, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up * -sight, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.right * sight, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.right * -sight, Color.red);
        if (eye1.collider != null || eye2.collider != null || eye3.collider != null || eye4.collider != null)
        {
            seesfood = true;
        }
        else
        {
            seesfood = false;
        }
        if(spriteRenderer != null)
        {
            Color newColor = new Color(255f, 255f, solor);
            spriteRenderer.color = newColor;
        }
        if (Water < 0 || Food < 0 || livetime < 0)
        {
            Instantiate(deadcreature, transform.position + new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            Destroy(gameObject);
        }
        else if (hitInfo.collider != null || hitInfo2.collider != null || hitInfo3.collider != null || hitInfo4.collider != null)
        {
            target = GameObject.FindGameObjectWithTag("predator").GetComponent<Transform>();
            transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
            RotateTowards(target.position);
        }

        else if (Water < 50f)
        {
            target = GameObject.FindGameObjectWithTag("water").GetComponent<Transform>();
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            RotateTowards(target.position);
        }
        else if (Food < 50f && seesfood == true)
        {

            if (GameObject.FindGameObjectWithTag("plants") == true)
            {
                findclosest();

            }

        }
        else if (Food > 120f && Water > 110f)
        {
            happines += 1;

        }
        else if (happines > 50f)
        {
            if (GameObject.FindGameObjectWithTag("herbivore") == true)
            {
                target = GameObject.FindGameObjectWithTag("herbivore").GetComponent<Transform>();
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                RotateTowards(target.position);
            }

        }
        else
        {
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            rb.AddForce(movement * speed);
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
        Plant closest = null;
        Plant[] allplants = GameObject.FindObjectsOfType<Plant>();

        foreach(Plant currentplant in allplants)
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
            Water = 200f;

        }
        if (other.gameObject.CompareTag("plants"))
        {
            Food = 200f;
            Destroy(other.gameObject);

        }
        if ((other.gameObject.CompareTag("herbivore")) && happines > 50)
        {
            StartCoroutine(mate());
            happines = 0;
            Food = Food * 0.9f;
            Water = Water * 0.9f;
                


        }

    }
    IEnumerator mate()
    {


        yield return new WaitForSeconds(3);
        GameObject offspring = Instantiate(spawning, transform.position + new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        offspring.GetComponent<DataContainer>().SetGene(this.speed, this.sight);


    }
}
