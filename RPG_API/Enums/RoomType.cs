namespace RPG_API.Enums;

// Het soort ruimte waarin de speler staat. Wordt als getal in de database opgeslagen.
// Normal, Rest, Combat en Exit komen uit de week 5-opdracht; Start, Treasure en Boss zijn eigen toevoegingen.
public enum RoomType
{
    Start,
    Normal,
    Combat,
    Treasure,
    Rest,
    Boss,
    Exit
}
