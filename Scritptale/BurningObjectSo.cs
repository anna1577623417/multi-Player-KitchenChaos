using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BurningRecipeSo : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimeMax;
}
