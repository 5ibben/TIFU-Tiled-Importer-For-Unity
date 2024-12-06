# TIFU-Tiled-Importer-For-Unity

A simple importer for using 'Tiled'(TMX) map files with unity, 
with persistent runtime data and automated tile creation.

Current version supports:
- Ortographic and Isometric maps.
- Embedded and external(.tsx) tilesets.
- Wang tiles
- Sprite flipping
- Colliders
- Custom property data for bool, int, float, color, & string.

### How to use:
Just copy the 'TIFU' directory to your unity project path.
.TMX maps will now be recognised as an asset by unity and will be imported accordingly.
Add maps by dragging and dropping them to the project window (don't forget to include images and tileset files referenced by the map, if any).
Dragging and dropping your map from the project to the 'scene' or 'heirarchy' window will automatically create the map as a Gameobject.

### Troubleshooting:
1. The importer fails.
-Make sure all external dependancy files (i.e images, .tsx files) are at the same path in the project as the tmx. file.
Alternativly, paths for images and tmx. files can be setup via the inspector for the map importer if they are located elsewere in the project.
	
2. The render order of tiles appear wrong.
-Try changing the sort mode order: (Edit>Project Settings...>Graphics)
I.e for Isometric: Transparency Sort Mode, Custom axis xyz=(0,1,0);


**Requires unity 2020.2 or higher.**
