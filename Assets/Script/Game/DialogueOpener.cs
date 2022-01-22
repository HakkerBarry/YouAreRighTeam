using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.Wrappers;
using UnityEngine;

public class DialogueOpener : MonoBehaviour
{
    private String conversation;
    private bool canTalk = false;
    private int overlapedItemNum = 0;
    private void Start()
    {
        DialogueManager.Instance.conversationStarted += OnConversationStart;
        DialogueManager.Instance.conversationEnded += OnConversationEnded;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(canTalk) DialogueManager.StartConversation(conversation);
        }
    }

    /// <summary>
    /// Conversation use the same name with the correspond game object.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            overlapedItemNum++;
            conversation = other.gameObject.name;
            canTalk = true;    
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC") && --overlapedItemNum == 0)
        {
            conversation = null;
            canTalk = false;
        }
    }

    void OnConversationStart(Transform t)
    {
        
    }

    void OnConversationEnded(Transform t)
    {
        
    }
}
