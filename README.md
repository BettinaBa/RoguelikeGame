# RoguelikeGame

Add the AI Navigation package through Unity's Package Manager.

In Package Manager, select **AI Navigation**.
Expand **Samples** and import **NavMesh Components 2D** (or similarly named).
The import creates a folder under `Assets/Samples/AI Navigation/…` containing `NavMeshBuilder2D` and related scripts.
`NavMeshBuilder2D` and `NavMeshSurface2D` will then appear in the Add Component menu.

## Baking a NavMesh
1. Open your scene and select the GameObject that spawns floor tiles (e.g., `RoomGenerator` or a dungeon generator).
2. Add a **NavMeshSurface2D** component to this object and keep the default settings.
3. Also add the provided **NavMeshBaker** component on the same GameObject so the mesh is rebuilt after generation.
4. Open **Window → AI → Navigation** and, on the **Bake** tab, click **Bake**.
5. Ensure all characters with `NavMeshAgent` components start on the baked surface so calls to `SetDestination` succeed.
