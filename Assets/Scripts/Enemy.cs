using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntitity
{
    public enum State {Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;
    NavMeshAgent pathfinder;
    Transform target;
    LivingEntitity targetEntitiy;
    Material skinMaterial;
    Color originalColor;
    float attackDistanceTreshold = .5f;
    float timeBetweenAttacks = 1f;
    float damage = 1f;
    float newAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    bool hasTarget;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start ();
        pathfinder = GetComponent<NavMeshAgent> ();
        skinMaterial = GetComponent<Renderer> ().material;
        originalColor = skinMaterial.color;
        if(GameObject.FindGameObjectWithTag ("Player") != null){
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag ("Player").transform;
            targetEntitiy = target.GetComponent<LivingEntitity> ();
            targetEntitiy.OnDeath += OnTargetDeath;
            myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;
            StartCoroutine (UpdatePath ());
        }
   
        
    }
    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
        if(damage >= health){
             Destroy(Instantiate(deathEffect.gameObject,hitPoint,Quaternion.FromToRotation(Vector3.forward,hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.TakeHit (damage,hitPoint,hitDirection);
    }
    void OnTargetDeath(){
       hasTarget = false;
       currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasTarget){
            if(Time.time > newAttackTime){
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            if(sqrDstToTarget < Mathf.Pow (attackDistanceTreshold + myCollisionRadius + targetCollisionRadius, 2)){
                newAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
        }
        }
        }
       
       
    }
    IEnumerator Attack(){
        currentState = State.Attacking;
        pathfinder.enabled = false;
        Vector3 originalPosition = transform.position;
        Vector3  dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius );
        float attackSpeed = 3f;
        float percent = 0f;
        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;
        while( percent <= 1f){
            if(percent >= .5f && !hasAppliedDamage){
                hasAppliedDamage = true;
                targetEntitiy.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        } 
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }
    IEnumerator UpdatePath() {
        float refreshRate = .25f;
        while(hasTarget){
            if (currentState == State.Chasing){
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceTreshold/2);
            if(!dead){
                 pathfinder.SetDestination (targetPosition);
            }
           }
            yield return new WaitForSeconds(refreshRate);
        
        }
    }
}