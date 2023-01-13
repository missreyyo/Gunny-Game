using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
   public Transform muzzle;
   public ProjectTiles projectile;
   public float msBetweenShots = 100f; 
   public float muzzleVelocity = 35f;

   float nextShotTime;
   public void Shoot(){
    if(Time.time > nextShotTime){
        nextShotTime = Time.time + msBetweenShots / 1000f;
        ProjectTiles newProjectile = Instantiate (projectile,muzzle.position, muzzle.rotation) as ProjectTiles;
        newProjectile.SetSpeed (muzzleVelocity);
    }
    
   }
}
