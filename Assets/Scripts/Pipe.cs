using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PipeType
{
    ENTERUNDERWORLD,
    EXITUNDERWORLD
}

public class Pipe : MonoBehaviour
{

    [SerializeField] private PipeType _pipeType;

    public PipeType Type => _pipeType;
    
    
    public void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"OnPipe");

        if (_pipeType == PipeType.ENTERUNDERWORLD)
        {
            other.gameObject.GetComponent<PlatformerPlayer>().OnPipe = true;
            return;
        }
        
        other.gameObject.GetComponent<PlatformerPlayer>().OnExitUnderworldPipe = true;

    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        if (_pipeType == PipeType.ENTERUNDERWORLD)
        {
            other.gameObject.GetComponent<PlatformerPlayer>().OnPipe = false;
            return;
        }
        
        other.gameObject.GetComponent<PlatformerPlayer>().OnExitUnderworldPipe = false;
    }
}
