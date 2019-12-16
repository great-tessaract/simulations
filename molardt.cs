using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class molardt : MonoBehaviour
{
    public float speed;
    public float Food = 110;
    public float Water = 100;
    public float happines = -22;
    private Transform target;
    private Transform targrt;
    private GameObject Enemy;
    private Vector2 movement;
    public Rigidbody2D rb;
    public float distancepredetor;
    public LayerMask predator;
    public SpriteRenderer spriteRenderer;
    private float solor;
    private float hercolor;
    private float childcolor;
    private Vector3 goclosest;
    public float livetime;
    public GameObject spawning;
    public float sight;
    public GameObject deadcreature;

    // Start is called before the first frame update
    void Start()
    {
        livetime = 300f;
        rb = GetComponent<Rigidbody2D>();
        speed = 2f;
        sight = GetComponent<DataContainer>().GetSight();
        sight = sight + Random.Range(0.2f, -0.2f);
        solor = GetComponent<DataContainer>().GetSpeed(); 
    }

    // Update is called once per frame
    void Update()
    {
        livetime -= Time.deltaTime;
        Food += Time.deltaTime * -((speed + sight) * 0.1f);
        Water += Time.deltaTime * -((speed + sight) * 0.5f);
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, sight, predator);
        RaycastHit2D hitInfo2 = Physics2D.Raycast(transform.position, transform.up, -sight, predator);
        RaycastHit2D hitInfo3 = Physics2D.Raycast(transform.position, transform.right, sight, predator);
        RaycastHit2D hitInfo4 = Physics2D.Raycast(transform.position, transform.right, -sight, predator);
        Debug.DrawLine(transform.position, transform.position + transform.up * sight, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.up * -sight, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.right * sight, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.right * -sight, Color.red);
        if (spriteRenderer != null)
        {
            Color newColor = new Color(0.5f, 0.5f, solor);
            spriteRenderer.color = newColor;
        }
        if (Water < 0 || Food < 0 || livetime < 0)
        {
            Instantiate(deadcreature, transform.position + new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            Destroy(gameObject);
        }
        else if (hitInfo.collider != null || hitInfo3.collider != null)
        {
            target = GameObject.FindGameObjectWithTag("predator").GetComponent<Transform>();
            transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
            RotateTowards(target.position);
        }
        else if (hitInfo2.collider != null || hitInfo4.collider != null)
        {
            target = GameObject.FindGameObjectWithTag("predator").GetComponent<Transform>();
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            RotateTowards(target.position);
        }

        else if (Water < 50f)
        {
            if (spriteRenderer != null)
            {
                Color newColor = new Color(0f, 0.5f, solor);
                spriteRenderer.color = newColor;
            }
            target = GameObject.FindGameObjectWithTag("water").GetComponent<Transform>();
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            RotateTowards(target.position);
        }
        else if (Food < 200f)
        {
            if (spriteRenderer != null)
            {
                Color newColor = new Color(0.5f, 0f, solor);
                spriteRenderer.color = newColor;
            }

            if (GameObject.FindGameObjectWithTag("deadherbivore") == true)
            {
                findclosest();

            }

        }
        else if (Food < 20f)
        {
            if (spriteRenderer != null)
            {
                Color newColor = new Color(0.5f, 0.5f, solor);
                spriteRenderer.color = newColor;
            }
            target = GameObject.FindGameObjectWithTag("plants").GetComponent<Transform>();
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            RotateTowards(target.position);
        }
        else if (Food > 120f && Water > 110f)
        {
            happines += 1;

        }
        else if (happines > 50f)
        {
            if (GameObject.FindGameObjectWithTag("molard") == true)
            {
                target = GameObject.FindGameObjectWithTag("molard").GetComponent<Transform>();
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                RotateTowards(target.position);
            }

        }
        else
        {

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
        dead closest = null;
        dead[] allplants = GameObject.FindObjectsOfType<dead>();

        foreach (dead currentplant in allplants)
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
        if (other.gameObject.CompareTag("deadherbivore"))
        {
            Food += 50f;
            Destroy(other.gameObject);

        }
        if (other.gameObject.CompareTag("plants") && Food < 25)
        {
            Food += 25f;
            Destroy(other.gameObject);

        }
        if ((other.gameObject.CompareTag("molard")) && happines > 50)
        {
            hercolor = other.gameObject.GetComponent<DataContainer>().GetSpeed();
            StartCoroutine(mate());
            happines = 0;
            Food = Food * 0.2f;
            Water = Water * 0.9f;



        }

    }
    IEnumerator mate()
    {


        yield return new WaitForSeconds(3);
        childcolor = solor + hercolor;
        if (childcolor > 1f)
        {
            childcolor = 0.1f;
        }

        GameObject offspring = Instantiate(spawning, transform.position + new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        offspring.GetComponent<DataContainer>().SetGene(childcolor, this.sight);


    }
}
