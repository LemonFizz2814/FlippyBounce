using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    bool direction;

    const float speedMax = 6.0f;
    const float speedMin = 2.5f;
    float speed;
    float deathWaitTime = 6.0f;
    Rigidbody2D rb;

    private void Start()
    {
        direction = (Random.Range(0, 2) == 0)? true : false;

        gameObject.GetComponent<SpriteRenderer>().flipX = direction;

        transform.position = new Vector3((direction) ? -66 : -74 , Random.Range(1.5f, 50), 0);
        rb = gameObject.GetComponent<Rigidbody2D>();

        speed = Random.Range(speedMin, speedMax);

        StartCoroutine(DeathWait());
    }

    private void Update()
    {
        if(direction)
        {
            rb.velocity = new Vector3(-speed, 0, 0);
        }
        else
        {
            rb.velocity = new Vector3(speed, 0, 0);
        }
    }

    private IEnumerator DeathWait()
    {
        yield return new WaitForSeconds(deathWaitTime);
        Destroy(gameObject);
    }
}
