using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalArea : MonoBehaviour
{
    public BoxToSprite boxToSprite;
    
    public Vector2 goalAreaMinBounds;
    public Vector2 goalAreaMaxBounds;

    public LayerMask boxLayer;
    public Collider2D goalCollider;
    public Button shipBoxesButton;

    private List<Box> boxesInGoal = new List<Box>();

    private Collider2D[] contacts = new Collider2D[20];

    private Dictionary<string, int> goal = new Dictionary<string, int>();
    private Dictionary<string, int> request = new Dictionary<string, int>();
    private List<Collider2D> stack = new List<Collider2D>();

    private Vector3 boxCenter, boxExtents;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 a = transform.position + Vector3.up * goalAreaMinBounds.y + Vector3.right * goalAreaMinBounds.x;
        Vector3 b = transform.position + Vector3.up * goalAreaMinBounds.y + Vector3.right * goalAreaMaxBounds.x;
        Vector3 c = transform.position + Vector3.up * goalAreaMaxBounds.y + Vector3.right * goalAreaMaxBounds.x;
        Vector3 d = transform.position + Vector3.up * goalAreaMaxBounds.y + Vector3.right * goalAreaMinBounds.x;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }

    private void Awake()
    {
        boxCenter = transform.position + new Vector3((goalAreaMinBounds.x + goalAreaMaxBounds.x) / 2,
            (goalAreaMinBounds.y + goalAreaMaxBounds.y) / 2, transform.position.z);
        boxExtents = new Vector2(Mathf.Abs(goalAreaMinBounds.x) + Mathf.Abs(goalAreaMaxBounds.x),
            Mathf.Abs(goalAreaMinBounds.y) + Mathf.Abs(goalAreaMaxBounds.y));
        
        shipBoxesButton.onClick.AddListener(ShipBoxes);
        
        ResetGoalCalc();
    }

    private void Start()
    {
        Physics2D.queriesHitTriggers = false;
    }

    private void ResetGoalCalc()
    {
        goal.Clear();
        foreach (var box in boxToSprite.boxSprites)
        {
            goal.Add(box.boxCode, 0);
        }
    }
    

    private void ShipBoxes()
    {
        print("Shipping Boxes!");
        
        goalCollider.GetContacts(contacts);
        
        foreach (var st in contacts)
        {
            if (st == null)
                continue;

            ResetGoalCalc();
            stack.Clear();
            
            Debug.Log("Generating Stack Count from collider " + st, st.gameObject);
            GenerateStackCount(goal, stack, st);
            
            for (int i = 0; i < RequestManager.instance.requests.Count; i++)
            {
                Tuple<RequestManager.Request, RequestUI> t = RequestManager.instance.requests[i];
                Debug.Log("Checking goal for " + t.Item2.transform.name, t.Item2.gameObject);
                if (IsGoalMet(t.Item1.reqComputed, goal))
                {
                    Dictionary<string, int> rc = new Dictionary<string, int>();
                    foreach (var pair in t.Item1.reqComputed)
                    {
                        rc.Add(pair.Key, pair.Value);
                    }

                    foreach (var col in stack)
                    {
                        if (col.TryGetComponent(out Box b))
                        {
                            if (rc.ContainsKey(b.code))
                            {
                                if (rc[b.code] > 0)
                                {
                                    Destroy(col.gameObject);
                                    rc[b.code]--;
                                }
                            }
                        }
                    }

                    rc.Clear();
                    RequestManager.instance.RemoveRequest(t.Item1);
                    break;
                }
            }
        }
    }

    private void GenerateStackCount(Dictionary<string, int> stackComputed, List<Collider2D> stackCols, Collider2D firstCollider)
    {
        Vector3 boxCastStart = firstCollider.transform.position;
        Vector3 boxCastSize = firstCollider.bounds.size;
        RaycastHit2D[] boxStack = Physics2D.BoxCastAll(boxCastStart,
            boxCastSize, transform.eulerAngles.z, Vector3.up);
        Debug.DrawLine(boxCastStart + new Vector3(-boxCastSize.x / 2f, -boxCastSize.y / 2f), boxCastStart + new Vector3(boxCastSize.x / 2f, -boxCastSize.y / 2f), Color.yellow, 10f);
        Debug.DrawLine(boxCastStart + new Vector3(boxCastSize.x / 2f, -boxCastSize.y / 2f), boxCastStart + new Vector3(boxCastSize.x / 2f, boxCastSize.y / 2f), Color.yellow, 10f);
        Debug.DrawLine(boxCastStart + new Vector3(boxCastSize.x / 2f, boxCastSize.y / 2f), boxCastStart + new Vector3(-boxCastSize.x / 2f, boxCastSize.y / 2f), Color.yellow, 10f);
        Debug.DrawLine(boxCastStart + new Vector3(-boxCastSize.x / 2f, boxCastSize.y / 2f), boxCastStart + new Vector3(-boxCastSize.x / 2f, -boxCastSize.y / 2f), Color.yellow, 10f);
        foreach (var b in boxStack)
        {
            if (stackCols.Contains(b.collider)) continue;
            
            if (b.transform.TryGetComponent(out Box box))
            {
                Debug.Log(b.transform.name + " found in stack ", b.collider.gameObject);
                stackComputed[box.code]++;
                stackCols.Add(b.collider);
            }
        }
    }

    private bool IsGoalMet(Dictionary<string, int> request, Dictionary<string, int> goal)
    {
        print("Checking if goal is met for request");
        foreach (var (code, count) in request)
        {
            if (goal[code] != count)
            {
                Debug.Log("Failing because goal's count of " + code + " is " + goal[code] + " where " + count + " is needed");
                return false;
            }
        }

        return true;
    }
}
