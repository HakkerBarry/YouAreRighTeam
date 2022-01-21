using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutPanel : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] Button BtnExit;
    // Start is called before the first frame update
    void Start()
    {
        BtnExit.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
