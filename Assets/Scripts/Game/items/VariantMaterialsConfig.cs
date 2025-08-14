using UnityEngine;

[CreateAssetMenu(fileName = "VariantMaterialsConfig", menuName = "Configs/Variant Materials Config")]
public class VariantMaterialsConfig : ScriptableObject
{
    [System.Serializable]
    public class VariantMaterialPair
    {
        public Variant variant;
        public Material material;
    }

    [SerializeField] private VariantMaterialPair[] materialsByVariant;

    public Material GetMaterial(Variant variant)
    {
        foreach (var pair in materialsByVariant)
        {
            if (pair.variant == variant)
                return pair.material;
        }
        return GetDefaultMaterial();
    }

    public Material GetDefaultMaterial()
    {
        foreach (var pair in materialsByVariant)
        {
            if (pair.variant == Variant.Default)
                return pair.material;
        }
        return null;
    }
}