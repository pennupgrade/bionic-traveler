using System.Collections;
using System.Linq;
using BionicTraveler.Scripts;
using BionicTraveler.Scripts.Interaction;
using BionicTraveler.Scripts.World;
using Items;
using UnityEngine;

/// <summary>
/// Player Entity class.
/// </summary>
public class PlayerEntity : DynamicEntity
{
    private const float MoveSpeed = 10;
    private const float DashDist = 10;
    private const float DashCooldown = 1;
    private float interactionRange = 100;
    private bool dashEnabled = true;

    /// <summary>   
    /// Gets or sets the BodyPart to activate with PrimaryBP input.
    /// </summary>
    public BodyPart PrimaryBP { get => this.primaryBP; set => this.primaryBP = value; }

    /// <summary>
    /// Gets or sets the BodyPart to activate with SecondaryBP input.
    /// </summary>
    public BodyPart SecondaryBP { get; set; }

    /// <summary>
    /// Gets or sets the Player interaction range.
    /// </summary>
    public float InteractionRange { get => interactionRange; set => interactionRange = value; }

    // TODO: Uncomment these after merging Combat classes
    // private Weapon PrimaryWeapon;
    // private Weapon SecondaryWeapon;
    // private List<Chip> ActiveChips = new List<Chip>();

    public enum MovementState
    {
        Normal,
        Traverse,
    }

    private MovementState MoveState = MovementState.Normal;
    private BodyPart primaryBP;
    private int batteryHealth = 50;

    /// <summary>
    /// Deals an amount of damage to the player (for testing purposes).
    /// </summary>
    /// <param name="damage">The amount of damage to deal.</param>
    public void DamageBattery(int damage)
    {
        this.batteryHealth = Mathf.Max(0, this.batteryHealth - damage);
        Debug.Log(this.GetHealth());
    }

    /// <summary>
    /// Heals the player's battery health to full (for testing purposes).
    /// </summary>
    public void HealBattery()
    {
        this.batteryHealth = 50;
        Debug.Log(this.GetHealth());
    }

    //private Inventory PlayerInventory;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Fixed Update is called at fixed intervals in real time
    void FixedUpdate()
    {
        if (this.Stunned)
        {
            return;
        }

        if (Input.GetButtonDown("Dash")) StartCoroutine(nameof(Dash));
        switch (MoveState)
        {
            case MovementState.Normal:
                Move();
                break;
            default:
                return;
        }

        // TODO: This is inefficient in an open world, refine later; Create helper function to find objects of a certain type
        var interactables = GameObject.FindGameObjectsWithTag("Interactable").Where(
            interactable => Vector3.Distance(
                interactable.transform.position, this.transform.position) < this.interactionRange).ToArray();

        foreach (var a in interactables)
        {
            //a.GetComponent<IInteractable>().OnInteract(this.gameObject);
        }

        //player pickup
        var pickups = GameObject.FindObjectsOfType<Pickup>().Where(pickup => Vector3.Distance(
            pickup.transform.position,
            this.transform.position) < this.interactionRange).ToArray();

        foreach (var a in pickups)
        {
            a.PickUp(this);
        }

        //press key to show inventory data
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(this.Inventory.ToString());
        }

        //Drink Health Potion
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var item in Inventory.Items)
            {
                this.Inventory.Use(item);
            }
        }
    }

    private void Move()
    {
        Rigidbody2D body = this.GetComponent<Rigidbody2D>();
        Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        if (!inputDirection.Equals(Vector2.zero)) Direction = inputDirection;
        body.velocity = Vector2.Lerp(inputDirection * MoveSpeed, Vector2.zero, 0.5f);
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
        if (!inputDirection.Equals(Vector2.zero)) Direction = inputDirection;
        dashEnabled = false;
        for (int i = 0; i < DashDist; i++)
        {
            body.MovePosition(body.position + inputDirection);
        }
        yield return new WaitForSeconds(DashCooldown);
        dashEnabled = true;
    }



}