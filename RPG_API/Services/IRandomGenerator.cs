namespace RPG_API.Services;

// Interface voor toeval. Services vragen om "iets dat random getallen geeft"
// en weten niet welke class dat precies is. In een test kun je een vaste
// (voorspelbare) versie meegeven.
public interface IRandomGenerator
{
    // Getal van minimum (inclusief) tot maximum (exclusief).
    int Next(int minimum, int maximum);

    // True met de opgegeven kans in procenten (0-100).
    bool Chance(int percentage);
}

public class RandomGenerator : IRandomGenerator
{
    public int Next(int minimum, int maximum)
    {
        return Random.Shared.Next(minimum, maximum);
    }

    public bool Chance(int percentage)
    {
        return Random.Shared.Next(0, 100) < percentage;
    }
}
