# RoguelikeGame

This project contains a tiny prototype used for demonstration. To test player attacks:

1. Open the project in Unity 2022 or newer.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Select the **Enemy** object in the Hierarchy and verify its **Layer** is set to `Enemy`.
4. Select the **Player** object and check the **PlayerAttack** component. The **Enemy Layers** field should include the `Enemy` layer. The script now assigns this automatically if left empty.
5. Press Play and hit Space to swing at the enemy. The Console will log how many hits were detected and the enemy's remaining health.
