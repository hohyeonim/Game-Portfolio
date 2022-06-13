using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static public bool isActivated = true;

    // ���ǵ� ���� ����
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float swimSpeed;
    [SerializeField]
    private float swimFastSpeed;
    [SerializeField]
    private float upSwimSpeed;

    [SerializeField]
    private float jumpForce;

    // ���� ����
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    // ������ üũ ����
    private Vector3 lastPos;

    // �ɾ����� �󸶳� ������ �����ϴ� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // �� ���� ����
    private CapsuleCollider capsuleCollider;

    // ī�޶� �ΰ���
    [SerializeField]
    private float lookSensitivity;

    // ī�޶� �Ѱ�
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Crosshair>();
        theStatusController = FindObjectOfType<StatusController>();

        // �ʱ�ȭ
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        if (isActivated && GameManager.canPlayerMove)
        {
            WaterCheck();
            IsGround();
            TryJump();
            if (!GameManager.isWater)
            {
                TryRun();
            }
            TryCrouch();
            Move();
            CameraRotation();
            CharacterRotation();
        }
    }

    // �� �� üũ(������ ����)
    private void WaterCheck()
    {
        if (GameManager.isWater)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                applySpeed = swimFastSpeed;
            }
            else
            {
                applySpeed = swimSpeed;
            }
        }
    }

    // ���� ��ҳ�?(���� üũ)
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.3f);
        theCrosshair.JumpingAnimation(!isGround);
    }

    // ���� �õ�
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true && theStatusController.GetCurrentSP() > 0 && !GameManager.isWater)
        {
            Jump();
        }
        else if (Input.GetKey(KeyCode.Space) && GameManager.isWater)
        {
            UpSwim();
        }
    }

    // ���� ���� ����
    private void UpSwim()
    {
        myRigid.velocity = transform.up * upSwimSpeed;
    }

    // ����
    private void Jump()
    {
        if (isCrouch)
            Crouch();

        theStatusController.DecreaseStamina(100);
        myRigid.velocity = transform.up * jumpForce;
    }

    // �޸��� �õ�
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
        {
            RunningCancel();
        }
    }

    // �ɱ� �õ�
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    // �ɱ�
    private void Crouch()
    {
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());
    }

    // �ɱ� �ӵ�(�ڷ�ƾ)(ī�޶� �ӵ�)
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.1f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    // �޸���
    private void Running()
    {
        if (isCrouch)
            Crouch();

        theGunController.CancelFineSight();

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        theStatusController.DecreaseStamina(1);
        applySpeed = runSpeed;
    }

    // �޸��� ���(�ȱ�)
    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    // ������
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

        if (!isRun && !isCrouch && isGround)
        {
            if (_velocity.magnitude >= 0.01f)
            {
                isWalk = true;
            }
            else
            {
                isWalk = false;
            }

            theCrosshair.WalkingAnimation(isWalk);

            lastPos = transform.position;
        }
    }

    // ī�޶� ȸ��(����)
    private void CameraRotation()
    {
        if (!PauseCameraRotation)
        {
            float _xRotation = Input.GetAxisRaw("Mouse Y");
            float _cameraRotationX = _xRotation * lookSensitivity;
            currentCameraRotationX -= _cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    private bool PauseCameraRotation = false;

    // ���� ���� �ڵ� �ü� ó��
    public IEnumerator TreeLookCoroutine(Vector3 _target)
    {
        PauseCameraRotation = true;

        // ��� ��ġ - ���� ī�޶� ��ġ
        Quaternion direction = Quaternion.LookRotation(_target - theCamera.transform.position);
        // ī�޶� ���� ������
        Vector3 eulerValue = direction.eulerAngles;
        // ������ x�� ��
        float destinationX = eulerValue.x;

        // ������ x�� - ����x ���� 0.5���� Ŀ�� ���� �ݺ�
        while (Mathf.Abs(destinationX - currentCameraRotationX) >= 0.5f)
        {
            // ������x ������ ī�޶� �̵�
            eulerValue = Quaternion.Lerp(theCamera.transform.localRotation, direction, 0.3f).eulerAngles;
            theCamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f);
            currentCameraRotationX = theCamera.transform.localEulerAngles.x;
            yield return null;
        }

        PauseCameraRotation = false;
    }


    // ĳ���� ȸ��(�¿�)
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    public bool GetRun()
    {
        return isRun;
    }
}
