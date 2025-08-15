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
public class Brainrot : Item, ITriggerEnterHandler, ISpawned
{
    public BrainrotData Data;

    // [Header("Sound Settings")]
    // [SerializeField] private AudioClip _clip;
    // [SerializeField] private float _soundRepeatInterval;
    // [SerializeField] private float _phaseOffset;
    // [SerializeField] private float _nextPlayTime;

    [Header("Animation Settings")]
    [SerializeField] private Animator _animator;
    private int _totalIncome;
    private Coroutine incomeRoutine;

    [Header("Material Settings")]
    [SerializeField] private VariantMaterialsConfig materialConfig;


    public bool HasSpawned
    {
        get => Data.HasSpawned;
        set => Data.HasSpawned = value;
    }

    private void Start()
    {
        Initialize();
        Debug.Log(Data.Size, gameObject);
    }

    public void Initialize()
    {
        FlipToGlobalXPos();
        SetMaterials();
        Data.Income += MultipleValues(Data.Income);
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
            if (incomeRoutine != null)
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

    public void SetMaterials()
    {
        if (materialConfig == null) return;

        Material newMaterial = materialConfig.GetMaterial(Data.Variant);
        if (newMaterial == null) return;

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in allRenderers)
        {
            Material[] sharedMaterials = renderer.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
                sharedMaterials[i] = newMaterial;
            
            renderer.sharedMaterials = sharedMaterials;
        }
    }

    private int MultipleValues(int value)
    {
        int incomeMulti = (int)Mathf.Floor(value * Data.Size) * (int)Data.Variant;
        return incomeMulti;
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
        Data.Size = Mathf.Round(size * 100f) / 100f;
        _displayOffset += new Vector3(0f, size, 0f);
        transform.localScale = Vector3.one * size;
    }
    public override string GetItemID()
    {
        if (Data.UniqueId == "")
            Data.AddUniqueId();
        return Data.UniqueId;
    }

    public override Variant GetVariant() => Data.Variant;
    public float GetSize() => Data.Size;
    public override int GetPrice()
    {
        var price = base.GetPrice();
        var multi = MultipleValues(price);
        return multi;
    }

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
