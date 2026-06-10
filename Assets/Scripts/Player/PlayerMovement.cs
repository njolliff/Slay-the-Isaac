using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Movement Variables
    public float acceleration = 1, maxVelocity = 1;
    private Vector2 movementInput;
    #endregion

    #region Components
    [SerializeField] private Rigidbody2D rb;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Calculate target velocity
        Vector2 targetVelocity = movementInput * maxVelocity;
        
        // Get current velocity and calculate delta (difference between target and current velocity)
        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 delta = targetVelocity - currentVelocity;

        // Apply more force the greater the delta (to change directions faster)
        Vector2 force = delta / Time.fixedDeltaTime * acceleration;
        rb.AddForce(force, ForceMode2D.Force);
    }

    #region Input Methods
    public void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
    public void OnNorthCard(InputValue inputValue)
    {
        Debug.Log("North Card used.");
    }
    public void OnSouthCard(InputValue inputValue)
    {
        Debug.Log("South Card used.");
    }
    public void OnEastCard(InputValue inputValue)
    {
        Debug.Log("East Card used.");
    }
    public void OnWestCard(InputValue inputValue)
    {
        Debug.Log("West Card used.");
    }
    #endregion
}