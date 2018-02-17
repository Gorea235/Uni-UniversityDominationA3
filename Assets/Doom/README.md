# Doom Minigame

This is our main minigame. It was orignally in [another repo](https://github.com/Gorea235/DoomClone) to allow us to develop it easier, and the main assets have been copied in and modified slightly to work with the game.

The minigame uses a Controller and a Trigger structure, meaning that GameObjects with logic have a Controller class attached that is (usually) completely self-contained, and to activate events we use the ActivateTrigger system that the Standard Assets provides.

One main exception to the self-contained rule is the DoomMinigameManager, which handles the general game state (like updating objectives and handling the win state), however the way it works is designed to be obvious.
