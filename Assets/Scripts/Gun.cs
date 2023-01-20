using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
   public Transform muzzle;
   public ProjectTiles projectile;
   public float msBetweenShots = 100f; 
   public float muzzleVelocity = 35f;

   public Transform shell;
   public Transform shellEjection;


   float nextShotTime;
   public void Shoot(){

    
   }
}
