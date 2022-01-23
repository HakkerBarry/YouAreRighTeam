using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trigger : MonoBehaviour
{
    [SerializeField] GameObject Block;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Block.GetComponent<Animator>().SetTrigger("Open");
        Block.GetComponent<Collider2D>().isTrigger = true;
        Block.GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().Play();
        this.gameObject.SetActive(false);
    }

}
