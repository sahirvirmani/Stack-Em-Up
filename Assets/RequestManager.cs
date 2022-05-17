using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RequestManager : MonoBehaviour
{
    public static RequestManager instance = null;

    [System.Serializable]
    public class Request
    {
        [System.Serializable]
        public struct BoxCount
        {
            public string boxCode;
            public int boxCount;
        }

        public List<BoxCount> boxRequests = new List<BoxCount>();
        public float reqTotalTime;
        public float reqRemainingTime;
        public int reqReward;
        public float reqProbabilityThreshold;
        public Dictionary<string, int> reqComputed = new Dictionary<string, int>();

        public Request()
        {
            boxRequests = new List<BoxCount>();
            reqComputed = new Dictionary<string, int>();
        }

        public Request(Request newRequest)
        {
            foreach (var bc in newRequest.boxRequests)
            {
                boxRequests.Add(bc);
            }

            reqTotalTime = newRequest.reqTotalTime;
            reqRemainingTime = newRequest.reqRemainingTime;
            reqReward = newRequest.reqReward;
            reqProbabilityThreshold = newRequest.reqProbabilityThreshold;

            foreach (var p in newRequest.reqComputed)
            {
                reqComputed.Add(p.Key, p.Value);
            }
        }
    }

    public BoxToSprite boxSprites;

    public List<Request> allRequests = new List<Request>();
    public List<Tuple<Request, RequestUI>> requests = new List<Tuple<Request, RequestUI>>();

    public float timeBetweenRequestMin, timeBetweenRequestMax;

    private float nextRequestTime;

    private float time = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < allRequests.Count; i++)
        {
            for (int j = 0; j < allRequests.Count; j++)
            {
                if (allRequests[i].reqProbabilityThreshold < allRequests[j].reqProbabilityThreshold)
                {
                    (allRequests[i], allRequests[j]) = (allRequests[j], allRequests[i]);
                }
            }
        }

        print("Requests Computed");
        int k = 0;
        string requestString = "";
        foreach (var req in allRequests)
        {
            req.reqRemainingTime = req.reqTotalTime;
            print("Request " + k + 1 + ":");

            foreach (var b in boxSprites.boxSprites)
            {
                req.reqComputed.Add(b.boxCode, 0);
            }

            foreach (var bc in req.boxRequests)
            {
                req.reqComputed[bc.boxCode] = bc.boxCount;
            }

            foreach (var (code, count) in req.reqComputed)
            {
                requestString += code + " " + count + "   ";
            }
            print(requestString);
            k++;
        }

        nextRequestTime = Random.Range(timeBetweenRequestMin, timeBetweenRequestMax);
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > nextRequestTime)
        {
            Request nextRequest = new Request(GetNextRequest());
            RequestUI rui = RequestUIManager.instance.CreateRequestUI(nextRequest);
            AddRequest(nextRequest, rui);

            time = 0;
            nextRequestTime = Random.Range(timeBetweenRequestMin, timeBetweenRequestMax);
        }

        for (int i = 0; i < requests.Count; i++)
        {
            Tuple<Request, RequestUI> t = requests[i];
            t.Item1.reqRemainingTime -= Time.deltaTime;
            t.Item2.UpdateFillAmount(t.Item1.reqRemainingTime / t.Item1.reqTotalTime);
            if (t.Item1.reqRemainingTime <= 0)
            {
                t.Item2.RequestExpired();
                RemoveRequest(t.Item1);
            }
        }
    }

    private void AddRequest(Request r, RequestUI rui)
    {
        requests.Add(Tuple.Create(r, rui));
    }

    public void RemoveRequest(Request r)
    {
        for (int i = 0; i < requests.Count; i++)
        {
            Tuple<Request, RequestUI> t = requests[i];
            if (t.Item1 == r)
            {
                t.Item2.RequestCompleted();
                requests.Remove(t);
            }
        }
    }

    private Request GetNextRequest()
    {
        float roll = Random.Range(0f, 1f);
        for (int i = 0; i < allRequests.Count - 1; i++)
        {
            if (roll > allRequests[i].reqProbabilityThreshold && roll < allRequests[i + 1].reqProbabilityThreshold)
            {
                return allRequests[i];
            }
        }

        return allRequests[^1];
    }
}