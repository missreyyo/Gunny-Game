using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : NetworkBehaviour
{
public float moveSpeed = 5f;   
Camera viewCamera;
PlayerController controller;
GunController gunController;
[SerializeField] private MeshRenderer meshRenderer;
[SerializeField] private TextMeshProUGUI playerName;
private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
public List<Color> colors = new List<Color>();

    private void Awake(){
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        
        controller = GetComponent<PlayerController> ();
        gunController = GetComponent<GunController> ();
        viewCamera = Camera.main;
    }
    public override void OnNetworkSpawn(){
          transform.position = new Vector3 (Random.RandomRange(-8,9),0,Random.RandomRange(-9,9));
          transform.rotation = new Quaternion (0,180,0,0);
          networkPlayerName.Value = "Player: "+ (OwnerClientId + 1);
          playerName.text = networkPlayerName.Value.ToString();
          meshRenderer.material.color = colors[(int) OwnerClientId%colors.Count ];

              
    }    

   
    void Update()
    {
        if(!IsOwner) return;
        //movement input
        Vector3 moveInput= new Vector3(Input.GetAxisRaw ("Horizontal"), 0 , Input.GetAxisRaw ("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);
        //look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if(groundPlane.Raycast(ray,out rayDistance)){
            Vector3 point = ray.GetPoint(rayDistance);
           // Debug.DrawLine(ray.origin,point,Color.red);
           controller.LookAt(point);
        }
        //weapon input
        if(Input.GetMouseButton(0)){
            gunController.Shoot();
        }
    }
}
