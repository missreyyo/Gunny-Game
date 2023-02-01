using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;

[RequireComponent (typeof (PlayerController))]

public class Player : NetworkBehaviour
{
public float moveSpeed = 5f;   
Camera viewCamera;
PlayerController controller;
Player player;
InputField inputfield;
[SerializeField] private MeshRenderer meshRenderer;
[SerializeField] private TextMeshProUGUI playerName;
[SerializeField] private GameObject bullet;
private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
public List<Color> colors = new List<Color>();

public float timer = 0f;

    private void Awake(){
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        
        controller = GetComponent<PlayerController> ();

        viewCamera = Camera.main;
    }
      public Transform muzzle;
   public ProjectTiles projectile;
   public float msBetweenShots = 0.1f; 
   public float muzzleVelocity = 35f;

 


   float nextShotTime;
   public void Shoot(){
   /*
     if(Time.time > nextShotTime){
        Debug.Log("ateş edildi");
        nextShotTime = Time.time + msBetweenShots;
        ProjectTiles newProjectile = Instantiate (projectile,muzzle.position, muzzle.rotation) as ProjectTiles;
        newProjectile.SetSpeed (muzzleVelocity);
        newProjectile.GetComponent<NetworkObject>().Spawn();
        
    }*/
   }
    public override void OnNetworkSpawn(){
          UpdatePositionServerRpc();
          networkPlayerName.Value = "Player: "+ (OwnerClientId );
          playerName.text = networkPlayerName.Value.ToString();
          meshRenderer.material.color = colors[(int) OwnerClientId%colors.Count ];
          if(IsLocalPlayer){
            AddChatServerRpc("Player" + (OwnerClientId ).ToString() + " joined the room");
          inputfield = ChatController.instance.chatInputField;
          inputfield.onSubmit.AddListener(SendMessageFromUI);
}
          
              
    }   
    public override void OnNetworkDespawn(){
        base.OnNetworkDespawn();
        AddChatServerRpc("Player"+ (OwnerClientId).ToString()+ " left the chat");
        inputfield = ChatController.instance.chatInputField;
    } 
    //List to hold all the instantiated bullets
    [SerializeField] private List<GameObject> spawnedBullets = new List<GameObject>();

   
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
            timer += Time.deltaTime;
            if(timer> 0.3f){
                nextShotTime = Time.time + msBetweenShots;
                ShootServerRpc();
                timer = 0f;
            }
        }
    }
    [ServerRpc] 
    private void ShootServerRpc(){
  
        ProjectTiles newProjectile = Instantiate (projectile,muzzle.position, muzzle.rotation) as ProjectTiles;
        newProjectile.SetSpeed (muzzleVelocity);
        newProjectile.GetComponent<ProjectTiles>().parent = this;
        newProjectile.GetComponent<NetworkObject>().Spawn();
    

    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(){
        GameObject toDestroy = spawnedBullets[0];
        toDestroy.GetComponent<NetworkObject>().Despawn(destroy:true);
        spawnedBullets.Remove(toDestroy);
        Destroy(toDestroy);
    }
    [ServerRpc]
    private void UpdatePositionServerRpc(){
          transform.position = new Vector3 (Random.RandomRange(-8,9),0,Random.RandomRange(-9,9));
          transform.rotation = new Quaternion (0,180,0,0);
    }
    public void SendMessageFromUI(string message){
        inputfield.text = "";
        AddChatServerRpc(message);
    
     //   chatInputField.Select();
        
    }
    [ServerRpc(RequireOwnership = false)]
    void AddChatServerRpc(string chatString){
       AddChatClientRpc(chatString);
       
    }
    [ClientRpc]
     void AddChatClientRpc(string chatString){
       ChatController.instance.AddChat(chatString,OwnerClientId); 
    }
    

}
