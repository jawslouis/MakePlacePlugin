# MakePlace Plugin

A plugin to automatically save and load housing furniture layouts for FFXIV. It can also import/export layouts from the [MakePlace](https://jawslouis.itch.io/makeplace) program since it uses the same file format.

## Installation

* You will need to use the [FFXIV Quick Launcher](https://goatcorp.github.io/) to run the game. This allows third-party plugins (such as this) to be used. 
* Open the settings window by using the command `/xlsettings` in the chat, or by hitting `Esc` and selecting Dalamud Settings.
* Click on the "Experimental" tab
* Copy the following url into a new line on the Custom Plugin Repositories section: https://raw.githubusercontent.com/jawslouis/MakePlacePlugin/master/MakePlacePlugin.json
* Click on the `+` button. Make sure the new entry has "Enabled" checked.
* Click the "Save and Close" button
* Open the plugins window by typing `/xlplugins` in the chat, or hit `Esc` and select Dalamud Plugins
* Search for "MakePlace Plugin" and click "Install"

## Usage
Type `/makeplace` to open the plugin screen. Most functions are only available when in Rotate Furniture mode.

* **Label Furniture**: Shows a small tooltip over each furniture, with a button to apply the layout position to the furniture
* **Save**: Saves the *currently loaded* layout to file. You start with a blank layout each time you enter the house, and will need to either import a layout or load the current house layout. 
* **Load**: Loads the layout from the specified file. The plugin will attempt to match the layout furniture to the current furniture in the house. It tries to match dye colors too, if possible. All layout furniture with no match will be grayed out.
* **House Layout**: Loads the current furniture layout in the house
* **Apply**: Applies the layout position to all applicable items (those that are not grayed out). 

###

## FAQ
### How do I update the plugin?
When an update is available, simply return to the plugins window and click on the "Update Plugins" button.

### Can I use this to copy layouts from other houses?
No. You can only load layouts when in furniture layout mode, in your own house.


## Donate
If you enjoy my work and wish to support me, you can use the below links. Every bit helps!

Ko-fi: [https://ko-fi.com/jawslouis](https://ko-fi.com/jawslouis)

Patreon: [https://www.patreon.com/jawslouis](https://www.patreon.com/jawslouis)
