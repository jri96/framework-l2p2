namespace RPG_API.Enums;

// Het soort ruimte waarin de speler staat. Wordt als getal in de database opgeslagen.
// Normal, Rest en Exit komen uit de week 5-opdracht; Start en Treasure zijn eigen toevoegingen.
public enum RoomType
{
    Start,
    Normal,
    Treasure,
    Rest,
    Exit
}
