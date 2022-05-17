using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxManager : MonoBehaviour
{

    public static BoxManager instance = null;

    public LayerMask boxLayer;
    public bool canInteractWithBoxes = true;
    public float boxDragForce = 3f;
    public Vector2 boxDamageableAreaMinBounds;
    public Vector2 boxDamageableAreaMaxBounds;
    public RectTransform damageAreaCanvas;

    private Camera cam;
    private ControlScheme controls;

    private bool mouseClicked = false;
    private Vector2 mousePrevPos, mouseCurrentPos, mouseForce;
    
    private Box heldBox;
    private Rigidbody2D heldBoxRb;
    private Vector3 refBoxPos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 a = transform.position + Vector3.up * boxDamageableAreaMinBounds.y + Vector3.right * boxDamageableAreaMinBounds.x;
        Vector3 b = transform.position + Vector3.up * boxDamageableAreaMinBounds.y + Vector3.right * boxDamageableAreaMaxBounds.x;
        Vector3 c = transform.position + Vector3.up * boxDamageableAreaMaxBounds.y + Vector3.right * boxDamageableAreaMaxBounds.x;
        Vector3 d = transform.position + Vector3.up * boxDamageableAreaMaxBounds.y + Vector3.right * boxDamageableAreaMinBounds.x;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        controls = new ControlScheme();
        controls.General.Click.performed += ctx => MouseDown();
        controls.General.Click.canceled += ctx => MouseUp();

        damageAreaCanvas.position = transform.position +
                                                Vector3.right * ((boxDamageableAreaMinBounds.x +
                                                                  boxDamageableAreaMaxBounds.x) / 2) +
                                                Vector3.up * ((boxDamageableAreaMinBounds.y +
                                                               boxDamageableAreaMaxBounds.y) / 2);
        damageAreaCanvas.sizeDelta = new Vector2(Mathf.Abs(boxDamageableAreaMinBounds.x) + Mathf.Abs(boxDamageableAreaMaxBounds.x),
            Mathf.Abs(boxDamageableAreaMinBounds.y) + Mathf.Abs(boxDamageableAreaMaxBounds.y));
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void MouseDown()
    {
        mouseClicked = true;
    }

    private void MouseUp()
    {
        mouseClicked = false;
        DropBox();
    }

    
    // Update is called once per frame
    void Update()
    {
        if(!canInteractWithBoxes) return;


        mouseCurrentPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseForce = (mouseCurrentPos - mousePrevPos) / Time.deltaTime;
        mouseForce = Vector3.ClampMagnitude(mouseForce, 10f);
        mousePrevPos = mouseCurrentPos;
        
        if (heldBox != null)
        {
            Vector3 heldBoxPrevPos = heldBox.transform.position;
            Vector3 heldBoxTargetPos = new Vector3(mouseCurrentPos.x, mouseCurrentPos.y, heldBoxPrevPos.z);
            heldBoxRb.velocity = boxDragForce * Time.deltaTime * (heldBoxTargetPos - heldBoxPrevPos);
        }
        else
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 200f);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 100f, boxLayer);
            if (hit.collider != null && mouseClicked)
            {
                if (hit.transform.TryGetComponent<Box>(out Box b))
                {
                    if (b != null)
                    {
                        b.PickedUp();
                        PickUpBox(b);
                    }
                }
            }

        }
    }
    

    private void PickUpBox(Box b)
    {
        heldBox = b;
        heldBoxRb = heldBox.GetComponent<Rigidbody2D>();
    }
    
    private void DropBox()
    {
        if (heldBox == null) return;
        
        heldBox.Dropped();
        heldBoxRb.AddForce(mouseForce, ForceMode2D.Impulse);
        heldBox = null;
        heldBoxRb = null;
    }
}
