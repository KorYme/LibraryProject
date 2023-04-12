using UnityEngine;

public class TestEnumData : MonoBehaviour
{
    //[ForceInterface(typeof(IWeapon))]
    //public Object weapon;

    public enum TEXTSTYLES
    {
        MEDIUM,
        TINY,
        SMALL,
        BIG,
        HUGE,
        SO_HUGE,
    }

    [EnumData(typeof(TEXTSTYLES))]
    public TextData[] _textData;
}

[System.Serializable]
public class TextData
{
    public int _size;
    public Color _color;
}