using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosingAnimationEvents : MonoBehaviour
{
    public GameFlowController2 gameFlow;

    // Method ini akan dipanggil dari animation event saat animasi selesai
    public void OnClosingAnimationComplete()
    {
        Debug.Log("Animasi menutup selesai.");
        if (gameFlow != null)
        {
            gameFlow.GameFinished();
        }
        else
        {
            Debug.LogWarning("belum ketrigger finish.");
        }
    }
    public void StartClosing()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Close");
    }

}
