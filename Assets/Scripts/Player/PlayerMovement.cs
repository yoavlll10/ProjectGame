using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    PhotonView view;

    private void Awake()
    {
        //References for rigidbody and animator
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        
        horizontalInput = Input.GetAxis("Horizontal");
        //body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        //player direaction flip
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //if (Input.GetKey(KeyCode.UpArrow) && isGrounded())
        //    Jump();


        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
    
        //Wall jump logic
        if (wallJumpCooldown < 0.2f)//>>>>>>
        {


            body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 1;

            if (Input.GetKey(KeyCode.UpArrow)/* && isGrounded()*/)
                Jump();

        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
        
    }
    private void Jump()
    {
        
            if (isGrounded())
            {
                body.velocity = new Vector2(body.velocity.x, jumpSpeed);
                anim.SetTrigger("jump");
                //grounded = false;
            }
            else if (onWall() && !isGrounded())
            {
                if (horizontalInput == 0)
                {
                    body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                    transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                    body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

                wallJumpCooldown = 0;
            }
        
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag == "Ground")
         //   grounded = true;
    }
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
