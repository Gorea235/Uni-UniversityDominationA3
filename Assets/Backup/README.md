# Backup Minigame

This is a backup minigame that we wrote relatively quickly, since we were not sure about how easy it would be to set up the actual minigame. It didn't turn out to be too difficult, and now it's set up, it wouldn't be difficult to modify in future, however we decided to keep this for 2 reasons:

- We didn't see the need to remove it, as it is a complete minigame that could be just dropped in
- If SEPR groups thought that the Doom minigame was too complex for them, then they could simply switch the minigame that is loaded to this one and have a very simple one to extend

## Swapping Minigames

To make this minigame the one that is loaded, you would have to do the following:

- Edit the build settings to include the SimpleMinigame.unity scene
- Edit [Sector.TriggerMinigame](/Assets/Scripts/Sector.cs#L172-L178) to load the scene `"SimpleMinigame"` rather than `"DoomMinigame"`

That's it! We've designed the minigame handling to be completely agnostic of the actual minigame, so the only other thing you might want to do is adjust the rewards or scores so that the result is more fair overall (but this is entirely optional).