# DialogueDesignerUnity
Unity runtime package for the Dialogue Designer tool by radmatt (https://radmatt.itch.io/dialogue-designer).

This package provides a parser to parse Dialogue Designer JSON files in Unity and a player to assist in playing them back in-game.

## Installation
Add this pair to the `dependencies` in your project's `Packages/manifest.json`:

    "com.brianmacintosh.dialoguedesigner": "https://github.com/BrianMacIntosh/DialogueDesignerUnity.git",

## Loading Dialogues
You should place the raw JSON files exported by Dialogue Designer in your Unity Assets folder so they will be imported as TextAssets.

At runtime, you can parse a TextAsset into a `DD.Dialogue` by calling `DD.Dialogue.FromAsset`.

## Playing Dialogues
To play a dialogue, create a `new DD.DialoguePlayer` and pass it a loaded `DD.Dialogue`.

Second, subscribe to any events on the Dialogue Player you would like to handle.

Finally, call `Play()` on the Player. This will advance the player to the first node.

### Dialogue Player Callbacks
`DD.DialoguePlayer` has several events you can subscribe to to handle various aspects of dialogue running. Each has a static "Global" version and a non-static "Override version". Subscribe to the Global events when you want to handle events for any dialogue the same way. Subscribe to the Override events when you need special handling for a particular Dialogue Player.

#### OnShowMessage
Called when a "Show Message" node is reached. You should use this to update the display in your interface. Advance the dialogue when the player has appropriately interacted with it using `DialoguePlayer.AdvanceMessage()`. If the node is a `DD.ShowMessageNodeChoice`, pass the choice the player made, otherwise, it doesn't matter what number is passed.

#### OnExecuteScript
Called when an "Execute" node is reached. Your game may parse and execute the script via any method you implement.

#### OnEvaluateCondition
Called when any condition string needs to be evaluated. Your game may parse and evaluate the condition by any method you implement.
