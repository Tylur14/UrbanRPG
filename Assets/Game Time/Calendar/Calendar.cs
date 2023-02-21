using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monthNameDisplay;
    [SerializeField] private GameObject calendarDayDisplayObject;
    [SerializeField] private Transform calendaryDisplayContainer;
    
    [SerializeField] private TimeVariable dayVariable;
    [SerializeField] private TimeVariable monthVariable;
    [SerializeField] private Slider yearProgressDisplay;

    private const int monthLength = 28;
    
    private void Awake()
    {
        UpdateCalendar();
    }

    private void Update()
    {
        monthNameDisplay.text = ((MonthName) monthVariable.Value).ToString();
        yearProgressDisplay.value = Mathf.InverseLerp(0, 5, monthVariable.Value);
    }

    public void UpdateCalendar()
    {
        print("Updating Calendar");
        var oldItems = calendaryDisplayContainer.GetComponentsInChildren<Transform>();
        foreach (Transform item in oldItems) {
            if(item!=calendaryDisplayContainer)
                GameObject.Destroy(item.gameObject);
        }
        for (int i = 0; i < 28; i++)
        {
            var obj = Instantiate(calendarDayDisplayObject, calendaryDisplayContainer);
            bool isHighlighted = i == (int) dayVariable.Value;
            int dayOffset = i + 1;
            print($"INFO | Day: {dayVariable.Value}, Day with Offset: {dayOffset}, Index: {i}, Highlighted: {isHighlighted}");
            obj.GetComponent<CalendarDayDisplay>().Setup(isHighlighted,dayOffset,null);
        }
    }
}
