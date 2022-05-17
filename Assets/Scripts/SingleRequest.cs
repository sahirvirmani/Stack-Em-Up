using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleRequest : MonoBehaviour
{

    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    
    public void Init(Sprite sprite, int count)
    {
        image.sprite = sprite;
        text.text = "" + count;
        print(name + " sprite " + sprite);
    }
}
