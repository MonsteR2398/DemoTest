using System.Collections;
using UnityEngine;

[System.Serializable]
public class BrainrotData
{
    public string UniqueId { get; private set; }
    public int Id;
    public int Income;
    [HideInInspector] public Rarity Rarity;
    [HideInInspector] public Variant Variant;
    [HideInInspector] public float Size;
    [HideInInspector] public bool HasSpawned;
    [HideInInspector] public Vector3 Position;


    public string SetUniqueId(string value) => UniqueId = value;
    public void AddUniqueId()
    {
        if (string.IsNullOrEmpty(UniqueId))
            SetUniqueId(System.Guid.NewGuid().ToString());
    }

}
public class Brainrot : Item, ITriggerEnterHandler, ISoundEmitter, ISpawned
{
    public BrainrotData Data;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip _clip;
    [SerializeField] private int _soundRepeatInterval;

    [Header("Animation Settings")]
    [SerializeField] private Animator _animator;
    private int _totalIncome;
    private Coroutine incomeRoutine;

    public AudioClip Clip => _clip;
    int ISoundEmitter.SoundInterval { get => _soundRepeatInterval; set => _soundRepeatInterval = value; }
    
    public bool HasSpawned
    {
        get => Data.HasSpawned;
        set => Data.HasSpawned = value;
    }

    private void Start()
    {
        Initialize();
    }


    public void Initialize()
    {
        FlipToGlobalXPos();
        if (!NowEnemy)
        {
            TextDisplayEvents.RaiseDisplayEnabled(this);
            incomeRoutine = StartCoroutine(IncomeComing());
        }
    }

    private void OnDisable()
    {
        if (!NowEnemy)
        {
            TextDisplayEvents.RaiseDisplayDisabled(this);
            if(incomeRoutine != null)
                StopCoroutine(incomeRoutine);
        }
        _animator.StopPlayback();
    }

    public override void OnSpawnToGround()
    {
        Data.HasSpawned = true;
        Data.Position = transform.position;
        _animator.Play("Dance");
    }

    public override void OnGetFromPool()
    {
        if (HasSpawned)
            _canBuy = false;
    }

    private IEnumerator IncomeComing()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _totalIncome += Data.Income;
        }
    }

    public void HandleTriggerEnter(Collider other)
    {
        if (NowEnemy) return;
        Debug.Log($"Собрано {_totalIncome} монет");
        _totalIncome = 0;
    }

    public void SetSize(float size)
    {
        Data.Size = size;
        transform.localScale = Vector3.one * size;
    }

    public override Variant GetVariant() => Data.Variant;

    public float GetSize() => Data.Size;

    public override string GetTextOnDisplay()
    {
        var rarity = $"<style=Rarity{Data.Rarity}>{Data.Rarity}</style>\n";
        var totalIncome = $"<style=Price>${_totalIncome}</style>\n";
        var income = $"${Data.Income}/s\n";
        var variant = $"<style=Variant{Data.Variant}>{Data.Variant}</style>\n";
        if (Data.Variant == Variant.Default)
            variant = "";

        return rarity + totalIncome + income + variant;
    }


    private void FlipToGlobalXPos()
    {
        float angle = transform.position.x < 0 ? 90f : -90f;
        transform.rotation = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
    }
}
