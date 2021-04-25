﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        LOOKFOR,
        GOTO,
        ATTACK,
    }
    public State CurState;
    public float Speed = 1;
    public float GoToDistance = 6;
    public float AttackDistance = 2;
    public float AttackTimer = 2;
    public Transform Target;
    public string PlayerTag = "Player";
    private float CurTime;
    private PlayerMovement PlayerScript;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Target = GameObject.FindGameObjectWithTag(PlayerTag).transform;
        CurTime = AttackTimer;
        if (Target != null)
        {
            PlayerScript = Target.GetComponent<PlayerMovement>();
        }

        while (true)
        {
            switch (CurState)
            {
                case State.LOOKFOR:
                    LookFor();
                    break;
                case State.GOTO:
                    GoTo();
                    break;
                case State.ATTACK:
                    Attack();
                    break;
            }
            yield return 0;
        }
    }

    void LookFor()
    {
        transform.LookAt(Target);
        if (Vector3.Distance(Target.position, transform.position) < GoToDistance)
        {
            CurState = State.GOTO;
        }
    }

    void GoTo()
    {
        transform.LookAt(Target);
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit Buddy;
        if (Physics.Raycast(transform.position, fwd, out Buddy))
        {
            if (Buddy.transform.tag != PlayerTag)
            {
                CurState = State.LOOKFOR;
                return;
            }
        }

        if (Vector3.Distance(Target.position, transform.position) > GoToDistance)
        {
            CurState = State.LOOKFOR;
            return;
        }


        if (Vector3.Distance(Target.position, transform.position) > AttackDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);
        }
        else
        {
            CurState = State.ATTACK;
        }
    }

    void Attack()
    {
        transform.LookAt(Target);
        CurTime -= Time.deltaTime;

        if (CurTime < 0)
        {
            //PlayerScript.health--;
            CurTime = AttackTimer;
        }

        if (Vector3.Distance(Target.position, transform.position) > AttackDistance)
        {
            CurState = State.GOTO;
        }
    }
}
