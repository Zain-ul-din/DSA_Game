using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class Solider : MonoBehaviour {
    
    public bool target = false;
    private float heath = 100f;
    
    public bool isDie = false;
    
    
    
    [Header("Feedbacks")]
    /// a feedback to call when the attack starts
    [Tooltip("a feedback to call when the attack starts")]
    public MMFeedbacks AttackFeedback;
    /// a feedback to call when each individual attack phase starts
    [Tooltip("a feedback to call when each individual attack phase starts")]
    public MMFeedbacks IndividualAttackFeedback;
    /// a feedback to call when trying to attack while in cooldown
    [Tooltip("a feedback to call when trying to attack while in cooldown")]
    public MMFeedbacks DeniedFeedback;
    
    public MMTween.MMTweenCurve AttackCurve = MMTween.MMTweenCurve.EaseInOutOverhead;
    
    /// the duration of the attack in seconds
    public float AttackDuration = 2.5f;
    /// an offset at which to attack enemies
    public float AttackPositionOffset = 0.3f;
    /// a duration by which to reduce movement duration after every attack (making each attack faster than the previous one)
    public float IntervalDecrement = 0.1f;
    protected float _lastAttackStartedAt = -100f;
    // if enemy
    [Header("Damage feedBack")]
    public MMFeedbacks DamageFeedback;
    
    [Tooltip("a duration, in seconds, between two attacks, during which attacks are prevented")]
    public float CooldownDuration = 0.1f;

    public Vector3 enemy; 
    
    protected Vector3 _lookAtTarget;
    
    protected Vector3 _initialPosition;
    protected Vector3 _initialLookAtTarget;

    private CircleFormation gameManager;

    private bool isAtacking = false;
    private GameObject slefObj;
    private void Awake() {
        _initialPosition = gameObject.transform.position;
        _initialLookAtTarget = this.transform.position + this.transform.forward * 10f;
        _lookAtTarget = _initialLookAtTarget;
        slefObj = this.gameObject;
    }
    
    private void Start() {
        gameManager = CircleFormation.instance;
        this.transform.LookAt(gameManager.center);
        _initialLookAtTarget = gameManager.center;
    }

    private void Update() {
        
        if(target || isDie)
            return;
        
        LookAtTarget();
        
        if (Input.GetKey(0) || Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
            Attack();
        }
    }
   
    protected virtual void LookAtTarget() {
        Vector3 direction = _lookAtTarget - _initialPosition;
        this.transform.LookAt(_lookAtTarget + direction); // + direction * -30f
    }
    
    public void Attack() {
        if (Time.time - _lastAttackStartedAt < CooldownDuration + AttackDuration) {
            DeniedFeedback?.PlayFeedbacks();
        }
        else {
            AcquireTargets();
            StartCoroutine(AttackCoroutine());
            _lastAttackStartedAt = Time.time;
        }
    }

    private void AcquireTargets() {
      //  var list = FindObjectsOfType<Solider>();
        
       // Vector3 enemyPosition = list[0].transform.position;
        Vector3 direction = this.transform.position - enemy;
        Vector3 vec =enemy + direction * AttackPositionOffset;
        enemy = vec; 
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Solider>() == null)
            return;
        if (other.GetComponent<Solider>().target) {
            float damage = Random.Range(100, 100);
            other.GetComponent<Solider>().SetDamage(damage);
        }
    }
   
    
   
    public void SetDamage(float damage) {
        if (isDie || !target) {
            return;   
        }

        heath -= damage;
        DamageFeedback.PlayFeedbacks(transform.position, damage); // play on damage feed back
        this.gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * .2f, ForceMode.Impulse);
        isDie = heath <= 0;
    }

    private void OnDestroy() {
       StopAllCoroutines();
       transform.position = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);
    }


    public void SetTargetState(bool state){
        this.target = state;
        Debug.Log(this.gameObject.name + " State has been set to : " + ((state) ? "true" : "false"));
    }
    
    protected virtual IEnumerator AttackCoroutine() {
        float intervalDuration = AttackDuration/2f;
            
        // we play our initial attack feedback
        AttackFeedback?.PlayFeedbacks();

        int enemyCounter = 1;
        
        // for each new enemy, we play an attack feedback
            IndividualAttackFeedback?.PlayFeedbacks();
            MMTween.MoveTransform(this, this.transform, this.transform.position, enemy, null, 0f, intervalDuration, AttackCurve);
            _lookAtTarget = enemy;                
            yield return MMCoroutine.WaitFor(intervalDuration - enemyCounter * IntervalDecrement);

            MMTween.MoveTransform(this, this.transform, this.transform.position, _initialPosition, null, 0f, intervalDuration, AttackCurve);
            transform.position = _initialPosition;
            _lookAtTarget = _initialLookAtTarget;
    }
}
