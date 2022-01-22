using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        this.gameObject.SetActive(false);
    }
    //Sequence Observation()
    //{
        

    //    Sequence s = DOTween.Sequence();
    //    s.Append(armIK.DOPunchPosition(armIK.right / 5, .2f, 10, 1));
    //    s.Join(armIK.DOPunchRotation(new Vector3(0, 0, -45), .2f, 10, 1));
    //    s.AppendCallback(() => isCastingMagic = false);
    //    return s;
    //}
}
