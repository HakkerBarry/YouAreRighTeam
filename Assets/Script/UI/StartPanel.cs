using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartPanel : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] Button BtnStart;
    [SerializeField] Button BtnAbout;
    [SerializeField] Button BtnExit;
    [Header("Panel")]
    [SerializeField] GameObject AboutPanel;

    // Start is called before the first frame update
    void Start()
    {
        AboutPanel.SetActive(false);
        BtnStart.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);// Game Scene
        });
        BtnAbout.onClick.AddListener(() =>
        {
            AboutPanel.SetActive(true);
        });
        BtnExit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

}
