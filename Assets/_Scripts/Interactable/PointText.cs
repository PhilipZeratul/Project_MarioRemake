using UnityEngine;
using TMPro;


public class PointText : MonoBehaviour
{
    private TextMeshPro text;


    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
    }

    public void SetScore(int score)
    {
        text.text = score.ToString();
    }

    public void Finish()
    {
        Destroy(gameObject);
    }
}
