## ModsOfPantheon mod template
This is the minimum solution for creating a mod for Pantheon: Rise of the Fallen. This serves as a starting point for those wanting to learn to create mods.
This solution uses .NET 6, which is required for modding Unity games.

### Setup
Ensure you have the [requirements to run MelonLoader set up](https://melonwiki.xyz/#/?id=requirements) and have installed MelonLoader to your game.

* Edit the solution and project name to something other than `ModTemplate`.
* Changing the folder names will require edits to the [solution file](https://github.com/ModsOfPantheon/ModTemplate/blob/master/ModTemplate.sln#L3).
* If your game is not installed to the default Steam path, set your game's path in the [props file](https://github.com/ModsOfPantheon/ModTemplate/blob/master/Directory.Build.props).
* Set your [mod metadata](https://github.com/ModsOfPantheon/ModTemplate/blob/master/ModTemplate/Properties/AssemblyInfo.cs#L4), including the author name etc.
* Edit the name of the built dll in the [csproj file](https://github.com/ModsOfPantheon/ModTemplate/blob/master/ModTemplate/ModTemplate.csproj#L551). This will automatically copy your mod to the mods folder in your game installation, to save you copying files by hand.

### Project structure
Documentation about MelonLoader can be found on [their wiki](https://melonwiki.xyz/#/README).

Mods must have a [class that inherits from MelonMod](https://github.com/ModsOfPantheon/ModTemplate/blob/master/ModTemplate/ModMain.cs#L5). This template has overriden OnInitializeMelon, which is called when your mod is first loaded.
This project comes with an [example hook](https://github.com/ModsOfPantheon/ModTemplate/blob/master/ModTemplate/Hooks/PlayerNetworkStartHook.cs#L20) which will fire when a player is created in the scene. This checks to see if this player is the local player and if it is, it then sends a message to the console using `MelonLogger`.
This project has a [NullableAttribute class](https://github.com/ModsOfPantheon/ModTemplate/blob/master/ModTemplate/NullableAttribute.cs#L3) to fix an error that occurs without it. Your IDE may suggest that you change the namespace of this class, but you must leave it as is.
