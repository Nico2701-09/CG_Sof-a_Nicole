using UnityEngine;
using System;

[ExecuteInEditMode]
public class MaterialAnimator : MonoBehaviour
{
    [Serializable]
    public struct AnimatableParameter 
    {
            public string name;
            public AnimationCurve floatvalue;

    }
    [SerializeField] AnimatableParameter[] animatableParameters;
    [SerializeField] Material material;

    public float normalizedTime;

    private void Update()
    {
        foreach (AnimatableParameter param in animatableParameters)
        {
            material.SetFloat(param.name, param.floatvalue.Evaluate(normalizedTime));
        }
    }
}
