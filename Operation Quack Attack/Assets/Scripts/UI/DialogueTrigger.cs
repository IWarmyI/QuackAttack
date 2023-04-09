using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueTrigger : MonoBehaviour {
    // internal properties exposed to editor
    [SerializeField] private string conversationStartNode;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private static bool interactable = true;
    private bool isCurrentConversation = false;

    public void Start() {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
    }

    private void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.CompareTag("Player") && !dialogueRunner.IsDialogueRunning && interactable){
            StartConversation();
        }
    }

    private void StartConversation() {
        Debug.Log($"Started conversation with {name}.");
        isCurrentConversation = true;
        dialogueRunner.StartDialogue(conversationStartNode);
    }

    private void EndConversation() {
        if (isCurrentConversation) {
            isCurrentConversation = false;
            interactable = false;
            Debug.Log($"Started conversation with {name}.");
        }
    }
}