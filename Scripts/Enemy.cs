﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Attacker))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : PooledMonoBehaviour, ITakeHit, IDie
{
    [SerializeField]
    private int maxHealth = 3;

    private int currentHealth;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Attacker attacker;
    private Character target;

    public event Action<int, int> OnHealthChanged = delegate { };
    public event Action<IDie> OnDied = delegate { };
    public event Action OnHit = delegate { };

    private bool IsDead { get { return currentHealth <= 0; } }

    public int Damage { get { return 1; } }

    public bool Alive { get; private set; }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        attacker = GetComponent<Attacker>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        Alive = true;
    }

    private void Update()
    {
        if (IsDead)
            return;

        if (target == null || target.Alive == false)
        {
            AquireTarget();
        }
        else
        {
            if (attacker.InAttakRange(target) == false)
            {
                FollowTarget();
            }
            else
            {
                TryAttack();
            }
        }
    }

    private void AquireTarget()
    {
        target = Character.All
                        .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
                        .FirstOrDefault();

        animator.SetFloat("Speed", 0f);
    }

    private void FollowTarget()
    {
        animator.SetFloat("Speed", 1f);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target.transform.position);
    }

    private void TryAttack()
    {
        animator.SetFloat("Speed", 0f);
        navMeshAgent.isStopped = true;

        if (attacker.CanAttack)
        {
            attacker.Attack(target);
        }
    }

    public void TakeHit(IDamage hitBy)
    {
        currentHealth -= hitBy.Damage;

        OnHealthChanged(currentHealth, maxHealth);

        OnHit();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        navMeshAgent.isStopped = true;

        Alive = false;
        OnDied(this);

        ReturnToPool(6f);
    }
}