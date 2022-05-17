using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoxToSprite", menuName = "BoxToSprite", order = 1)]
public class BoxToSprite : ScriptableObject
{
    [System.Serializable]
    public class BoxSprite
    {
        public string boxCode;
        public Sprite boxSprite;
    }

    public List<BoxSprite> boxSprites = new List<BoxSprite>();

    public Sprite GetSprite(string box)
    {
        foreach (var bs in boxSprites)
        {
            if (bs.boxCode == box)
            {
                return bs.boxSprite;
            }
        }

        return null;
    }
}
