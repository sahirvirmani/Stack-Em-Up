using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxSpawner : MonoBehaviour
{
    [System.Serializable]
    public class BoxProbability
    {
        public GameObject boxPrefab;
        public float probabilityThreshold;
    }
    
    public Vector2 spawnArea;
    public float timePerBox = 1f;
    public List<BoxProbability> BoxProbabilities = new List<BoxProbability>();

    private float time = 0;
    private float rolledNum;
    private void OnDrawGizmos()
    {
        Vector3 a = transform.position - Vector3.up * spawnArea.y / 2f - Vector3.right * spawnArea.x / 2f;
        Vector3 b = transform.position - Vector3.up * spawnArea.y / 2f + Vector3.right * spawnArea.x / 2f;
        Vector3 c = transform.position + Vector3.up * spawnArea.y / 2f + Vector3.right * spawnArea.x / 2f;
        Vector3 d = transform.position + Vector3.up * spawnArea.y / 2f - Vector3.right * spawnArea.x / 2f;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }

    private void Start()
    {
        for (int i = 0; i < BoxProbabilities.Count; i++)
        {
            for (int j = 0; j < BoxProbabilities.Count; j++)
            {
                if (BoxProbabilities[i].probabilityThreshold < BoxProbabilities[j].probabilityThreshold)
                {
                    (BoxProbabilities[i], BoxProbabilities[j]) = (BoxProbabilities[j], BoxProbabilities[i]);
                }
            }
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > timePerBox)
        {
            SpawnBox();
            time = 0;
        }
    }

    private void SpawnBox()
    {
        float roll = Random.Range(0f, 1f);
        rolledNum = roll;
        Vector3 randomPos = new Vector3(Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f), Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f), transform.position.z);

        if (roll < BoxProbabilities[0].probabilityThreshold)
        {
            GameObject box = Instantiate(BoxProbabilities[0].boxPrefab, transform.TransformPoint(randomPos),
                Quaternion.identity);
        }

        for (int i = 0; i < BoxProbabilities.Count - 1; i++)
        {
            if (roll > BoxProbabilities[i].probabilityThreshold && roll < BoxProbabilities[i + 1].probabilityThreshold)
            {
                randomPos = new Vector3(Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f),
                    Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f), transform.position.z);
                GameObject box = Instantiate(BoxProbabilities[i + 1].boxPrefab, transform.TransformPoint(randomPos), Quaternion.identity);
            }
        }
    }
}
