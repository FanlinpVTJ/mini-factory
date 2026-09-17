using UnityEngine;
using Zenject;

public class FpsLock : IInitializable
{
    private readonly int targetFrameRate;

    public FpsLock(int targetFrameRate) 
    {
        this.targetFrameRate = targetFrameRate;
    }

    void IInitializable.Initialize()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}