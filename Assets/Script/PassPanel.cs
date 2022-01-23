using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PassPanel : MonoBehaviour
{
    [SerializeField] public Button BtnMainMenu;
    [SerializeField] public Button BtnRestart;
    // Start is called before the first frame update
    void Start()
    {
        BtnMainMenu.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        BtnRestart.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
