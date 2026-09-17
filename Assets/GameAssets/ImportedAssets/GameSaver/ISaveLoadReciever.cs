using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveLoadReciever
{
    void OnBeforeSerialize();
    void OnAfterDeserialize();
}