using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
   [SerializeField] private int _checkpointKey;
   private void OnTriggerEnter2D(Collider2D col)
   {
      if (col.gameObject.GetComponent<PlatformerPlayer>())
         col.gameObject.GetComponent<PlatformerPlayer>().UpdateCheckpoint(_checkpointKey);
      
   }
}
