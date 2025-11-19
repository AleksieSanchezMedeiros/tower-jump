using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    //jump and move
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float climbSpeed = 2.5f;

    //rb
    private Rigidbody2D rb;

    //check if touching floor
    public bool isGrounded;

    //check if touching vine
    public bool isOnVine;
    public bool insideVineZone;
    
    //damage cooldown
    public bool canTakeDamage = true;

    //ui manager reference
    UIManager uiManager;

    //Start is called before the first frame update
    private void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>(); //too lazy to drag it in rn
        if(uiManager == null)
        {
            Debug.LogError("Add a UI manager to the scene, bruh");
        }
        rb = GetComponent<Rigidbody2D>();
    }

    //Update is called once per frame
    private void Update()
    {
        //are we moving left or right
        float moveInput = Input.GetAxis("Horizontal");
        if(!isOnVine){

            //normal movement
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            //if we press jump and are grounded, jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }

            // Enter vine climbing ONLY when inside vine and pressing up
            if (insideVineZone && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)))
            {
                isOnVine = true;
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f;
            }
        
        }else{
            //vine climbing movement
            float vertical = Input.GetAxis("Vertical");

            // apply horizontal velocity even on vine
            float horizontal = moveInput * moveSpeed;

            //dragon ball fusion our velocities
            rb.linearVelocity = new Vector2(horizontal, vertical * climbSpeed);

            // Leave vine if player lets go of up AND not moving down
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && vertical == 0f)
            {
                isOnVine = false;
                rb.gravityScale = 3f;
            }

            // Jump off vine
            if (Input.GetButtonDown("Jump"))
            {
                isOnVine = false;
                rb.gravityScale = 3f;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            }
        
    }

    //ground check every time we collide with something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        //if we hit an enemy, lose health
        if(collision.gameObject.CompareTag("Enemy") && canTakeDamage)
        {
            uiManager.UpdateHealth(-1);
            
            //grant temporary invincibility
            canTakeDamage = false;
            Invoke("ResetDamageCooldown", 1.5f);
            isGrounded = true; //let the player jump again immediately after getting hit
        }
    }

    //check if we are leaving the ground
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    //check if we are in the vine
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Vine")) insideVineZone = true;
        
        //collect coin
        if(collision.CompareTag("Coin"))
        {
            uiManager.UpdateScore(1);
            Destroy(collision.gameObject);
        }

        //collect heart
        if(collision.CompareTag("Heart"))
        {
            uiManager.UpdateHealth(1);
            Destroy(collision.gameObject);
        }
    }

    //reset damage cooldown
    private void ResetDamageCooldown()
    {
        canTakeDamage = true;
    }

    //check if we are leaving the vine
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Vine"))
        {
            insideVineZone = false;
            isOnVine = false;
            rb.gravityScale = 3f;
        }
    }
}
