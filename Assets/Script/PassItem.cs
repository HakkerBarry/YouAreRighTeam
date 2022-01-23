using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PassItem : MonoBehaviour
{
    [SerializeField] GameObject PassPanel;
    void Start()
    {
        PassPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PassPanel.SetActive(true);
            PassPanel.GetComponent<Image>().DOFade(1, 1.5f);
            PassPanel.GetComponent<PassPanel>().BtnMainMenu.gameObject.GetComponent<Image>().DOFade(1, 0.5f);
            PassPanel.GetComponent<PassPanel>().BtnMainMenu.gameObject.GetComponent<Image>().DOFade(1, 0.5f);
        }
    }
}
