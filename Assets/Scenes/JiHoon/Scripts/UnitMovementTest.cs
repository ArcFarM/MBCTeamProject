using UnityEngine;
namespace JiHoon
{
    public class UnitMovement : MonoBehaviour
    {
        [SerializeField] float speed = 3f;
        Rigidbody2D rb;
        Vector2 input;
        bool canMove = true;

        void Awake() => rb = GetComponent<Rigidbody2D>();

        void Update()
        {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        void FixedUpdate()
        {
            // 테스트용: 항상 오른쪽(1,0) 방향으로만 이동
            Vector2 moveDir = Vector2.right;
            rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                canMove = false;
        }

        void OnCollisionExit2D(Collision2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                canMove = true;
        }
    }
}
