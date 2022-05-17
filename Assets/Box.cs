using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Box : MonoBehaviour
{
    public string code;

    private float health = 3;
    public bool canTakeDamage = true;
    [SerializeField] private TMP_Text collisionText;

    [SerializeField] private float weight = 1;
    public float Weight => weight;

    [SerializeField]
    private float capacity;

    [SerializeField] private float capacityExceededDamageTime = 1f;

    private float currentStackWeight;

    private Rigidbody2D rb;

    private bool grounded = false;
    private const float COLLISION_FORCE_THRESHOLD = 20f;

    private List<Collider2D> collidersStackedAbove = new List<Collider2D>();
    private List<Collider2D> collidersStackedBelow = new List<Collider2D>();

    [SerializeField] private SpriteRenderer gfx;

    private Vector3 newPos, oldPos;

    private bool capacityExceeded = false;
    private float time = 0;
    private Color origGFXColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collisionText.text = "" + health;
        time = capacityExceededDamageTime;
        origGFXColor = gfx.color;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        float dir = Vector3.Dot(Vector3.up, col.GetContact(0).normal);
        if (!collidersStackedAbove.Contains(col.collider) && dir <= -0.707f)
        {
            collidersStackedAbove.Add(col.collider);
        }
        else if (!collidersStackedBelow.Contains(col.collider) && dir >= 0.707f)
        {
            collidersStackedBelow.Add(col.collider);
            if (col.collider.TryGetComponent(out Box b))
            {
                b.CalculateStackWeight(currentStackWeight + Weight);
            }
        }

        if (col.relativeVelocity.sqrMagnitude > COLLISION_FORCE_THRESHOLD)
        {
            TakeDamage();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (collidersStackedAbove.Contains(other.collider))
        {
            collidersStackedAbove.Remove(other.collider);
        }

        if (collidersStackedBelow.Contains(other.collider))
        {
            collidersStackedBelow.Remove(other.collider);
            if (other.collider.TryGetComponent(out Box b))
            {
                b.CalculateStackWeight(0);
            }
        }
    }

    public void CalculateStackWeight(float weightAbove)
    {

        currentStackWeight = weightAbove;
        if (currentStackWeight > capacity)
        {
            capacityExceeded = true;
        }
        else
        {
            capacityExceeded = false;
            gfx.color = origGFXColor;
        }

        foreach (var c in collidersStackedBelow)
        {
            if (c.TryGetComponent(out Box b))
            {
                b.CalculateStackWeight(currentStackWeight + Weight);
            }
        }
    }
    
    private void Update()
    {
        //TODO: Keeping track of colliders that are in contact with box
        //TODO: Figure out stack mechanics, should the box instantly lose health when its weight is exceeded?
        //TODO: How to calculate stacks, counting boxes above current box but not below?

        if (!capacityExceeded) return;
        
        time += Time.deltaTime;
        if (time >= capacityExceededDamageTime)
        {
            TakeDamage();
            time = 0;
        }

        gfx.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 20f, 1));

    }


    public void PickedUp()
    {
        rb.gravityScale = 0f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Dropped()
    {
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.None;
    }

    public void TakeDamage()
    {
        if (!canTakeDamage) return;
        Vector2 minBounds = BoxManager.instance.boxDamageableAreaMinBounds;
        Vector2 maxBounds = BoxManager.instance.boxDamageableAreaMaxBounds;
        if (transform.position.x < minBounds.x || transform.position.y < minBounds.y ||
            transform.position.x > maxBounds.x || transform.position.y > maxBounds.y)
        {
            return; //Out of box damageable area
        }

        health -= 1;
        if (health < 0)
            Destroy(gameObject);
        switch (health)
        {
            case 2: //Switch Sprites
                break;
            case 1: //Switch Sprites
                break;
            case 0:
                Destroy(gameObject);
                break;
            default: //Play sound?
                break;
        }

        collisionText.text = "" + health;
    }
}