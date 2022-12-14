using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lexic;
public class NamesGeneratorManager : MonoBehaviour
{
    public static Action<Action<string>> UpdateName = delegate { };

    [SerializeField] private NameGenerator nameGenerator_;

    private void OnEnable()
    {
        UpdateName += GenerateName;
    }

    private void OnDisable()
    {
        UpdateName -= GenerateName;
    }

    private void GenerateName(Action<string> callback)
    {
        callback?.Invoke(nameGenerator_.GetNextRandomName());

    }
}
