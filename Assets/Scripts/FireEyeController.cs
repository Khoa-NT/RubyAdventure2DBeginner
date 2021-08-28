using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEyeController : MonoBehaviour
{
    // ----------------------------- For Movement -----------------------------
    public float speed = 2;

    // Moving axis
    public bool vertical;

    // Moving interval
    public float changeTime = 2.0f;
    // Timer
    float timer;
    // Direction status
    int direction = 1;

    // MOving object by physics instead of transform
    Rigidbody2D rigidbody2d;

    // ----------------------------- For Animation -----------------------------
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;

        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;

        }

        rigidbody2d.MovePosition(position);

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }




}
