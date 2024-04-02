using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI scoreNumber;

    [SerializeField] 
    private GameObject restartPanel;
    
    [SerializeField] 
    private Button restartButton;

    public static UIManager instance = null;

    private int _scoreCounter;
    
    private void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance == this) 
            Destroy(gameObject); 
    }

    public void UpdateScore()
    {
        _scoreCounter++;
        scoreNumber.text = _scoreCounter.ToString();
    }

    public void ShowRestartButton()
    {
        restartPanel.SetActive(true);
        restartButton.onClick.AddListener((() => SceneManager.LoadScene(sceneBuildIndex: 0)));
    }
}
