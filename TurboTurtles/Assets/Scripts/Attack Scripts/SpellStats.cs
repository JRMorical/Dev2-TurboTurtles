using UnityEngine;

public enum SpellType { Fire, Ice, Shock }

[CreateAssetMenu(menuName = "Spells/Spell Stats")]
public class SpellStats : ScriptableObject
{
    public string spellName;
    public SpellType spellType;
    public GameObject staffPrefab;
    public GameObject projectilePrefab;
    public GameObject chainEffectPrefab;

    [Range(1, 20)] public int damage = 5;
    [Range(3, 100)] public int castDist = 30;
    [Range(0.1f, 3)] public float castRate = 0.5f;
    [Range(10, 150)] public float projectileSpeed = 80f;

    [Range(0f, 1f)] public float slowAmount = 0f;
    [Range(0f, 5f)] public float slowDuration = 0f;

    [Range(1, 5)] public int chainCount = 2;
    [Range(1, 20)] public float chainRadius = 8f;
    [Range(0f, 1f)] public float chainDamageFalloff = 0.5f;
}