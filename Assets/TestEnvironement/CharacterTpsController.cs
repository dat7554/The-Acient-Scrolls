using UnityEngine;

public class CharacterTpsController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform mainCamera;

    [Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float maxSlopeAngle = 55f;

    [Header("CAPSULE COLLIDER")]
    [SerializeField] private CapsuleCollider capsule;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float skinWidth = 0.1f;

    private float rotationSmoothVelocity;
    private Vector3 gravity = new Vector3(0, -9.8f, 0);
    private bool isGrounded;

    private void Awake()
    {
        capsule = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        Vector3 inputDirection = GetMoveDirection();
        if (inputDirection.magnitude > 0.1f)
        {
            Rotate(inputDirection);
        }
        Move(inputDirection * moveSpeed);
    }

    //get ZQSD keyboard axis values -1,0,1 values on Horizontal/vertical
    private Vector3 GetMoveDirection()
    {
        Vector3 camForward = new Vector3(mainCamera.forward.x, 0, mainCamera.forward.z).normalized;
        Vector3 camRight = new Vector3(mainCamera.right.x, 0, mainCamera.right.z).normalized;
        Vector3 inputDir = Input.GetAxisRaw("Horizontal") * camRight + Input.GetAxisRaw("Vertical") * camForward;
        return inputDir.normalized;
    }

    //move the character in the input direction and apply gravity when needed
    private void Move(Vector3 moveAmount)
    {
        Vector3 gravityAmount = gravity * Time.fixedDeltaTime;
        moveAmount = moveAmount * Time.fixedDeltaTime;
        moveAmount = CollideAndSlide(moveAmount, moveAmount, transform.position, false, 0);
        moveAmount += CollideAndSlide(gravityAmount, gravityAmount, transform.position + moveAmount, true, 0);
        transform.position += moveAmount;
    }

    //find the rotation toward's input direction based on where the camera is facing
    private void Rotate(Vector3 direction)
    {
        float smoothAngle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg,
            ref rotationSmoothVelocity,
            rotationSmoothTime
        );
        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
    }

    //handle bumping into object and calulate the updated direction after impact
    private Vector3 CollideAndSlide(Vector3 originalVelocity, Vector3 dynamicVelocity, Vector3 position, bool gravityPass, int depth)
    {
        if (depth >= 3)
        {
            return Vector3.zero;
        }

        RaycastHit hit = CastInDirection(position, dynamicVelocity);

        //early return if we didn't hit anything
        if (hit.collider == null)
        {
            //convert the direction based on the surface we are standing on
            if (gravityPass)
            {
                isGrounded = false;
            }
            return dynamicVelocity;
        }

        //cut the vector in 2: before and past the hit point
        Vector3 distanceToSurface = dynamicVelocity.normalized * (hit.distance - skinWidth);
        Vector3 distancePastSurface = dynamicVelocity - distanceToSurface;
        float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

        //prevent overlap when the movement would be within the skin width
        if (distanceToSurface.magnitude <= skinWidth)
        {
            distanceToSurface = Vector3.zero;
        }

        //gentle slope
        if (slopeAngle <= maxSlopeAngle)
        {
            if (gravityPass)
            {
                isGrounded = true;
                return distanceToSurface;
            }
            distancePastSurface = Vector3.ProjectOnPlane(distancePastSurface, hit.normal);

            //wall or steep slope
        }
        else
        {
            if (!gravityPass && isGrounded)
            {

                // find the sliding direction (left/right) when walking in a wall or steep slope 
                distancePastSurface = Vector3.ProjectOnPlane(
                    new Vector3(distancePastSurface.x, 0, distancePastSurface.z),
                    new Vector3(hit.normal.x, 0, hit.normal.z)
                );
            }

            if (gravityPass)
            {
                //adjusting falling direction to slide on the steep slope
                distancePastSurface = Vector3.ProjectOnPlane(distancePastSurface, hit.normal);
            }
        }
        return distanceToSurface + CollideAndSlide(originalVelocity, distancePastSurface, position + distanceToSurface, gravityPass, depth + 1);
    }


    //cast the capsule collider in the movment direction to find collisions
    public RaycastHit CastInDirection(Vector3 position, Vector3 velocity)
    {
        RaycastHit hit;
        Physics.CapsuleCast(
            position + Vector3.up * (capsule.height - capsule.radius),
            position + Vector3.up * capsule.radius,
            capsule.radius - skinWidth,
            velocity,
            out hit,
            velocity.magnitude + skinWidth,
            collisionLayer);
        return hit;
    }
}
