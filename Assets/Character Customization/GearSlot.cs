using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GearSlot : MonoBehaviour
{
    [SerializeField] private Gear gear;
    [FoldoutGroup("Display")] [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private List<Sprite> sheet = new();
    
    [FoldoutGroup("Data")] [SerializeField] private int frameIndex;
    [FoldoutGroup("Data")] [SerializeField] private AnimationSheet anim;
    [FoldoutGroup("Data")] [SerializeField] private int startOffset;
    [FoldoutGroup("Data")] [SerializeField] private int frameCount;

    private int currentDirection;
    private Gear lastKnownGear;
    
    private void Awake()
    {
        LoadSpriteSheet(gear.GearAnimationSheets[0].ID);
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Tick(int targetDirection, bool flip)
    {
        if (lastKnownGear != gear)
        {
            lastKnownGear = gear;
            LoadSpriteSheet(gear.GearAnimationSheets[0].ID);
        }
        currentDirection = targetDirection;
        _spriteRenderer.flipX = flip;
        Animate();
    }
    
    void SetSprite()
    {
        int index = startOffset+(currentDirection * frameCount) + frameIndex;
        // print($"Sheet Length {sheet.Count}, index: {index}"); // Todo add debug flag
        _spriteRenderer.sprite = sheet[index];
    }
    
    void Animate()
    {
        frameIndex++;
        if (frameIndex > frameCount-1)
            frameIndex = 0;
        SetSprite();
    }
    
    public void LoadSpriteSheet(string sheetName)
    {
        sheet.Clear();
        sheet = Resources.LoadAll<Sprite>(sheetName).ToList();
    }
    
    public void LoadAnimation(int actionIndex)
    {
        if (actionIndex == -1 || gear.GearAnimationSheets.Length == 0) return;
        anim = gear.GearAnimationSheets[actionIndex];
        startOffset = anim.startIndex;
        frameCount  = anim.frameCount;
        if (frameCount <= 0)
            frameCount = 8;
    }
    
}