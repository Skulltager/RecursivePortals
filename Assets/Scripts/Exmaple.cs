
using UnityEngine;

public class Exmaple
{
    public void Main()
    {
        int result = 5 + (10 / 2);
        Debug.Log(result);

        if (result == 10 && (result == 7 || result == 4))
        {

        }

        AValue aValue = new AValue(10);
        AddOne(aValue);
        Debug.Log(aValue.a);
    }

    public int AddOne(AValue aValue)
    {
        return aValue.a + 1;
    }
}


public struct AValue
{
    public int a;

    public AValue(int value)
    {
        a = value;
    }
}