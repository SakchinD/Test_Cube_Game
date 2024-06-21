using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private GameExample gameExample;

    public override void InstallBindings()
    {
        Container
            .Bind<GameExample>()
            .FromInstance(gameExample)
            .AsSingle();
    }
}