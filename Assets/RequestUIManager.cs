using System;
using Unity.VisualScripting;
using UnityEngine;

public class RequestUIManager : MonoBehaviour
{

    public static RequestUIManager instance;

    public GameObject requestUIPrefab;
    public Transform requestUITransform;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public RequestUI CreateRequestUI(RequestManager.Request req)
    {
        GameObject ruiPrefab = Instantiate(requestUIPrefab, requestUITransform);
        RequestUI r =ruiPrefab.GetComponent<RequestUI>();
        r.Init(req);
        return r;
    }

}
