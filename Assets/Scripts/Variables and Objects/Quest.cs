using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I302.Manu;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Questing/Quest")]
public class Quest : ActivitySignature
{
    [Title("Quest Settings")]
    public string QuestName;
    public QuestType QuestType;
    public QuestState CurrentState;
    [TextArea] public string QuestDescription;
    
    [Space(10),PropertyOrder(70)]
    public List<QuestTaskData> Tasks = new();
    
    [Space(10)]
    public QuestTaskSignature CurrentStep;
    public int TaskIndex;
    public bool AutoComplete;
    
    private StringVariable saveSlot;

    [Button, TabGroup("Functions","Progress Functions")]
    public void StartQuest()
    {
        if (CurrentState == QuestState.Started || CurrentState == QuestState.Completed) return;
        TaskIndex = -1;
        CurrentState = QuestState.Started;
        StartNextTask();
        GameEvents.OnAcceptQuest.Raise(this);
    }
    
    public int TryCompleteTask(QuestTaskSignature taskTaskSignature, int amount = 1)
    {
        if (CurrentState is not QuestState.Started or QuestState.ReadyToComplete) return 0;
        
        var task = Tasks[TaskIndex];

        if (task.taskTaskSignature != taskTaskSignature)
        {
            return 0;
        }
        if (amount > (task.numberOfRequiredHits - task.hits))
        {
            amount = task.numberOfRequiredHits - task.hits;
        }
        
        task.Hit(amount);
        GameEvents.OnMakeQuestProgress.Raise(this);
        
        if (task.IsTaskComplete())
        {
            StartNextTask();
        }
        else SaveQuest();

        return amount;
    }

    [Button, TabGroup("Functions","Progress Functions")]
    private void StartNextTask()
    {
        TaskIndex++;
        if (TaskIndex >= Tasks.Count && AutoComplete)
        {
            CompleteQuest();
            return;
        }
        else if(TaskIndex >= Tasks.Count) TaskIndex--;
        CurrentStep = Tasks[TaskIndex].taskTaskSignature;
        var lastQuestTask = Tasks.Last();
        if (CurrentStep == lastQuestTask.taskTaskSignature &&
            lastQuestTask.hits == lastQuestTask.numberOfRequiredHits && !AutoComplete)
        {
            CurrentState = QuestState.ReadyToComplete;
            GameEvents.OnReadyToComplete.Raise(this);
        }
        SaveQuest();
    }

    public QuestTaskData GetCurrentQuestTask()
    {
        return Tasks[TaskIndex];
    }
    
    [Button, TabGroup("Functions","Progress Functions")]
    public void CompleteQuest()
    {
        CurrentStep = null;
        CurrentState = QuestState.Completed;
        GameEvents.OnCompleteQuest.Raise(this);
        SaveQuest();
    }

    [Button, TabGroup("Functions","Progress Functions")]
    private void ResetQuest()
    {
        foreach (var task in Tasks)
            task.hits = 0;
        TaskIndex = 0;
        CurrentStep = null;
        CurrentState = QuestState.NotStarted;
    }

    [Button, TabGroup("Functions","Data Functions")]
    private void SaveQuest()
    {
        SaveLoad.SaveQuest(new QuestSaveData(this));
    }

    [Button, TabGroup("Functions","Data Functions")]
    private void LoadQuest()
    {
        QuestSaveData loadedData = SaveLoad.LoadQuest(QuestName);
        if (loadedData == null)
        {
            return;
        }
        TaskIndex = loadedData.SavedTaskIndex;
        if (Tasks.Count < TaskIndex)
        {
            CurrentStep = Tasks[TaskIndex].taskTaskSignature;
        }
        
        int j = 0;
        Tasks.ForEach(o =>
        {
            o.hits = loadedData.SavedTaskHits[j];
            j++;
        });
        CurrentState = loadedData.SavedQuestState;
    }

    private void OnEnable()
    {
        LoadQuest();
    }
}

[Serializable]
public class QuestTaskData
{
    public QuestTaskSignature taskTaskSignature;
    [FoldoutGroup("Display"), TextArea]public string TaskDescription;
    [FoldoutGroup("Data")]public int numberOfRequiredHits = 1;
    [FoldoutGroup("Data")]public int hits;

    public void Hit(int amount = 1)
    {
        hits += amount;
    }
    
    public bool IsTaskComplete()
    {
        return hits >= numberOfRequiredHits;
    }
}

[Serializable]
public class QuestSaveData
{
    public string Name;
    public int SavedTaskIndex;
    public int[] SavedTaskHits;
    public QuestState SavedQuestState;

    public QuestSaveData(Quest quest)
    {
        Name = quest.QuestName;
        SavedTaskIndex = quest.TaskIndex;
        SavedTaskHits = quest.Tasks.Select(o => o.hits).ToArray();
        SavedQuestState = quest.CurrentState;
    }
}