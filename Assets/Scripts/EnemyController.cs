using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
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


    // Projectile
    bool broken = true;

    // ----------------------------- For Animation -----------------------------
    Animator animator;

    // ParticleSystem
    public ParticleSystem smokeEffect;


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
        // Projectile
        if(!broken)
        {
            return;
        }


        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

    }

    void FixedUpdate()
    {
        // Projectile
        if(!broken)
        {
            return;
        }


        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;

            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
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

    // Projectile
    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false;

        animator.SetTrigger("Fixed");

        // ParticleSystem. 
        // Using .Stop because it simply stops the Particle System from creating particles, and the particles that already exist can finish their lifetime normally. 
        smokeEffect.Stop();
        // Destroy(smokeEffect.gameObject);
    }




}
