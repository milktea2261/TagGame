using UnityEngine;

public enum AttributeModifierType { Flat, Percent }

[System.Serializable]
public class AttributeModifier
{
    public float value;
    public AttributeModifierType type;
    public object source;

    public AttributeModifier(float value, AttributeModifierType type, object source) {
        this.value = value;
        this.type = type;
        this.source = source;
    }
    public AttributeModifier(float value, AttributeModifierType type) : this(value, type, null) { }

}
