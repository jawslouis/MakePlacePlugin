# MakePlace Plugin

A plugin to automatically save and load housing furniture layouts for FFXIV. It can also import/export layouts from the [MakePlace](https://jawslouis.itch.io/makeplace) program since it uses the same file format.

## Contents
* [Installation](#installation)
* [Usage](#usage)
  * [Saving a layout from the game](#saving-a-layout-from-the-game)
  * [Loading a layout into the game](#loading-a-layout-into-the-game)  
* [FAQ](#faq)
* [Credits](#credits)
* [Donate](#donate)


## Installation

* You will need to use the [FFXIV Quick Launcher](https://goatcorp.github.io/) to run the game. This allows third-party plugins (such as this) to be used.
* Open the settings window by using the command `/xlsettings` in the chat, or by hitting `Esc` and selecting Dalamud Settings.
* Click on the "Experimental" tab
* Copy the following url into a new line on the Custom Plugin Repositories section: https://raw.githubusercontent.com/jawslouis/MakePlacePlugin/master/MakePlacePlugin.json  
  - Note: This plugin does not work for the CN server. [Follow the steps here to try using an older version of the plugin](https://wiki.makeplace.app/en/ffxiv/plugin/CN-Server)
* Click on the `+` button. Make sure the new entry has "Enabled" checked.
* Click the "Save and Close" button
* Open the plugins window by typing `/xlplugins` in the chat, or hit `Esc` and select Dalamud Plugins
* Search for "MakePlace Plugin" and click "Install"

## Usage
Type `/makeplace` to open the plugin screen. Most functions are only available when in Rotate Furniture mode (Housing -> Indoor/Outdoor Furnishings. Click on the Rotate button).

### Saving a layout from the game
* Make sure you are in furniture mode
* Click on `Save As`, and specify the file name. Done!

### Loading a layout into the game
* Make sure you are in *rotate* furniture mode
* Make sure the relevant floors are checked.
* Make sure all the required furniture are placed in the house
  * The plugin will not touch furniture that are in your inventory or storage
  * The plugin will not dye furniture for you.
  * If you re-load the design, all the furniture in the list should be white and not grayed out
* Click on `Load From` and select the .json design file. Sit back and wait for the placement to finish!
  * Your layout should also show up in the plugin
  * If there are crashes, check [this section of the FAQ](#why-does-the-plugin-crash-sometimes-when-applying-a-layout):    

### Interface
![Settings](screenshot.png?raw=true)  
* **Save**: Saves the layout to the current file. When first starting the plugin, this is not available since no file is specified. To save both the exterior and interior to the same file, simply save once while inside, then save to the same file again while outside.
* **Save As**: Saves the layout to the specified file.
* **Load**: Loads and applies the layout from the current file. The plugin will check the currently placed furniture in the house to see if there is a match with the layout. It also checks if there is a match for the dye color. All layout furniture with no match will be grayed out.
* **Load From**: Same as Load, but loads the layout from the specified file.
* **Placement Interval**: The time period between each furniture placement when applying the entire layout. Setting this too low (e.g. 200ms) may cause some furniture placements to be missed.
* **Label Furniture**: Shows a small tooltip over each furniture, with a button to apply the layout position to the furniture

###

## FAQ
### Help! I can't get the plugin to work.
Create a github issue or post on the [discord channel](https://discord.gg/YuvcPzCuhq) for help with troubleshooting.

### Does the plugin work if the game is not in English?
Yes! Saving, loading, and transferring layouts will work in all languages.

### Can I use this to copy layouts from other houses?
No. You can only load layouts when in furniture layout mode, in your own house.

### Does the plugin use furniture from my inventory or storage?
It uses the furniture that is already placed in the house. It won't touch your inventory or storage, so there's no worries about messing up inventory management. Also, since placing furniture binds it and makes it untradeable, it's best that the player does it directly.

### Is it safe to use the plugin?
Since all third-party plugins are not in line with the game's terms of service, there is always an inherent risk. The plugin's automated furniture placement may also be detectable by the server as there is a very short interval between placing each furniture. You can increase the time interval between each furniture placement to reduce this risk.

Overall, I believe that the danger is low since SE focuses more on ToS violations that upset the game-balance (e.g. RMT). You can also reduce your risk by keeping a low profile and not mentioning the use of the plugin within the game. 

### How do I update the plugin?
When an update is available, simply return to the plugins window and click on the "Update Plugins" button.

### Why does the furniture snap back to the floor/wall after I apply a layout and exit the furnishing menu?
Items have a minimum float distance, below which they will snap back to the floor. Similarly, wall-mounted items have a minimum distance they need to be from a wall or partition, otherwise they will snap to it. Unfortunately, this is a game limitation and you will need to adjust your design.

If using the MakePlace app, you can enable `Minimum Float Distance` in settings to get a visual indicator when placing furniture.

### Why does the plugin crash sometimes when applying a layout?
Make sure of the following:
- Mouse cursor is not hovering over any item (even if the item is behind the plugin UI). When applying a layout, hovering over an item may cause some game code to conflict with the placement process. 
- Character is not in the way of where furniture will be placed
- No more than 10 items attached to another (e.g. table-top items on tables, wall-mounted items on partition walls)

There is unfortunately no solution to the above issues, since this is tied to how the game operates.

## Credits
This plugin builds upon the foundation laid by 3 other great plugins: [HousingPos](https://github.com/Bluefissure/HousingPos), [BDTH](https://github.com/LeonBlade/BDTHPlugin) and [HouseMate](https://github.com/lmcintyre/Housemate). The UI in particular borrows heavily from HousingPos.

## Donate
Thank you for using the plugin. If you enjoy my work and wish to support me, you can use the below links:

Ko-fi: [https://ko-fi.com/jawslouis](https://ko-fi.com/jawslouis)

Patreon: [https://www.patreon.com/jawslouis](https://www.patreon.com/jawslouis)

Monthly supporters get the special Patron role on Discord, which allows you to view the list of upcoming features and vote on feature prioritization
