using Utils.Core.Events;

public class SceneLoadedEvent : IEvent
{
	public string Scene { get; private set; }

	public SceneLoadedEvent(string scene)
	{
		Scene = scene;
	}
}
