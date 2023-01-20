using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GunController : NetworkBehaviour
{
    public Transform weaponHold;
    public Gun startingGun;
    Gun equippedGun;
   void Start(){
    if(startingGun != null){
        EquipGun(startingGun);
    }
   }
   public void EquipGun(Gun gunToEquip) {
    if(equippedGun != null){
        Destroy(equippedGun.gameObject);
    }
       equippedGun = Instantiate (gunToEquip,weaponHold.position, weaponHold.rotation) as Gun;
       
       equippedGun.transform.parent = weaponHold;
       equippedGun.GetComponent<NetworkObject>().Spawn();
   }
   public void Shoot(){
   
   }
}
