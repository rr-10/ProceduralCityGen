using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Process
{
    NoChange,
    ShrinkRow,
    ShrinkColumn,
    ShrinkRandom,
    ApplyRoof
}

public class GenerateBuilding : MonoBehaviour
{
    //User Controller Variables 
    [SerializeField] private GameObject NormalWallPrefab;
    [SerializeField] private GameObject DoorWallPrefab;
    [SerializeField] private GameObject WindowWallPrefab;
    [SerializeField] private GameObject BalconyWallPrefab;

    [SerializeField] private GameObject FloorPrefab;
    [SerializeField] private GameObject RoofPrefab;
    [SerializeField] private Bounds Bounds; //TODO - Will be determined from the terrain data

    //TODO - Add the custom editor field for these
    [SerializeField] private int MaximumFloors = 1;
    [SerializeField] private float setDoorChance;
    public static float DoorPercentChance = 0.2f;
    [SerializeField] private float setWindowChance;
    public static float WindowPercentChance = 0.4f;
    [SerializeField] private float setBalconyChance;
    public static float BalconyPercentChance = 0.35f;

    //Internal Variables 
    private Building building;
    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    private Process ProcessToApply;

    public void Generate()
    {
        DoorPercentChance = setDoorChance;
        WindowPercentChance = setWindowChance;
        BalconyPercentChance = setBalconyChance;

        //Clear any prefab that was previously created 
        Clear();

        //Generate the buildings structure
        CreateBuilding();

        //Once the building is generated, create it in the scene 
        Render();
    }

    //This function is where the building generation is done
    private void CreateBuilding()
    {
        ProcessToApply = Process.ShrinkColumn;
        //Determine initial symbol 
        building = new Building(4, 4, transform, Quaternion.identity); //TODO : Initial Values should be determined 

        while (building.AddFloor(ProcessToApply))
        {
            //If the next floor is going to be the max number of floors, we have to force the next floor to be roofed
            if (building.NumberOfFloors == MaximumFloors - 1)
            {
                ProcessToApply = Process.ApplyRoof;
                continue;
            }

            //TODO : The grammar rules could be moved to a scriptable object allowing for easy changes to variation
            //TODO : Work on adding some more variation to the grammar rules 
            //Apply grammar rules
            switch (ProcessToApply)
            {
                case Process.NoChange:
                    ProcessToApply = Process.ShrinkColumn;
                    break;
                case Process.ShrinkColumn:
                    ProcessToApply = Process.NoChange;
                    break;
                case Process.ShrinkRow:
                    ProcessToApply = Process.ApplyRoof;
                    break;
                case Process.ShrinkRandom:
                    ProcessToApply = Process.ApplyRoof;
                    break;
                case Process.ApplyRoof:
                    break;
            }
        }
    }

    //Clear previously created buildings 
    private void Clear()
    {
        //Destroy every gameobject we spawned 
        foreach (GameObject o in spawnedPrefabs)
        {
            DestroyImmediate(o);
        }

        spawnedPrefabs.Clear(); // All gameobject in the list should now be destroyed so safe to clear
    }


    //TODO - Should also place a ground plane for each floor

    //Place all the prefabs in the scene to represent the generated building
    private void Render()
    {
        //Create folder for building
        GameObject buildingFolder = new GameObject($"Building");
        spawnedPrefabs.Add(buildingFolder);

        foreach (Floor floor in building.Floors)
        {
            //Create a folder for each floor and set parent to buildingFolder
            GameObject floorFolder = new GameObject($"Floor_{floor.FloorLevel}");
            floorFolder.transform.parent = buildingFolder.transform;

            for (int x = 0; x < floor.Rooms.GetLength(0); x++)
            {
                for (int y = 0; y < floor.Rooms.GetLength(1); y++)
                {
                    Room room = floor.Rooms[x, y]; //Get the room 

                    //3d position of room relative to 
                    Vector3 roomPosition = new Vector3(room.Position.x, floor.FloorLevel, room.Position.y);

                    
                    
                    
                    //Draw the room only if it has walls that need drawing
                    if (room.IsInterior)
                    {
                        
                        PlaceFloor(floorFolder.transform, roomPosition);
                        if (room.Walls != null)
                        {
                            foreach (Wall wall in room.Walls)
                            {
                                PlaceWall(wall, floorFolder.transform,roomPosition);
                            }
                        }
                        
                        if (room.HasRoof)
                        {
                            PlaceRoof(room.RoomRoof, floorFolder.transform, roomPosition);
                        }
                    }
                    
                }
            }
        }
    }

    private void PlaceFloor(Transform parentTransform, Vector3 position)
    {
        Vector3 setPosition = new Vector3();
        setPosition.x = position.x * 2;
        setPosition.y = (position.y * 3);
        setPosition.z = position.z * 2;
        
        
        SpawnPrefab(FloorPrefab, parentTransform, setPosition, Quaternion.identity);
    }
    

    private void PlaceRoof(Roof roof, Transform parentTransform, Vector3 position)
    {
        Vector3 setPosition = new Vector3();
        setPosition.x = position.x * 2;
        setPosition.y = (position.y * 3) + 3;
        setPosition.z = position.z * 2;
        
        
        SpawnPrefab(RoofPrefab, parentTransform, setPosition, Quaternion.identity);
    }


    private void PlaceWall(Wall wall, Transform parentTransform, Vector3 position)
    {
        Vector3 setPostion = new Vector3();
        setPostion.y = position.y * 3;
        
        switch (wall.Side)
        {
            case WallSide.North:
                setPostion.x = position.x * 2;
                setPostion.z = position.z * 2;
                
                break;
            case WallSide.East:
                setPostion.x = position.x * 2;
                setPostion.z = (position.z * 2) + 2;
                break;
            case WallSide.South:
                setPostion.x = (position.x * 2) + 2;
                setPostion.z = (position.z * 2) + 2;
                break;
            case WallSide.West:
                setPostion.x = (position.x * 2) + 2;
                setPostion.z = (position.z * 2);
                break;
        }

        //Spawn all the walls of the room
        switch (wall.Type)
        {
            case WallType.Normal:
                SpawnPrefab(NormalWallPrefab, parentTransform, setPostion,
                    Quaternion.Euler(0.0f, (float) wall.Side, 0.0f));
                break;
            case WallType.Door:
                SpawnPrefab(DoorWallPrefab, parentTransform, setPostion, Quaternion.Euler(0.0f, (float) wall.Side, 0.0f)
                );
                break;
            case WallType.Window:
                SpawnPrefab(WindowWallPrefab, parentTransform, setPostion,
                    Quaternion.Euler(0.0f, (float) wall.Side, 0.0f));
                break;
            case WallType.Balcony:
                SpawnPrefab(BalconyWallPrefab, parentTransform, setPostion,
                    Quaternion.Euler(0.0f, (float) wall.Side, 0.0f));
                break;
        }
    }


    private void SpawnPrefab(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(prefab, transform.position + position, rotation);
        go.transform.parent = parent;
    }
}