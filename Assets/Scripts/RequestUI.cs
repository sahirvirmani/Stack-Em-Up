using System;
using UnityEngine;
using UnityEngine.UI;

public class RequestUI : MonoBehaviour
{
    public GameObject singleRequestPrefab;

    public Transform singleRequestParent;
    public Image image;

    public void Init(RequestManager.Request req)
    {
        if (!RequestManager.instance) return;
        
        foreach (var bc in req.boxRequests)
        {
            print("Create a single request with sprite " + RequestManager.instance.boxSprites.GetSprite(bc.boxCode) + " of box code " + bc.boxCode + "and count " + bc.boxCount);
            GameObject sr = Instantiate(singleRequestPrefab, singleRequestParent);
            sr.GetComponent<SingleRequest>().Init(RequestManager.instance.boxSprites.GetSprite(bc.boxCode), bc.boxCount);
        }
    }

    public void UpdateFillAmount(float fill)
    {
        if (image == null) return;
        
        image.fillAmount = fill;
    }
    
    public void RequestExpired()
    {
        Destroy(gameObject);
    }

    public void RequestCompleted()
    {
        Destroy(gameObject);
    }
    
}
