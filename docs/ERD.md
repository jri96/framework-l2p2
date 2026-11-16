# Relatie Player en Game (week 4)

Een speler hoort bij één spel; ieder spel heeft precies één speler (verplicht, via `PlayerId`).

```mermaid
erDiagram
    Players ||--|| Games : "speelt"
    Players {
        int Id PK
        varchar Name
        int Health
        int MaxHealth
    }
    Games {
        int Id PK
        int PlayerId FK
        varchar CurrentLocation
        int CurrentLevel
        bool IsFinished
        bool IsWon
        datetime CreatedAt
        datetime UpdatedAt
    }
```

**Waarom deze velden in de response?** Om een spel te hervatten heeft de console nodig: het id (om volgende requests te sturen), level en locatie (waar sta je), de levens van de speler en of het spel afgelopen of gewonnen is. `CreatedAt` en `PlayerId` stuurt de API niet mee, want de console heeft ze niet nodig.
