using System;
using System.IO;
using UnityEngine;

namespace Swordfish
{
	public class Util
	{
        public static int WrapInt(int _value, int _rangeMin, int _rangeMax)
        {
            int range = _rangeMax - _rangeMin + 1;

            if (_value < _rangeMin)
            {
                _value += range * ((_rangeMin - _value) / range + 1);
            }

            return _rangeMin + (_value - _rangeMin) % range;
        }

        public static float RangeToRange(float _input, float _low, float _high, float _newLow, float _newHigh)
        {
            return ((_input - _low) / (_high - _low)) * (_newHigh - _newLow) + _newLow;
        }

        public static float Distance (Vector3 firstPosition, Vector3 secondPosition)
        {
            Vector3 heading;

            heading.x = firstPosition.x - secondPosition.x;
            heading.y = firstPosition.y - secondPosition.y;
            heading.z = firstPosition.z - secondPosition.z;

            float distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
            return Mathf.Sqrt(distanceSquared);
        }

        public static float DistanceUnsquared(Vector3 firstPosition, Vector3 secondPosition)
        {
            return (firstPosition.x - secondPosition.x) * (firstPosition.x - secondPosition.x) +
                    (firstPosition.y - secondPosition.y) * (firstPosition.y - secondPosition.y) +
                    (firstPosition.z - secondPosition.z) * (firstPosition.z - secondPosition.z);
        }

        public static Vector3[] CreateEllipse(float a, float b, float h, float k, float theta, int resolution)
        {
            Vector3[] positions = new Vector3[resolution+1];
            Quaternion q = Quaternion.AngleAxis (theta, Vector3.up);
            Vector3 center = new Vector3(h,k,0.0f);

            for (int i = 0; i <= resolution; i++) {
                float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
                positions[i] = new Vector3(a * Mathf.Cos (angle), 0.0f, b * Mathf.Sin (angle));
                positions[i] = q * positions[i] + center;
            }

            return positions;
     }

        public static Direction DirectionFromVector3(Vector3 _vector)
		{
			if (_vector == new Vector3(0, 0, 1)) { return Direction.NORTH; }
			if (_vector == new Vector3(1, 0, 0)) { return Direction.EAST; }
			if (_vector == new Vector3(0, 0, -1)) { return Direction.SOUTH; }
			if (_vector == new Vector3(-1, 0, 0)) { return Direction.WEST; }
			if (_vector == new Vector3(0, 1, 0)) { return Direction.ABOVE; }
			if (_vector == new Vector3(0, -1, 0)) { return Direction.BELOW; }

            return Direction.NORTH;
		}

        public static void LoadVoxelObject(VoxelObject _voxelObject, string _fileName)
        {
            String path = @"external/saves/voxel objects/" + _fileName + ".svo";

            if (File.Exists(path) == true)
            {
                StreamReader file = new StreamReader(path);

                string name = file.ReadLine();

                string size = file.ReadLine();
                string[] parts = size.Split(',');
                int sizeX = int.Parse(parts[0]);
                int sizeY = int.Parse(parts[1]);
                int sizeZ = int.Parse(parts[2]);

                _voxelObject.setName(name);
                _voxelObject.chunkSizeX = sizeX;
                _voxelObject.chunkSizeY = sizeY;
                _voxelObject.chunkSizeZ = sizeZ;
                _voxelObject.Reset();

                while (file.EndOfStream == false)
                {
                    Voxel voxel = Voxel.VOID;
                    Block thisBlock = null;
                    int posX = 0;
                    int posY = 0;
                    int posZ = 0;

                    string entry = file.ReadLine();
                    string[] sections = entry.Split('/');
                    for (int i = 0; i < sections.Length; i++)
                    {
                        string[] section = sections[i].Split(':');
                        string tag = section[0];
                        string value = section[1];

                        if (tag == "v")
                        {
                            voxel = (Voxel)Enum.Parse(typeof(Voxel), value);
                            thisBlock = voxel.toBlock();
                        }
                        else if (tag == "p")
                        {
                            parts = value.Split(',');
                            posX = int.Parse(parts[0]);
                            posY = int.Parse(parts[1]);
                            posZ = int.Parse(parts[2]);
                        }
                        else if (tag == "r")
                        {
                            Direction facing = (Direction)Enum.Parse(typeof(Direction), value);
                            Rotatable data = (Rotatable)thisBlock.getBlockData();
                            data.setDirection(facing);
                            thisBlock.setBlockData(data);
                        }
                        else if (tag == "f")
                        {
                            bool flipped = bool.Parse(value);
                            Flippable data = (Flippable)thisBlock.getBlockData();
                            data.setFlipped(flipped);
                            thisBlock.setBlockData(data);
                        }
                        else if (tag == "o")
                        {
                            Direction orientation = (Direction)Enum.Parse(typeof(Direction), value);
                            Orientated data = (Orientated)thisBlock.getBlockData();
                            data.setOrientation(orientation);
                            thisBlock.setBlockData(data);
                        }
                    }

                    _voxelObject.setBlockAt(posX, posY, posZ, thisBlock);
                }

                file.Close();
            }
        }

        public static void SaveVoxelObject(VoxelObject _voxelObject, string _fileName)
        {
            String path = @"external/saves/voxel objects/" + _fileName + ".svo";

            if (File.Exists(path) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            StreamWriter file = new StreamWriter(path);

            file.WriteLine(_voxelObject.getName());
            file.WriteLine(_voxelObject.chunkSizeX + "," + _voxelObject.chunkSizeY + "," + _voxelObject.chunkSizeZ);

            for (int x = 0; x < _voxelObject.getLoadedChunks().Count; x++)
            {
                Chunk thisChunk = _voxelObject.getLoadedChunks()[x];
                Block[] blocks = thisChunk.getRawData();

                for (int i = 0; i < blocks.Length; i++)
                {
                    Block thisBlock = blocks[i];

                    if (thisBlock != null)
                    {
                        String line = "";
                        line += "v:" + thisBlock.getType();
                        line += "/p:" + thisBlock.getWorldX() + "," + thisBlock.getWorldY() + "," + thisBlock.getWorldZ();

                        if (thisBlock.getBlockData() is Rotatable)
                        {
                            line += "/r:" + ((Rotatable)thisBlock.getBlockData()).getDirection();
                        }

                        if (thisBlock.getBlockData() is Flippable && ((Flippable)thisBlock.getBlockData()).canFlip())
                        {
                            line += "/f:" + ((Flippable)thisBlock.getBlockData()).isFlipped();
                        }

                        if (thisBlock.getBlockData() is Orientated)
                        {
                            line += "/o:" + ((Orientated)thisBlock.getBlockData()).getOrientation();
                        }

                        file.WriteLine(line);
                    }
                }
            }

            file.Close();
        }
    }
}