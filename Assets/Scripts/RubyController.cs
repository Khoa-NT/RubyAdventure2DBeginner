using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 4.0f;

    // Define maximum health
    public int maxHealth = 5;
    
    public int health { get { return currentHealth; } }
    int currentHealth;


    // Ghost mode interval
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;



    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    // Projectile
    public GameObject projectilePrefab;
    public float launchForce = 300f;


    // ----------------------------- For Animation -----------------------------
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    
    // ----------------------------- For Audio -----------------------------
    AudioSource audioSource;
    public AudioClip throwClip;
    public AudioClip hitClip;
    public AudioClip FootStepClip;


    // Start is called before the first frame update
    void Start()
    {
        // Timing and Framerate
        // Will make the game only 10 fps
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 10;

        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        // currentHealth = 1;

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {
        // ----------------------------- Manual -----------------------------
        // // Get the coordinate of object. 
        // // transform.position return (x, y, z) coordinate
        // // Because we using 2D so Vector2 will copy (x, y) for us
        // Vector2 position = transform.position; 

        // // Move the object to the right with 0.1 units
        // position.x = position.x + 0.1f;

        // // Assign it back to transform
        // transform.position = position;

        // ----------------------------- Read input -----------------------------
        // Get the value from user input
        // float horizontal = Input.GetAxis("Horizontal");
        // float vertical = Input.GetAxis("Vertical");

        // Just print it
        // Debug.Log(horizontal);

        // Modify transform
        // Vector2 position = transform.position;
        // position.x = position.x + 0.1f * horizontal;
        // position.y = position.y + 0.1f * vertical;
        // transform.position = position;

        // ----------------------------- Movement in Units per Second -----------------------------
        // float horizontal = Input.GetAxis("Horizontal");
        // float vertical = Input.GetAxis("Vertical");

        // Vector2 position = transform.position;

        // // Time.deltaTime return time per frame. It's mean we moving 0.3 units per second
        // position.x = position.x + 3.5f * horizontal * Time.deltaTime;
        // position.y = position.y + 3.5f * vertical * Time.deltaTime;
        // transform.position = position;



        // ----------------------------- Read Input -----------------------------
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // ----------------------------- Animation -----------------------------
        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();


            if (!audioSource.isPlaying)
            {
                PlaySound(FootStepClip);
            }

        }
        else
        {
            audioSource.Stop();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);


        // ----------------------------- Ghost time -----------------------------
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        
        // ----------------------------- Ghost time -----------------------------
        // Input settings (Edit > Project Settings > Input). For an example, take a look at the Axes > Fire1 
        // if (Input.GetAxis("Fire1") != 0) #if (Input.GetMouseButtonUp(0))
        // {
        //     Launch();
        // }

        // Use Key C
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }


        // Raycasting
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Ratcast( start_point, direection vector, maximum distance, layer Mask name, minDepth included, maxDepth included)
            // rigidbody2d.position + Vector2.up * 0.2f: Get the position of Ruby and move that point to the center
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }




    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }


    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            // ----------------------------- Animation -----------------------------
            animator.SetTrigger("Hit");


            // ----------------------------- Ghost time -----------------------------
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitClip);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        Debug.Log(currentHealth + "/" + maxHealth);
        UIHealthBar.instance.SetValue(currentHealth/ (float)maxHealth);
    }


    void Launch()
    {
        // Create a copy of projectile object by using Instantiate(Reference object, Position, Rotation)
        // Read the position of player and move it up so that it's near ruby hand, not her feet. Vector2.up = Vector2 (0,1) #(X,Y)
        // Quaternion represents for rotation (https://docs.unity3d.com/ScriptReference/Quaternion.html). 
        // Quanternion.identity means 'no rotation'
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity); 

        // Get the Projectile.cs script inside the projectileObject
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        // Run the Launch function with input launchForce force field (Newton units).
        projectile.Launch( lookDirection, launchForce);
        
        // Trigger Ruby to Launch animation
        animator.SetTrigger("Launch");
        
        PlaySound(throwClip);

    }



    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }



}
