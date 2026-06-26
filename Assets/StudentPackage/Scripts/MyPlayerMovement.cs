using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkStudy.Student
{
    public class MyPlayerMovement : NetworkBehaviour
    {

        [Tooltip("초당 이동 속도(월드 유닛).")]
        [SerializeField]
        private float m_MoveSpeed = 5f;

        [Tooltip("초당 회전 속도(도).")]
        [SerializeField]
        private float m_RotateSpeed = 120f;

        [SerializeField]
        private float m_JumpSpeed = 5f;

        private Rigidbody m_RigidBody;
        private bool m_IsGrounded;
        private bool m_IsMoved;


        public override void OnNetworkSpawn()
        {
            m_RigidBody = GetComponent<Rigidbody>();

            if (IsOwner)
            {
                Debug.Log($"[MyPlayerMovement] 내 플레이어 스폰 {OwnerClientId}");
            }
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null )
            {
                return;
            }

            float move = 0f;
            float turn = 0f;

            if (keyboard.wKey.isPressed)
            {
                m_IsMoved = true;
                move += 1f;
            }


            if (keyboard.sKey.isPressed)
            {
                m_IsMoved = true;
                move += 1f;
            }

            if (keyboard.dKey.isPressed)
            {
                m_IsMoved = true;
                turn += 1f;
            }


            if (keyboard.aKey.isPressed)
            {
                m_IsMoved = true;
                turn += -1f;
            }


            transform.Rotate(0f, turn * m_RotateSpeed * Time.deltaTime, 0f);

            float currentSpeed = keyboard.leftShiftKey.isPressed ? m_MoveSpeed * 2f : m_MoveSpeed;
            transform.Translate(0f, 0f, move * currentSpeed * Time.deltaTime);

            if (keyboard.spaceKey.wasPressedThisFrame && m_IsGrounded)
            {
                m_RigidBody.AddForce(Vector3.up * m_JumpSpeed, ForceMode.Impulse);
            }

        }

        private void OnCollisionStay(Collision collision)
        {
            m_IsGrounded = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            m_IsGrounded = false;
        }
    }
}
