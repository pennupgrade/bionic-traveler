using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Items;
using UnityEditor.Tilemaps;
using UnityEngine;


public class PlayerEntity : DynamicEntity
{
    private const float MoveSpeed = 10;
    private const float DashDist = 10;
    private const float DashCooldown = 1;
    private bool _dashEnabled = true;
    public BodyPart PrimaryBodyPart { set; get; }

    public BodyPart SecondaryBodyPart { set; get; }

    //private Weapon PrimaryWeapon;
    //private Weapon SecondaryWeapon;
    //private List<Chip> ActiveChips = new List<Chip>();
    public enum MovementState
    {
        Default,
        Stun,
        Traverse,
    }

    private MovementState MoveState = MovementState.Default;

    //private Inventory PlayerInventory;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_stunned) return;
        if (Input.GetButtonDown("Dash") && _dashEnabled) StartCoroutine(nameof(Dash));
        switch (MoveState)
        {
            case MovementState.Default:
                Move();
                break;
            default:
                return;
        }
    }

    private void Move()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (!inputDirection.Equals(Vector2.zero)) _direction = inputDirection;
        body.velocity = Vector2.Lerp(body.velocity, inputDirection * MoveSpeed, 0.5f);
    }

    public Transform GetTransform()
    {
        return GetComponent<Transform>();
    }

    public MovementState GetMovementState()
    {
        return MoveState;
    }

    private void ActivateAbility(BodyPart b)
    {
        b.ActivateAbility();
    }

    private void Interact()
    {

    }

    private IEnumerator Dash()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (!inputDirection.Equals(Vector2.zero)) _direction = inputDirection;
        _dashEnabled = false;
        for (int i = 0; i < DashDist; i++)
        {
            body.MovePosition(body.position + inputDirection);
        }
        yield return new WaitForSeconds(DashCooldown);
        _dashEnabled = true;
    }
}