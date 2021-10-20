using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRenderer : MonoBehaviour
{
    public Transform groundPrefab;
    public Transform[] wallPrefab;
    public Transform[] roofPrefab;
    Transform buildingFolder;

    private float wallHeight = 3f;
    private float wallWidth = 2f;

    public void Render(Building building)
    {
        buildingFolder = new GameObject("Building").transform;
        foreach (Room room in building.Rooms)
        {
            RenderRoom(room);
        }
    }

    private void RenderRoom(Room room)
    {
        Transform roomFolder = new GameObject("Rooms").transform;
        roomFolder.SetParent(buildingFolder);
        foreach (Floor floor in room.Floors)
        {
            RenderFloor(floor, room, roomFolder);
        }
        RenderRoof(room, roomFolder);
    }

    private void RenderFloor(Floor floor, Room room, Transform roomFolder)
    {
        Transform floorFolder = new GameObject("Floors").transform;
        floorFolder.SetParent(roomFolder);

        for (int x = room.Bounds.min.x; x < room.Bounds.max.x; x++)
        {
            for (int y = room.Bounds.min.y; y < room.Bounds.max.y; y++)
            {
                Debug.Log("X: " + x + "   Y: " + y);

                PlaceGround(x, y, floor.Level, floorFolder);

                //South Wall
                if (y == room.Bounds.min.y)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x, floor.Level * wallHeight,y ,false , floorFolder, wall);
                }
                //East wall
                if (x == room.Bounds.min.x + room.Bounds.size.x - 1)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x + (wallWidth / 2), floor.Level * wallHeight,y + (wallWidth / 2) ,true , floorFolder, wall);
                }
                //North Wall
                if (y == room.Bounds.min.y + room.Bounds.size.y - 1)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x, floor.Level * wallHeight,y + (wallWidth / 2) ,false , floorFolder, wall);
                }
                //West Wall
                if (x == room.Bounds.min.x)
                {
                    Transform wall = wallPrefab[(int)floor.Walls[x - room.Bounds.min.x]]; //Get the index of the wall at this position
                    PlaceWall(x, floor.Level * wallHeight,y + (wallWidth / 2),true , floorFolder, wall);
                }
            }
        }
    }

    private void PlaceGround(int x, int y, int level, Transform floorFolder)
    {
        Transform f = Instantiate(groundPrefab, floorFolder.TransformPoint(new Vector3(x * wallWidth, level * wallHeight, y * wallWidth)), Quaternion.identity);
        f.SetParent(floorFolder);
    }

    private void PlaceWall(float x, float y, float z, bool rotate, Transform floorFolder, Transform wall)
    {
        Transform w = Instantiate(
            wall,
            floorFolder.TransformPoint(new Vector3(x * wallWidth, y, z * wallWidth)),
            rotate ? Quaternion.Euler(0, 90, 0): Quaternion.identity);
        w.SetParent(floorFolder);
        w.name = ("X: " + x + "   Y: " + z);
    }

    private void RenderRoof(Room room, Transform roomFolder)
    {
        //Place Roof above everyroom
        for (int x = room.Bounds.min.x; x < room.Bounds.max.x; x++)
            for (int y = room.Bounds.min.y; y < room.Bounds.max.y; y++)
            {
                PlaceRoof(x + (wallWidth / 4) , y + (wallWidth / 4) , room.Floors.Length, roomFolder, room.GetRoof.Type, room.GetRoof.Direction);
            }
    }

   private void PlaceRoof(float x, float y, int level, Transform roomFolder, RoofType type, RoofDirection direction)
   {
        Transform r;
        r = Instantiate(
            roofPrefab[(int)type],
            roomFolder.TransformPoint(new Vector3(
                x * wallWidth,
                level * wallHeight,
                y * wallWidth)),
            Quaternion.Euler(0f, rotationOffset[(int)direction].y, 0f));
   }

    Vector3[] rotationOffset = {
        new Vector3 (0f, 270f, 0f),
        new Vector3 (0f, 0f, 0f),
        new Vector3 (0f, 90f, 0f),
        new Vector3 (0f, 180f, 0f)
    };

}
