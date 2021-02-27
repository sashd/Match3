using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private void Start()
    {
        GameManager.OnScoreChange += UpdateText;
    }

    private void UpdateText(int value)
    {
        if (scoreText == null)
            return;

        scoreText.text = value.ToString();
    }

    private void OnDestroy()
    {
        GameManager.OnScoreChange -= UpdateText;
    }
}
