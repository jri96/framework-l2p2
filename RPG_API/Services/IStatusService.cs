namespace RPG_API.Services;

// Week 3: het statusbericht komt uit een service in plaats van uit de controller.
public interface IStatusService
{
    string GetMessage();
}

public class StatusService : IStatusService
{
    public string GetMessage() => "API werkt";
}
