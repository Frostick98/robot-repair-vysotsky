using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    Animator animator;

    public InputAction MoveAction;
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    public GameObject projectilePrefab;
    public float timeInvincible = 2.0f;
    public InputAction LaunchAction;

    int currentHealth = 1;
    bool isInvincible;
    float damageCooldown;

    Vector2 move;
    Vector2 moveDirection = new Vector2(1, 0);
    public float speed = 3.0f;

    void Start()
    {
        LaunchAction.Enable();
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        //currentHealth = 1;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        if (!Mathf.Approximately(move.x, 0.0f)
            || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        //Debug.Log(move);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (LaunchAction.WasPressedThisFrame())
        {
            Launch();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position
            + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }
    
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }
        currentHealth = Mathf.Clamp(currentHealth + amount,
        0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab,
        rigidbody2d.position + Vector2.up * 0.5f,
        Quaternion.identity);
        Projectile projectile =
        projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
    }
}
