using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRenderer : MonoBehaviour
{
    public Transform groundPrefab;
    public Transform[] wallPrefab;
    public Transform[] roofPrefab;
    Transform buildingFolder;

    private float wallHeight = 2.5f;
    private float wallWidth = 3f;

    public void Render(Building building)
    {
        buildingFolder = new GameObject("Building").transform;
        foreach (Room room in building.rooms)
        {
            RenderRoom(room);
        }
    }

    private RenderRoom(Room room)
    {
        Transform roomFolder = new GameObject("Rooms").transform;
        roomFolder.SetParent(buildingFolder);
        foreach (Floor floor in room.floors)
        {
            RenderFloor(floor, room, roomFolder);
        }
    }

    private RenderFloor(Floor floor, Room room, Transform roomFolder)
    {
        Transform floorFolder = new GameObject("Floors").transform;
        floorFolder.SetParent(roomFolder);

        for(int x = 0; room.Bounds.min.x; x < room.Bounds.max.x; x++)
        {
            for (int y = 0; y < room.Bounds.min.y; y++)
            {
                PlaceGround(x, y, floor.Level, floorFolder);

                //South Wall
                if (y == floor.Bounds.min.y)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x * -wallWidth, 0.3f * floor.Level * wallHeight, true, floorFolder, wall);
                }
                //East wall
                else if (x == floor.Bounds.min.x + floor.Bounds.size.x - 1)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x * -wallWidth - 2.5f, 0.3f * floor.Level * wallHeight, false, floorFolder, wall);
                }
                //North Wall
                else if (y == room.Bounds.min.y + room.Bounds.size.y - 1)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x * -wallWidth, floor.Level * wallHeight, true, floorFolder, wall)
                }
                //West Wall
                else if (x == room.Bounds.min.x)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x * -wallWidth, floor.Level * wallHeight, false, floorFolder, wall)
                }
            }
        }
    }

    private PlaceGround(int x, int y, int level, Transform floorFolder)
    {
        Transform f = Instantiate(groundPrefab, floorFolder.TransformPoint(new Vector3(x * -wallWidth, level * wallHeight, y * -wallWidth)), Quaternion.identity);
        f.SetParent(floorFolder);
    }

    private PlaceWall(int x, int y, int z, bool rotate, Tranform floorFolder, Tranform wall)
    {
        Transform w = Instantiate(
            wall,
            floorFolder.TransformPoint(new Vector3(x, y, z)),
            rotate ? Quartenion.Euler(0, 90, 0): Quartenion.Identity);
        w.SetParent(floorFolder);
    }

    private RenderRoof(Room room, Transform roomFolder)
    {
        //Place Roof above everyroom
        for (int x = room.Bounds.min.x; x < room.Bounds.max.x; x++)
            for (int y = room.Bounds.min.y; y < room.Bounds.max.y; y++)
            {
                PlaceRoof(x, y, room.Floors.Length, roomFolder, room.GetRoof.Type, room.GetRoof.Direction);
            }
    }

   private PlaceRoof(int x, int y, int level, Tranfrom roomFolder, RoofType type, RoofDirection direction)
   {
        Transform r;
        r = Instantiate(
            roofPrefab[(int)type],
            roomFolder.TransformPoint(new Vector3(
                x * -3f,
                level * 2.5f,
                y * -3f)),
            Quartenion.Euler(0f, rotationOffset[(int)direction].y, 0f);
   }

    Vector3[] rotationOffset = {
        new Vector3 (0f, 270f, 0f),
        new Vector3 (0f, 0f, 0f),
        new Vector3 (0f, 90, 0f),
        new Vector3 (0f, 180, 0f)
    };

}
