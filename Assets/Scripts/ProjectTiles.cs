using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectTiles : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10f;
    float damage = 1f;
    float lifeTime = 3f;
    float skinWidth = .1f;
    void Start(){
      Destroy(gameObject, lifeTime);
      Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
      if(initialCollisions.Length > 0){
          OnHitObject(initialCollisions[0]);
      }
    }
    public void SetSpeed(float newSpeed){
        speed= newSpeed;
    }
    void Update()
    {
      float moveDistance = speed * Time.deltaTime;
      CheckCollisions (moveDistance);
      transform.Translate(Vector3.forward * Time.deltaTime * speed);  
    }
    void CheckCollisions(float moveDistance){
      Ray ray =new Ray(transform.position,transform.forward);
      RaycastHit hit;
      if(Physics.Raycast(ray,out hit, moveDistance + skinWidth,collisionMask,QueryTriggerInteraction.Collide)){
            OnHitObject(hit);
      }

    }
    void OnHitObject(RaycastHit hit){
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable> ();
        if (damageableObject != null){
          damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }
    void OnHitObject(Collider c)  {
        IDamageable damageableObject = c.GetComponent<IDamageable> ();
        if (damageableObject != null){
          damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }
}
