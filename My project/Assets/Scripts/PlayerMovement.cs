using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //please let this be the last time i have to edit this script
    //made by Aleksie
    //if anything in here breaks uhhh idk good luck

    //input variables
    private float horizontalInput;
    private float verticalInput;

    //movement related variables
    //moveSpeed and jumpForce are the changeable ones
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    private Rigidbody2D rb;
    private bool isGrounded;

    //vine climbing variables
    //vine speed and vine slide down speed are changeable
    //onVine is for debugging purposes
    [Header("Vine Climbing")]
    public bool onVine = false;
    public float vineClimbSpeed = 4f;
    public float vineSideSpeed = 6f;

    //ledge grab variables
    //ledgeCheck can be modified in inspector, just move it around
    //ledgeOffset and vaultOffset are also modifiable, but I would leave them alone unless necessary
    //ledgeGrabDistance is how far the player can be from the ledge to grab it, also modifiable, higher number is less precise needed to grab
    //boxCastSize is the size of the boxcast used to detect ledges, also modifiable but I got no clue how it works so yeah
    [Header("Ledge Grab")]
    public Transform ledgeCheck;
    public Vector2 ledgeOffset = new Vector2(0.5f, -0.3f);
    public Vector2 vaultOffset = new Vector2(0.7f, 1.2f);
    public float ledgeGrabDistance = 0.3f;
    private bool isHanging = false;
    private bool canGrabLedge = true;
    public Vector2 boxCastSize = new Vector2(0.2f, 0.4f);
    private Vector2 ledgePos; // position of the ledge we grabbed, set by raycast

    //check if we're grounded, changed from Tag to Layer for raycasting
    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    //ui manager reference for updating score and health
    UIManager uiManager;
    bool canTakeDamage = true; //damage cooldown

    //basic initialization
    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>(); //too lazy to drag it in rn
        if (uiManager == null)
        {
            Debug.LogError("Add a UI manager to the scene, bruh");
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //check our inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //handle all movement
        HandleFlip(); //flip player based on movement direction
        CheckGround(); //check if we're on the ground
        HandleJump(); //jumping if we're on the ground
        HandleLedgeGrab(); //check for ledge grab

        //only allow movement if we're not hanging
        if (!isHanging)
        {
            if (onVine) HandleVineMovement(); //if we are on a vine, climb it
            else HandleHorizontalMovement(); //normal horizontal movement if nothing else is happening
        }

        HandleLedgeInputs(); //if we are hanging, wait for vault or drop input
    }

    //flip player based on movement direction
    //pretty simple
    void HandleFlip()
    {
        if (horizontalInput > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        else if (horizontalInput < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    //handle regular schmegular horizontal movement
    void HandleHorizontalMovement()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    //handle jumping logic
    void HandleJump()
    {
        //if we press jump
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            //check if we are on the ground or on a vine
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (onVine)
            {
                //this part doesnt work cause it was messing with the vine climbing
                //if anyone can get this to work properly where you can jump off the vine that would be sick
                //but probably not worth the effort ngl
                onVine = false; // jump off vine
                rb.gravityScale = 3f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    //handle vine climbing movement, both vertical and horizontal
    void HandleVineMovement()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(horizontalInput * vineSideSpeed, verticalInput * vineClimbSpeed);
    }

    //check if we're on the ground using OverlapCircle
    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        //if we are grounded and not on a vine, reset gravity scale
        if (isGrounded && !onVine)
            rb.gravityScale = 3f;
    }


    // LEDGE GRAB LOGIC BELOW

    //handle ledge grabbing
    void HandleLedgeGrab()
    {
        //if we are already on a ledge or if we are on a vine, do nothing
        if (isHanging || onVine) return;

        //boxcast for ledge to be more reliable
        RaycastHit2D hit = Physics2D.BoxCast(
            ledgeCheck.position, boxCastSize, 0f, Vector2.right * Mathf.Sign(transform.localScale.x), ledgeGrabDistance, groundLayer);

        // If there is a ledge there and can grab it, grab it
        if (hit.collider != null)
        {
            //if we would like the player to only grab when falling, change this condition
            //something like "&& rb.linearVelocity.y < 0f"
            if (canGrabLedge)
            {
                ledgePos = hit.point;
                StartLedgeGrab();
            }
        }
    }

    //start ledge grab process
    void StartLedgeGrab()
    {
        //we are hanging, turn off all other physics
        isHanging = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        //set our hang position based on ledge position and offsets set above; tweak in inspector if needed; if it breaks the defaults are in code just reset to that
        Vector2 hangPos = ledgePos + new Vector2(-transform.localScale.x * ledgeOffset.x, ledgeOffset.y);
        transform.position = hangPos;
    }

    //handle inputs while hanging on a ledge
    void HandleLedgeInputs()
    {
        //if we're not hanging, do nothing
        if (!isHanging) return;

        //check for vault or drop input
        if (Input.GetKeyDown(KeyCode.W)) // vault
        {
            VaultUp();
        }
        else if (Input.GetKeyDown(KeyCode.S)) // drop
        {
            DropFromLedge();
        }
    }

    //vault up from ledge
    void VaultUp()
    {
        //end hanging state and reset physics
        isHanging = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;

        //move player up and forward based on vault offset, should probably be replaced with an animation at a later time
        transform.position = (Vector2)transform.position + new Vector2(transform.localScale.x * vaultOffset.x, vaultOffset.y);

        //ledge grab cooldown to prevent immediate re-grab
        canGrabLedge = false;
        Invoke(nameof(ResetLedgeGrab), 0.5f);
    }

    //drop down from ledge
    void DropFromLedge()
    {
        //stop hanging and reset physics
        isHanging = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        canGrabLedge = false;

        //cooldown so we dont grab the same ledge again
        Invoke(nameof(ResetLedgeGrab), 0.5f);
    }

    //reset ledge grab ability after cooldown
    void ResetLedgeGrab()
    {
        canGrabLedge = true;
    }

    // COLLISION HANDLING BELOW
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //climb vine
        if (collision.CompareTag("Vine"))
        {
            onVine = true;
            rb.linearVelocity = Vector2.zero;
        }

        //collect coin
        if (collision.CompareTag("Coin"))
        {
            uiManager.UpdateScore(1);
            Destroy(collision.gameObject);
        }

        //collect heart
        if (collision.CompareTag("Heart"))
        {
            uiManager.UpdateHealth(1);
            Destroy(collision.gameObject);
        }
    }


    //we stay on vine as long as we're inside the trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        //climb vine
        if (collision.CompareTag("Vine"))
        {
            if (verticalInput > 0)
            {
                onVine = true;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    //exit vine
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Vine"))
        {
            onVine = false;
            rb.gravityScale = 3f;
        }
    }

    //ground check every time we collide with something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if we hit an enemy, lose health
        if (collision.gameObject.CompareTag("Enemy") && canTakeDamage)
        {
            uiManager.UpdateHealth(-1);

            //grant temporary invincibility
            canTakeDamage = false;
            Invoke("ResetDamageCooldown", 1.5f);
            isGrounded = true; //let the player jump again immediately after getting hit
        }
    }

    //reset damage cooldown
    private void ResetDamageCooldown()
    {
        canTakeDamage = true;
    }

    // GIZMOS FOR CHECKING POSITIONS
    private void OnDrawGizmos()
    {
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ledgeCheck.position, 0.1f);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}