# V Rising Texture Replacer
This is a framework that enables player texture replacement in V Rising.
Included are a few sample textures.

**This mod is meant for single player!**

The changes are client side only, so they will probably not be visible to other players in multiplayer,
unless everyone uses the same replacement textures. Even then, I have not tested this mod in multiplayer,
so I cannot guarantee it will work without issues.
## Requirements
[BepInEx 1.733.2](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/)
## Installation
1. Put the DLL along with the Textures folder, and replacement textures in it, into the BepInEx plugins folder.
2. The plugin will create a config file in the BepInEx config folder.
This file is meant for toggling info logging of this plugin.
The default setting is "false". If you have any issue, set it to "true".
Errors and warnings will be logged regardless. The additional info tells you if and what textures have been replaced.
The info logging is disabled by default to avoid spamming the log file, but it can be helpful for debugging.
## Creating Custom Textures
- Assets are in VRising\VRising_Data\StreamingAssets\ContentArchives

- github.com/zhangjiequan/AssetStudio for browsing and finding assets and their archives that contain them

- github.com/AssetRipper/AssetRipper for extracting assets. (AssetStudio has issues with extracting normal maps!)

- github.com/nesrak1/UABEA for replacing assets. The sixth release of UABEA works. The seventh release is unstable, do not use! I have not tested UABEA for extracting assets.

Save any edited texture as PNG. The plugin will convert it to the correct format when loading it, either DXT1 or DXT5 depending on the texture type.
## Technical details
The plugin works by loading the replacement textures into memory and then patching the game's texture loading function.
The initial replacement happens on game world load. Subsequent replacements happen when the player changes equipment.
## Credits
Claude Opus 4.7 Adaptive for helping with the code and providing guidance on how to implement the texture replacement.
## License
This project is licensed under the AGPL-3.0 license.