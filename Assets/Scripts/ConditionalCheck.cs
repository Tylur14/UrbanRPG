using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class ConditionalCheck : SerializedMonoBehaviour
{
    [SerializeField] private bool checkOnAwake;
    [SerializeField] private bool continousCheck;
    [SerializeField] private float tickRate;
    private float timer;
    [FoldoutGroup("Events")][SerializeField] private UnityEvent onMeetsRequirements;
    [FoldoutGroup("Events")][SerializeField] private UnityEvent onFailsRequirements;
    public List<Condition> Conditions = new();

    private void Awake()
    {
        if (checkOnAwake)
        {
            CheckStatus();
        }
    }

    private void Update()
    {
        if (!continousCheck) return;
        if (timer <= 0)
        {
            CheckStatus();
            timer = tickRate;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void CheckStatus()
    {
        bool canDo = IsConditionMet();

        if (canDo)
        {
            onMeetsRequirements.Invoke();    
        }
        else
        {
            onFailsRequirements.Invoke();
        }
    }
    
    private bool IsConditionMet()
    {
        return Conditions.TrueForAll(c => c.IsConditionMet());
    }
}