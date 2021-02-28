using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreUpdater : MonoBehaviour
{
    private Text scoreText;

    private void Start()
    {
        scoreText = GetComponent<Text>();
        GameManager.OnScoreChange += UpdateText;        
    }

    private void UpdateText(int value)
    {
        scoreText.text = value.ToString();
    }

    private void OnDestroy()
    {
        GameManager.OnScoreChange -= UpdateText;
    }
}
