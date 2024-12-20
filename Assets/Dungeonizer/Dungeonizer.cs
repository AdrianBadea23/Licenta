﻿/*
Thank you for buying Dungeonizer. I hope you will enjoy it.

My aim was to create a dungeon generator that can be used in 2D and 3D games. 
And i never used too much complicated algorithms to make this one easy to modify and understand.

So feel free to hack it and make it better.

And join our discord channel for support and discussions.
https://discord.com/invite/fWjQWbkfQB

Have fun!
Mert
*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.AI;

namespace Dungeonizer { 
	public class Room {
		public int x = 0;
		public int y = 0;
		public int w = 0;
		public int h = 0;
		public Room connectedTo = null;
		public int branch = 0;
		public string relative_positioning = "x";
		public bool dead_end = false;
		public int room_id = 0;

		public int room_height = 0;
	}

	public class SpawnList {
		public int x;
		public int y;
		public bool byWall;
		public string wallLocation;
		public bool inTheMiddle;
		public bool byCorridor;

		public int asDoor = 0;
		public Room room = null;
		public bool spawnedObject;
	}

	[System.Serializable]
	public class SpawnOption {
		public int minSpawnCount;
		public int maxSpawnCount;
		public bool spawnByWall;
		public bool spawmInTheMiddle;
		public bool spawnRotated;
		//public bool byCorridor;
		[Tooltip("This is for make spawned object will be higher than ground.")]
		public float heightFix = 0;

		public GameObject gameObject;
		[Tooltip("Use 0 for random room, make sure spawn room isnt bigger than your room count")]
		public int spawnRoom = 0;
	}

	[System.Serializable]
	public class CustomRoom {
		[Tooltip("make sure room id isnt bigger than your room count")]
		public int roomId = 1;
		public GameObject floorPrefab;
		public GameObject wallPrefab;
		public GameObject doorPrefab;
		public GameObject cornerPrefab;
	}

	public class MapTile {
		public int x = 0;
		public int y = 0;
		public int z = 0;

		public int type = 0; //Default = 0 , Room Floor = 1, Wall = 2, Corridor Floor 3, Room Corners = 4, 5, 6 , 7
		public int orientation = 0;
		public Room room = null;   
		public int tile_height = 0;

		public bool isCorner = false;
		public bool isEdge = false;
		public bool isDoor = false;
		public int doorDirection = 0;
		public bool byCorridor = false;
		public string edgeLocation = "";

		public string debugInfo = "";

		public bool isWall(){
			if(this.type >= 8 && this.type <= 11) return true;
			return false;
		}
		
	}


	public class Dungeonizer : MonoBehaviour {
		[Tooltip("Select a seed. use 0 for random seed.")]
        public int seed = 0; // Use 0 for a random seed

        [Tooltip("This prefab will be instantiate on dungeons entrance. You can put your character, or character spawner here.")]
		public GameObject startPrefab;

		[Tooltip("This will be end of level. ")]
		public GameObject exitPrefab;

		public List<SpawnList> spawnedObjectLocations = new List<SpawnList>();
		public GameObject floorPrefab;
		public GameObject wallPrefab;
		public GameObject doorPrefab;
		//public GameObject doorCorners;
		public GameObject corridorFloorPrefab;
		public GameObject corridorWallPrefab;

		public GameObject cornerPrefab;
		public bool cornerRotation = false;

		public int maximumRoomCount = 10;

		[Tooltip("Min gap between rooms. Also affects corridor lengths ")]
		public int minimumRoomMargin = 0;

		[Tooltip("Maximum gap between rooms. Also affects corridor lengths ")]
		public int roomMargin = 3;
		[Tooltip("If Checked: makes dungeon reset on every time level loads.")]	
		public bool generate_on_load = true;
		public int minRoomSize = 5;
		public int maxRoomSize = 10;
		
		[Tooltip("How big are your tiles? (Affects corridor and room sizes)")]
		public float tileScaling = 1f;
		public List<SpawnOption> spawnOptions = new List<SpawnOption>();
		public List<CustomRoom> customRooms = new List<CustomRoom> ();
		public bool makeIt3d = false;

		//public NavMeshSurface surface;

		class Dungeon {
			public int seed;
			public static int map_size;
			public static int map_size_x;
			public static int map_size_y;


			public static List<MapTile> map;

			public static List<Room> rooms = new List<Room>();
			
			public static Room goalRoom;
			public static Room startRoom;
			
			public int min_size;
			public int max_size;
			
			public int maximumRoomCount;
			public int minimumRoomMargin;
			public int roomMargin;
			public int roomMarginTemp;

			//tile types for ease
			public static List<int> roomsandfloors = new List<int> { 1, 3 };
			public static List<int> corners = new List<int> {4,5,6,7};
			public static List<int> walls = new List<int> {8,9,10,11};
			public static List<int> corridor_walls = new List<int> {108,109,1010,1011};
			private static List<string> directions = new List<string> {"x","y","-y","-x"}; //,"-y"};
			
			public MapTile createTile(int type, int x, int y, Room room = null){
				MapTile newRoomTile = new MapTile();
				newRoomTile.type = type;
				newRoomTile.room = room;
				newRoomTile.x = x;
				newRoomTile.y = y;
				if(room != null) {
					newRoomTile.z = room.room_height;
					newRoomTile.tile_height = room.room_height;
				}
				return newRoomTile;
			}

			public void Generate() {
                if (seed == 0)
                {
                    seed = Random.Range(int.MinValue, int.MaxValue);
                }
                Random.InitState(seed);

                int room_count = this.maximumRoomCount;
				int min_size = this.min_size;
				int max_size = this.max_size;
				map = new List<MapTile>();
				rooms = new List<Room> ();
				
				int collision_count = 0;
				string direction = "set";
				string oldDirection = "set";
				Room lastRoom;


				for (var i = 0; i < room_count; i++) {
					Room room = new Room ();
					if (rooms.Count == 0) {
						//first room
						room.x = (int)Mathf.Floor (map_size / 2f);
						room.y = (int)Mathf.Floor (map_size / 2f);

						room.w = Random.Range (min_size, max_size);
						if (room.w % 2 == 0) room.w += 1;
						room.h = Random.Range (min_size, max_size);
						if (room.h % 2 == 0) room.h += 1;

						room.branch = 0;
						lastRoom = room;
					} else {
						int branch = 0;
						if (collision_count == 0) {
							branch = Random.Range (5, 20); //complexity
						}
						room.branch = branch;

						if(rooms.Count > 1){
							lastRoom = rooms [rooms.Count - 1];
						}
						else{
							lastRoom = rooms[0];
						}
						int lri = 1;

						while (lastRoom.dead_end) {
							lastRoom = rooms [rooms.Count - lri++];
						}


						if (direction == "set") {
							string newRandomDirection = directions[Random.Range(0, directions.Count)];
							direction = newRandomDirection;
							while (direction == oldDirection)
							{
								newRandomDirection = directions[Random.Range(0, directions.Count)];
								direction = newRandomDirection;
							}
						}
						this.roomMarginTemp = Random.Range(0, this.roomMargin - 1);

						room.w = Random.Range (min_size, max_size);
						if (room.w % 2 == 0) room.w += 1;

						room.h = Random.Range (min_size, max_size);
						if (room.h % 2 == 0) room.h += 1;
						


						if (direction == "y") {
							room.x = lastRoom.x + lastRoom.w + this.roomMarginTemp + this.minimumRoomMargin + 2;
							room.y = lastRoom.y;
						} else if (direction == "-y") {
							room.x = lastRoom.x - room.w  - this.roomMarginTemp - this.minimumRoomMargin - 2;
							room.y = lastRoom.y;
						} else if (direction == "x") {
							room.y = lastRoom.y + lastRoom.h  + this.roomMarginTemp + this.minimumRoomMargin + 2;
							room.x = lastRoom.x;
						} else if (direction == "-x") {
							room.y = lastRoom.y - room.h - this.roomMarginTemp - this.minimumRoomMargin - 2 ;
							room.x = lastRoom.x;
						}
						room.room_height = roomMarginTemp;



						
						//Debug.Log(room.room_id + "- x: " + room.x + " y: " + room.y);

						room.connectedTo = lastRoom;
					}

					
					bool doesCollide = this.DoesCollide (room, 0);				
					if (doesCollide) {
						i--;
						collision_count += 1;
						if (collision_count > 3) {
							lastRoom.branch = 1;
							lastRoom.dead_end = true;
							collision_count = 0;
						} else {
							oldDirection = direction;
							direction = "set";
						}
					} else {
						room.room_id = i;
						rooms.Add (room);
						oldDirection = direction;
						direction = "set";
					}
					
				}

				//room making
				for (int i = 0; i < rooms.Count; i++) {
					Room room = rooms [i];
					for (int x = room.x; x < room.x + room.w; x++) {       
						for (int y = room.y; y < room.y + room.h; y++) {
							MapTile newRoomTile = new MapTile();
							newRoomTile.type = 1;
							newRoomTile.room = room;
							newRoomTile.x = x;
							newRoomTile.y = y;
							newRoomTile.z = room.room_height;
							newRoomTile.tile_height = room.room_height;

							//mark edges:
							if(y == room.y + room.h - 1) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "n"; }
							if(y == room.y) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "s"; }
							if(x == room.x) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "w"; }
							if(x == room.x + room.w - 1) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "e"; }

							//mark corners:
							if(x == room.x && y == room.y){ map.Add(this.createTile(4, room.x - 1, room.y - 1, room)); }
							if(x == room.x && y == room.y + room.h - 1){ map.Add(this.createTile(5, room.x - 1, room.y + room.h, room)); }
							if(x == room.x + room.w - 1 && y == room.y){ map.Add(this.createTile(7, room.x + room.w, room.y - 1 , room)); }
							if(x == room.x + room.w - 1 && y ==  room.y + room.h - 1){ map.Add(this.createTile(6, room.x + room.w , room.y + room.h, room)); }

							map.Add(newRoomTile);
						}
					}

					/* these 4 loops creates room walls */
					for(int j = 0; j < room.h; j++ ){
						map.Add(createTile(11, room.x -1, room.y + j, room));
					}

					for(int j = 0; j < room.w; j++ ){
						map.Add(createTile(10, room.x + j, room.y - 1, room));
					}	

					for(int j = 0; j < room.h; j++ ){
						map.Add(createTile(9, room.x + room.w, room.y + j, room));
					}	

					for(int j = 0; j < room.w; j++ ){
						map.Add(createTile(8, room.x + j, room.y + room.h, room));
					}				
				}

				
				//find far far away room
				goalRoom = rooms[rooms.Count -1 ];
				//starting point
				startRoom = rooms[0];

				
				//corridor making
				for (int i = 0; i < rooms.Count; i++) {
					Room roomA = rooms [i];
					Room roomB = rooms [i].connectedTo;

					if (roomB != null) {
						var pointA = new Room (); //start
						var pointB = new Room ();
						bool horizontalCorridor = false;
						bool nextTileBlocksDoor = false;

						
						// Created door count for this corridor
						int doorCount = 0;
						int doorDirection = 0;
						
						pointA.x = roomA.x + (int)Mathf.Floor (roomA.w / 2); // First Room Center X
						pointB.x = roomB.x + (int)Mathf.Floor (roomB.w / 2); // Second Room Center X

						pointA.y = roomA.y + (int)Mathf.Floor (roomA.h / 2); // First Room Center Y
						pointB.y = roomB.y + (int)Mathf.Floor (roomB.h / 2); // Second Room Center Y

						if (Mathf.Abs (pointA.x - pointB.x) > Mathf.Abs (pointA.y - pointB.y)) {
							//horizontal
							horizontalCorridor = true;
							if (roomA.h > roomB.h) {
								pointA.y = pointB.y;
							} else {
								pointB.y = pointA.y;
							}						
						} else {
							//vertical
							if (roomA.w > roomB.w) {
								pointA.x = pointB.x;
							} else {
								pointB.x = pointA.x;
							}					
						}

						MapTile currentTile = null;
						while ((pointB.x != pointA.x) || (pointB.y != pointA.y)) {						
							// So dungeonizer starts from one room's center and goes to other room's center tile by tile.
							// And it creates a corridor.
							// This currentDirection means which direction we are going to create a corridor tile.
							// When its created it doesnt matter if its created left to right or right to left.
							// But we need to know which direction we are going to create next tile and how to rotate doors etc.
							
							int currentDirection = 0;
							if (horizontalCorridor) { 
								if (pointB.x > pointA.x) {
									pointB.x--;
									currentDirection = 4;
								} else {
									pointB.x++;
									currentDirection = 2;
								}
							} else {
								if (pointB.y > pointA.y) {
									currentDirection = 1;
									pointB.y--;
								} else {
									currentDirection = 3;
									pointB.y++;
								}
							}

							//This code checks if corridor hits a wall. Also saves it for later to create door if needed.
							//That means Dungeonizer will try not to spawn anything blocks corridors and doors.					
							bool isWall = map.Find(item => (item.x == pointB.x && item.y == pointB.y && item.isWall())) != null;
							map.RemoveAll(item => (item.x == pointB.x && item.y == pointB.y && item.isWall() ));

							if(isWall && currentTile != null){
								currentTile.byCorridor = true; //this is actually previous tile
							}

							//dont spawn anything if there is a floor already
							currentTile = map.Find(item => (pointB.x == item.x && pointB.y == item.y && item.type == 1));
							if(currentTile != null && nextTileBlocksDoor)
							{
								currentTile.byCorridor = true;
								nextTileBlocksDoor = false;
								continue;
							}
							else if(currentTile != null) {
								continue;
							}


							
							MapTile newCorridorTile = new MapTile();
							newCorridorTile.type = 3;
							//newCorridorTile.room = room;
							newCorridorTile.x = pointB.x;
							newCorridorTile.y = pointB.y;
							if(isWall){							
								// if this is the first door:
								doorCount++;
								if(doorCount == 1){
									doorDirection = currentDirection;
								}
								else {
									doorDirection = currentDirection + 2;
								}
								nextTileBlocksDoor = true; //noting this because we want some items to spawn and block corridor entrances.			
								newCorridorTile.isDoor = true; //this tile could be a door?
								newCorridorTile.doorDirection = doorDirection;
								newCorridorTile.debugInfo = " New Corridor Tile " + newCorridorTile.x + " " + newCorridorTile.y + " " + newCorridorTile.doorDirection;

							}
							map.Add(newCorridorTile);
							

							//Corridor wall locations
							if(horizontalCorridor){			
								currentTile = map.Find(item => (pointB.x == item.x && pointB.y + 1 == item.y && Dungeon.walls.Contains(item.type)));
								if(currentTile == null) {
									map.Add(createTile(108, pointB.x, pointB.y + 1));
								}

								currentTile = map.Find(item => (pointB.x == item.x && pointB.y - 1  == item.y && Dungeon.walls.Contains(item.type)));
								if(currentTile == null)
								{
									map.Add(createTile(1010, pointB.x, pointB.y - 1));
								}							
							}
							else {
								currentTile = map.Find(item => (pointB.x + 1 == item.x && pointB.y == item.y && Dungeon.walls.Contains(item.type)));
								if(currentTile == null) {
									map.Add(createTile(109, pointB.x + 1, pointB.y));
								}
								currentTile = map.Find(item => (pointB.x - 1 == item.x && pointB.y == item.y && Dungeon.walls.Contains(item.type))); 
								if(currentTile == null) {
									map.Add(createTile(1011, pointB.x - 1, pointB.y));
								}
							}
							


						} 

					}
				}
				
			}

			private bool DoesCollide (Room room, int ignore) {
				int random_blankliness = 0;

				for (int i = 0; i < rooms.Count; i++) {
					//if (i == ignore) continue;
					var check = rooms[i];
					if ( 
						!((room.x + room.w + random_blankliness < check.x) ||
						(room.x > check.x + check.w + random_blankliness) || 
						(room.y + room.h + random_blankliness < check.y) || 
						(room.y > check.y + check.h + random_blankliness)))
						return true;
				}
				
				return false;
			}
	

			private float lineDistance( Room point1, Room point2 )
			{
				var xs = 0;
				var ys = 0;
				
				xs = point2.x - point1.x;
				xs = xs * xs;
				
				ys = point2.y - point1.y;
				ys = ys * ys;
				
				return Mathf.Sqrt( xs + ys );
			}



		}

		public void ClearOldDungeon(bool immediate = false)
		{
			int childs = transform.childCount;
			for (var i = childs - 1; i >= 0; i--)
			{
				if(immediate){
					DestroyImmediate(transform.GetChild(i).gameObject);
				}
				else {
					Destroy(transform.GetChild(i).gameObject);
				}
			}
		}


        public void Generate()
        {
            SetupDungeon();
            InstantiateTiles();
            InstantiateStartAndEndPoints();
            List<SpawnList> spawnedObjectLocations = FindSpawnLocations();
            ShuffleSpawnLocations(spawnedObjectLocations);
            SpawnObjects(spawnedObjectLocations);
            InstantiateDoors(spawnedObjectLocations);
        }

        // Sets up the dungeon parameters and generates the dungeon layout
        private void SetupDungeon()
        {
            Dungeon dungeon = new Dungeon();

            dungeon.seed = this.seed;
            dungeon.min_size = minRoomSize;
            dungeon.max_size = maxRoomSize;
            dungeon.maximumRoomCount = maximumRoomCount;
            dungeon.roomMargin = roomMargin;
            dungeon.minimumRoomMargin = minimumRoomMargin;

            dungeon.Generate(); // Calculates all static object locations (walls, corridors, doors, corners etc.)
        }

        // Instantiates the tiles (floors, walls, corridors, corners) based on the dungeon map
        private void InstantiateTiles()
        {
            foreach (MapTile mapTile in Dungeon.map)
            {
                int tile = mapTile.type;
                int tile_height = mapTile.tile_height;

                int orientation = mapTile.orientation;
                GameObject created_tile = null;
                Vector3 tile_location;

                if (!makeIt3d)
                {
                    tile_location = new Vector3(mapTile.x * tileScaling, mapTile.y * tileScaling, 0);
                }
                else
                {
                    tile_location = new Vector3(mapTile.x * tileScaling, 0, mapTile.y * tileScaling);
                }

                created_tile = null;
                if (tile == 1)
                {
                    GameObject floorPrefabToUse = GetFloorPrefab(mapTile);
                    created_tile = Instantiate(floorPrefabToUse, tile_location, Quaternion.identity) as GameObject;
                }

                if (Dungeon.walls.Contains(tile) || (Dungeon.corridor_walls.Contains(tile) && !corridorWallPrefab))
                {
                    GameObject wallPrefabToUse = GetWallPrefab(mapTile);
                    created_tile = Instantiate(wallPrefabToUse, tile_location, Quaternion.identity) as GameObject;

                    RotateWall(created_tile, tile);
                }
                else if (Dungeon.corridor_walls.Contains(tile))
                {
                    GameObject wallPrefabToUse = GetCorridorWallPrefab(mapTile);
                    created_tile = Instantiate(wallPrefabToUse, tile_location, Quaternion.identity) as GameObject;

                    RotateWall(created_tile, tile);
                }

                if (tile == 3)
                {
                    GameObject floorPrefabToUse = corridorFloorPrefab ? corridorFloorPrefab : floorPrefab;
                    created_tile = Instantiate(floorPrefabToUse, tile_location, Quaternion.identity) as GameObject;

                    if (orientation == 1 && makeIt3d)
                    {
                        created_tile.transform.Rotate(Vector3.up * (-90));
                    }
                }

                if (Dungeon.corners.Contains(tile))
                {
                    GameObject cornerPrefabToUse = GetCornerPrefab(mapTile);

                    if (cornerPrefabToUse)
                    {
                        created_tile = Instantiate(cornerPrefabToUse, tile_location, Quaternion.identity) as GameObject;
                        RotateCorner(created_tile, tile);
                    }
                    else
                    {
                        created_tile = Instantiate(wallPrefab, tile_location, Quaternion.identity) as GameObject;
                    }
                }

                if (created_tile)
                {
                    created_tile.transform.parent = transform;
                }
            }
        }

        // Instantiates the start and end points of the dungeon
        private void InstantiateStartAndEndPoints()
        {
            GameObject end_point;
            GameObject start_point;

            if (!makeIt3d)
            {
                end_point = Instantiate(exitPrefab, new Vector3(Dungeon.goalRoom.x * tileScaling, Dungeon.goalRoom.y * tileScaling, 0), Quaternion.identity) as GameObject;
                start_point = Instantiate(startPrefab, new Vector3(Dungeon.startRoom.x * tileScaling, Dungeon.startRoom.y * tileScaling, 0), Quaternion.identity) as GameObject;
            }
            else
            {
                end_point = Instantiate(exitPrefab, new Vector3((Dungeon.goalRoom.x + Mathf.FloorToInt(Dungeon.goalRoom.w / 2)) * tileScaling, 0, (Dungeon.goalRoom.y + Mathf.FloorToInt(Dungeon.goalRoom.h / 2)) * tileScaling), Quaternion.identity) as GameObject;
                start_point = Instantiate(startPrefab, new Vector3((Dungeon.startRoom.x + Mathf.FloorToInt(Dungeon.startRoom.w / 2)) * tileScaling, 0, (Dungeon.startRoom.y + Mathf.FloorToInt(Dungeon.startRoom.h / 2)) * tileScaling), Quaternion.identity) as GameObject;
            }

            end_point.transform.parent = transform;
            start_point.transform.parent = transform;
        }

        // Finds suitable locations to spawn objects and returns a list of spawn locations
        private List<SpawnList> FindSpawnLocations()
        {
            List<SpawnList> spawnedObjectLocations = new List<SpawnList>();

            foreach (MapTile mapTile in Dungeon.map)
            {
                if (mapTile.type == 1
                    // Do not spawn anything on player's start location or finish
                    && !(mapTile.x == Dungeon.startRoom.x + Mathf.FloorToInt(Dungeon.startRoom.w / 2) && mapTile.y == Dungeon.startRoom.y + Mathf.FloorToInt(Dungeon.startRoom.h / 2))
                    && !(mapTile.x == Dungeon.goalRoom.x + Mathf.FloorToInt(Dungeon.goalRoom.w / 2) && mapTile.y == Dungeon.goalRoom.y + Mathf.FloorToInt(Dungeon.goalRoom.h / 2)))
                {
                    SpawnList location = new SpawnList
                    {
                        byWall = mapTile.isEdge,
                        wallLocation = mapTile.edgeLocation,
                        x = mapTile.x,
                        y = mapTile.y,
                        byCorridor = mapTile.byCorridor,
                        room = mapTile.room
                    };

                    int roomCenterX = (int)Mathf.Floor(location.room.w / 2) + location.room.x;
                    int roomCenterY = (int)Mathf.Floor(location.room.h / 2) + location.room.y;

                    if (mapTile.x == roomCenterX + 1 && mapTile.y == roomCenterY + 1)
                    {
                        location.inTheMiddle = true;
                    }

                    spawnedObjectLocations.Add(location);
                }
                else if (mapTile.type == 3)
                {
                    if (mapTile.isDoor)
                    {
                        SpawnList location = new SpawnList
                        {
                            x = mapTile.x,
                            y = mapTile.y,
                            byCorridor = true,
                            asDoor = mapTile.doorDirection,
                            room = mapTile.room
                        };
                        spawnedObjectLocations.Add(location);
                    }
                }
            }

            return spawnedObjectLocations;
        }

        // Shuffles the list of spawn locations
        private void ShuffleSpawnLocations(List<SpawnList> spawnedObjectLocations)
        {
            for (int i = 0; i < spawnedObjectLocations.Count; i++)
            {
                SpawnList temp = spawnedObjectLocations[i];
                int randomIndex = Random.Range(i, spawnedObjectLocations.Count);
                spawnedObjectLocations[i] = spawnedObjectLocations[randomIndex];
                spawnedObjectLocations[randomIndex] = temp;
            }
        }

        // Spawns objects at suitable locations based on spawn options
        private void SpawnObjects(List<SpawnList> spawnedObjectLocations)
        {
            int objectCountToSpawn = 0;

            // Uncomment the following lines if you are going to use dynamic pathfinding
            // and have a NavMeshSurface component attached to the same gameobject.
            //surface = GetComponent<NavMeshSurface>();
            //surface.BuildNavMesh();

            // Now instantiating gameobjects wanted to "spawn"
            foreach (SpawnOption objectToSpawn in spawnOptions)
            {
                objectCountToSpawn = Random.Range(objectToSpawn.minSpawnCount, objectToSpawn.maxSpawnCount);
                while (objectCountToSpawn > 0)
                {
                    bool created = false;

                    for (int i = 0; i < spawnedObjectLocations.Count; i++)
                    {
                        if (CanSpawnObjectAtLocation(objectToSpawn, spawnedObjectLocations[i]))
                        {
                            SpawnList spawnLocation = spawnedObjectLocations[i];
                            GameObject newObject = InstantiateObjectAtLocation(objectToSpawn, spawnLocation);
                            newObject.transform.parent = transform;
                            spawnedObjectLocations[i].spawnedObject = newObject;
                            objectCountToSpawn--;
                            created = true;
                            break;
                        }
                    }

                    if (!created) // If can't find anywhere to put, don't put (prevents endless loops)
                    {
                        objectCountToSpawn--;
                    }
                }
            }
        }

        // Instantiates doors at door locations
        private void InstantiateDoors(List<SpawnList> spawnedObjectLocations)
        {
            if (doorPrefab)
            {
                foreach (SpawnList spawnLocation in spawnedObjectLocations)
                {
                    if (spawnLocation.asDoor > 0)
                    {
                        GameObject doorPrefabToUse = GetDoorPrefab(spawnLocation);
                        GameObject newObject = InstantiateDoorAtLocation(doorPrefabToUse, spawnLocation);
                        newObject.transform.parent = transform;
                        spawnLocation.spawnedObject = newObject;
                    }
                }
            }
        }

        // Helper method to get the floor prefab to use for a map tile
        private GameObject GetFloorPrefab(MapTile mapTile)
        {
            GameObject floorPrefabToUse = floorPrefab;
            Room room = mapTile.room;
            if (room != null)
            {
                foreach (CustomRoom customroom in customRooms)
                {
                    if (customroom.roomId == room.room_id)
                    {
                        floorPrefabToUse = customroom.floorPrefab;
                        break;
                    }
                }
            }
            return floorPrefabToUse;
        }

        // Helper method to get the wall prefab to use for a map tile
        private GameObject GetWallPrefab(MapTile mapTile)
        {
            GameObject wallPrefabToUse = wallPrefab;
            Room room = mapTile.room;
            if (room != null)
            {
                foreach (CustomRoom customroom in customRooms)
                {
                    if (customroom.roomId == room.room_id)
                    {
                        wallPrefabToUse = customroom.wallPrefab;
                        break;
                    }
                }
            }
            return wallPrefabToUse;
        }

        // Helper method to get the corridor wall prefab to use for a map tile
        private GameObject GetCorridorWallPrefab(MapTile mapTile)
        {
            GameObject wallPrefabToUse = corridorWallPrefab;
            Room room = mapTile.room;
            if (room != null)
            {
                foreach (CustomRoom customroom in customRooms)
                {
                    if (customroom.roomId == room.room_id)
                    {
                        wallPrefabToUse = customroom.wallPrefab;
                        break;
                    }
                }
            }
            return wallPrefabToUse;
        }

        // Helper method to get the corner prefab to use for a map tile
        private GameObject GetCornerPrefab(MapTile mapTile)
        {
            GameObject cornerPrefabToUse = cornerPrefab;
            Room room = mapTile.room;
            if (room != null)
            {
                foreach (CustomRoom customroom in customRooms)
                {
                    if (customroom.roomId == room.room_id)
                    {
                        cornerPrefabToUse = customroom.cornerPrefab;
                        break;
                    }
                }
            }
            return cornerPrefabToUse;
        }

        // Helper method to rotate a wall tile
        private void RotateWall(GameObject created_tile, int tile)
        {
            if (!makeIt3d)
            {
                created_tile.transform.Rotate(Vector3.forward * (90 * (tile - 4)));
            }
            else
            {
                created_tile.transform.Rotate(Vector3.up * (90 * (tile - 2))); // 3D corner rotation
            }
        }

        // Helper method to rotate a corner tile
        private void RotateCorner(GameObject created_tile, int tile)
        {
            if (cornerRotation)
            {
                if (!makeIt3d)
                {
                    created_tile.transform.Rotate(Vector3.forward * (-90 * (tile - 4)));
                }
                else
                {
                    created_tile.transform.Rotate(Vector3.up * (90 * (tile - 4)));
                }
            }
        }

        // Checks if an object can be spawned at a given location based on spawn options
        private bool CanSpawnObjectAtLocation(SpawnOption objectToSpawn, SpawnList spawnLocation)
        {
            bool createHere = false;

            if (!spawnLocation.spawnedObject && !spawnLocation.byCorridor)
            {
                if (objectToSpawn.spawnRoom > maximumRoomCount)
                {
                    objectToSpawn.spawnRoom = 0;
                }
                if (objectToSpawn.spawnRoom == 0)
                {
                    if (objectToSpawn.spawnByWall)
                    {
                        if (spawnLocation.byWall)
                        {
                            createHere = true;
                        }
                    }
                    else if (objectToSpawn.spawmInTheMiddle)
                    {
                        if (spawnLocation.inTheMiddle)
                        {
                            createHere = true;
                        }
                    }
                    else
                    {
                        createHere = true;
                    }
                }
                else
                {
                    if (spawnLocation.room.room_id == objectToSpawn.spawnRoom)
                    {
                        if (objectToSpawn.spawnByWall)
                        {
                            if (spawnLocation.byWall)
                            {
                                createHere = true;
                            }
                        }
                        else
                        {
                            createHere = true;
                        }
                    }
                }
            }

            return createHere;
        }

        // Instantiates an object at a given spawn location
        private GameObject InstantiateObjectAtLocation(SpawnOption objectToSpawn, SpawnList spawnLocation)
        {
            GameObject newObject;
            Quaternion spawnRotation = Quaternion.identity;

            if (!makeIt3d)
            {
                newObject = Instantiate(objectToSpawn.gameObject, new Vector3(spawnLocation.x * tileScaling, spawnLocation.y * tileScaling, 0), spawnRotation) as GameObject;
            }
            else
            {
                if (spawnLocation.byWall)
                {
                    switch (spawnLocation.wallLocation)
                    {
                        case "s":
                            spawnRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            break;
                        case "w":
                            spawnRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                            break;
                        case "n":
                            spawnRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                            break;
                        case "e":
                            spawnRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                            break;
                    }
                }
                else
                {
                    if (objectToSpawn.spawnRotated)
                    {
                        spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                    }
                    else
                    {
                        spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 2) * 90, 0));
                    }
                }

                newObject = Instantiate(objectToSpawn.gameObject, new Vector3(spawnLocation.x * tileScaling, 0 + objectToSpawn.heightFix, spawnLocation.y * tileScaling), spawnRotation) as GameObject;
            }

            return newObject;
        }

        // Helper method to get the door prefab to use for a spawn location
        private GameObject GetDoorPrefab(SpawnList spawnLocation)
        {
            GameObject doorPrefabToUse = doorPrefab;
            Room room = spawnLocation.room;
            if (room != null)
            {
                foreach (CustomRoom customroom in customRooms)
                {
                    if (customroom.roomId == room.room_id)
                    {
                        doorPrefabToUse = customroom.doorPrefab;
                        break;
                    }
                }
            }
            return doorPrefabToUse;
        }

        // Instantiates a door at a given spawn location
        private GameObject InstantiateDoorAtLocation(GameObject doorPrefabToUse, SpawnList spawnLocation)
        {
            GameObject newObject;

            if (!makeIt3d)
            {
                newObject = Instantiate(doorPrefabToUse, new Vector3(spawnLocation.x * tileScaling, spawnLocation.y * tileScaling, 0), Quaternion.identity) as GameObject;
            }
            else
            {
                newObject = Instantiate(doorPrefabToUse, new Vector3(spawnLocation.x * tileScaling, 0, spawnLocation.y * tileScaling), Quaternion.identity) as GameObject;
            }

            RotateDoor(newObject, spawnLocation.asDoor);

            return newObject;
        }

        // Rotates a door based on its direction
        private void RotateDoor(GameObject newObject, int doorDirection)
        {
            if (!makeIt3d)
            {
                newObject.transform.Rotate(Vector3.forward * (-90 * (doorDirection - 1)));
            }
            else
            {
                // 3D Door Rotation
                newObject.transform.Rotate(Vector3.up * (-90 * (doorDirection - 1)));
            }
        }



        // Use this for initialization
        void Start () {
			if (generate_on_load){
				ClearOldDungeon();
				Generate();

			}
		}





	}
}