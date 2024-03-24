SampleScene.unity - game scene.
LevelEditor.unity - level editor scene.

Level Editor.unity

<img width="950" alt="Screenshot 2024-03-24 at 16 13 16" src="https://github.com/doshikkuzkin/Elements/assets/60369516/a5051f4a-2466-4bf2-a3d8-42e6c392e988">

How to use:
1. To create new level:
- enter playmode
- enter columns and rows count
- click "generate"
- click on empty cells to change blocks types
- enter level number
- click "save"
- add created level config to Addressables "Playfield" group

2. To edit existing level:
- enter playmode
- enter level number
- click "load"
- modify
- click "save"

<img width="822" alt="Screenshot 2024-03-24 at 16 20 49" src="https://github.com/doshikkuzkin/Elements/assets/60369516/32ebe7c4-8eec-458c-85dd-69c9c08dffe7">

GameSettingsConfig in the root of Assets folder contains available levels count and block types field.

1. Levels Count - set value according to how many levels should be available in game.
2. Block Types Data - add new block Addressables key to array to create new available block type. To display valid images in level editor - add sprite for new block inside EditorCellHolder component of Assets/Prefabs/LevelEditor/Button.prefab

<img width="501" alt="Screenshot 2024-03-24 at 16 19 58" src="https://github.com/doshikkuzkin/Elements/assets/60369516/47a7dac6-7f7e-43fe-9316-dc4adacacfc9">
<img width="609" alt="Screenshot 2024-03-24 at 16 23 59" src="https://github.com/doshikkuzkin/Elements/assets/60369516/d2e2411f-7cf1-429a-9fdc-278bd406037d">

Created level configs are saved to Assets/LevelConfigs. To manually create level Scriptable Object use Create/LevelConfig.
