﻿using System;
using System.Collections;
using UnityEngine;

public class Attacker: AbilityBase, IAttack
{
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float attackOffset = 1f;
    [SerializeField]
    private float attackRadius = 1f;
    [SerializeField]
    private float attackImpactDelay = 1f;
    [SerializeField]
    private float attackRange = 2f;


    private LayerMask layerMask;    
    private Collider[] attackResults;
    private Animator animator;
    
    public int Damage { get { return damage; } }
    
    private void Awake()
    {
        string currentLayer = LayerMask.LayerToName(gameObject.layer);
        layerMask = ~LayerMask.GetMask(currentLayer);

        animator = GetComponentInChildren<Animator>();

        var animationImpactWatcher = GetComponentInChildren<AnimationImpactWatcher>();
        if (animationImpactWatcher != null)
        {
            animationImpactWatcher.OnImpact += AnimationImpactWatcher_OnImpact;
        }

        attackResults = new Collider[10];
    }
    public void Attack(ITakeHit target)
    {
        attackTimer = 0;
        StartCoroutine(DoAttack(target));
    }

    internal bool InAttakRange(ITakeHit target)
    {
        if (target.Alive == false)
            return false;

        var distance = Vector3.Distance(transform.position, target.transform.position);

        return distance < attackRange;
    }
    private IEnumerator DoAttack(ITakeHit target)
    {
        yield return new WaitForSeconds(attackImpactDelay);

        if (target.Alive && InAttakRange(target))
        {
            target.TakeHit(this);
        }        
    }   

    //CALLED by animation event via AnimationImpactWatcher
    private void AnimationImpactWatcher_OnImpact()
    {
        Vector3 position = transform.position + transform.forward * attackOffset;
        int hitCount = Physics.OverlapSphereNonAlloc(position, attackRadius, attackResults, layerMask);

        for (int i = 0; i < hitCount; i++)
        {
            var takeHit = attackResults[i].GetComponent<ITakeHit>();
            if (takeHit != null)
            {
                takeHit.TakeHit(this);
            }
        }
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    protected override void OnUse()
    {
        Attack();
    }
}