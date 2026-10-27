using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 5f; // Lực nhảy (có thể chỉnh trong Inspector)
    public float timeToWait = 10f;

    private float idleTimer = 0f;
    private Animator anim;
    private Rigidbody rb; // Gọi Rigidbody để xử lý vật lý
    private bool isGrounded = true; // Biến kiểm tra chạm đất

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // --- KHI CÓ DI CHUYỂN ---
            idleTimer = 0f;
            anim.ResetTrigger("isAFK");

            bool isRunningInput = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isRunningInput ? runSpeed : walkSpeed;

            transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

            if (isRunningInput)
            {
                anim.SetBool("isRunning", true);
                anim.SetBool("isWalking", false);
            }
            else
            {
                anim.SetBool("isWalking", true);
                anim.SetBool("isRunning", false);
            }
        }
        else
        {
            // --- KHI ĐỨNG YÊN ---
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);

            idleTimer += Time.deltaTime;

            if (idleTimer >= timeToWait)
            {
                anim.SetTrigger("isAFK");
                idleTimer = 0f;
            }
        }

        // --- XỬ LÝ NHẢY ---
        // Bấm phím Space (Jump) và phải đang ở trên mặt đất
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Đẩy nhân vật lên
            anim.SetTrigger("isJumping"); // Bật hoạt ảnh nhảy

            isGrounded = false; // Đánh dấu là đang ở trên không

            idleTimer = 0f;
            anim.ResetTrigger("isAFK"); // Hủy AFK nếu đang nhảy
        }
    }

    // Hàm mặc định của Unity: Kiểm tra khi nhân vật chạm vào vật thể khác (mặt sàn)
    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true; // Chạm đất rồi, cho phép nhảy tiếp
    }
}