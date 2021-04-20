using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Swordfish;
using Swordfish.items;

public class PlayerInteractControl : MonoBehaviour
{
	private Mesh displayModel;
    private VoxelHitData hitData;

    public Material displayMaterial;

    public AudioClip placeSound;

    public int currentPaletteIndex = 2;
    public Voxel currentVoxel = Voxel.METAL_PANEL;

    public int rotationIndex = 0;
    public Direction rotationDirection = Direction.NORTH;
    public Direction rotationOrientation = Direction.NORTH;

	public Image paletteImage;

	private void Start()
	{
		displayModel = new Mesh();
	}

	public void Update()
	{
		Interact();
	}

	public void Interact()
	{
		if (Input.GetKeyDown(KeyCode.I) == true)
		{
			if (UIManager.hasInventoryOpen() == false)
			{
				UIManager.ShowInventory( GameMaster.Instance.player.getInventory() );
			}
			else
			{
				UIManager.CloseInventory();
			}
		}

		if (Input.GetKeyDown(KeyCode.F) == true)
		{
			currentPaletteIndex = Util.WrapInt(currentPaletteIndex += 1, 2, Enum.GetValues(typeof(Voxel)).Length - 1);
			currentVoxel = (Voxel)currentPaletteIndex;
			UIManager.ForceUpdate();
		}

		//	Everything below here can't be done when mousing over non-clickthrough UI
		if (UIManager.isMouseOverUIWithIgnore() == true)
		{
			return;
		}

		Unity.Physics.RaycastHit hit;
		Unity.Entities.Entity hitEntity;
		hitData = null;

		if (GameMaster.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 8, out hit, out hitEntity))
		{
			hitData = new VoxelHitData(hit, hitEntity);

			Vector3 offsetPos = hitData.hitTransform.InverseTransformPoint(hit.Position);

			offsetPos.x = (float)Math.Round(offsetPos.x);
			offsetPos.y = (float)Math.Round(offsetPos.y);
			offsetPos.z = (float)Math.Round(offsetPos.z);

			// offsetPos.x = offsetPos.x - offsetPos.x % 1;
			// offsetPos.y = offsetPos.y - offsetPos.y % 1;
			// offsetPos.z = offsetPos.z - offsetPos.z % 1;

			offsetPos = hitData.hitTransform.rotation * offsetPos;
			Vector3 hitPos = hitData.hitTransform.position + offsetPos;

			Debug.DrawLine(Camera.main.ScreenPointToRay(Input.mousePosition).origin, hitPos, Color.red);
			Debug.DrawRay(hit.Position, hit.SurfaceNormal, Color.yellow);
			Debug.DrawRay(hit.Position, hit.SurfaceNormal * -0.5f, Color.cyan);
		}
		else
		{
			Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 8, Color.green);
		}

		// if (hit.collider != null)
		// {
		// 	Interactable interactable = hit.collider.GetComponent<Interactable>();
		// 	if (interactable != null)
		// 	{
		// 		if (Input.GetMouseButtonDown(1) == true)
		// 		{
		// 			interactable.TryInteract(GameMaster.Instance.player.gameObject);
		// 			return;
		// 		}
		// 		else
		// 		{
		// 			interactable.TryHover(GameMaster.Instance.player.gameObject);
		// 		}
		// 	}
		// }

		Item mainHand = GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.MAINHAND);
		if (mainHand.isValid() == false) { return; }

		if (Input.GetKeyDown(KeyCode.G) == true)
		{
			int amountToDrop = 1;

			if (Input.GetKey(KeyCode.LeftControl) == true || Input.GetKey(KeyCode.RightControl) == true)
			{
				amountToDrop = mainHand.getAmount();
			}

			GameMaster.Instance.DropItemNaturally( Position.fromVector3(transform.position + ( transform.forward * 3 )), mainHand, amountToDrop );
			GameMaster.Instance.player.getInventory().Remove(mainHand, amountToDrop);
		}

		if (hitData == null || hitData.isValid() == false) { return; }

		if (mainHand is BLOCKITEM)
		{
			rotationOrientation = hitData.getFace().getOpposite();
			currentVoxel = ((BLOCKITEM)mainHand).getVoxel();

			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				rotationIndex = Util.WrapInt(rotationIndex += 1, 0, 3);
				rotationDirection = (Direction)rotationIndex;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				rotationIndex = Util.WrapInt(rotationIndex -= 1, 0, 3);
				rotationDirection = (Direction)rotationIndex;
			}

			if (Input.GetMouseButtonDown(1) == true)
			{
				if (hitData.getAtFace().getType() == Voxel.VOID)
				{
					Block thisBlock = hitData.voxelObject.setBlockAt(hitData.atFace, currentVoxel);

					if (thisBlock.getChunk() != null)
					{
						// GameMaster.Instance.player.getInventory().Remove(mainHand, 1);

						GameMaster.Instance.PlaySound(placeSound, hit.Position);
						GetComponent<MotionAnimator>().PlayMotion("mainHandAttack");

						if (thisBlock.getBlockData() is Rotatable)
						{
							Rotatable data = (Rotatable)thisBlock.getBlockData();
							data.setDirection(rotationDirection);
							thisBlock.setBlockData(data);
						}

						if (thisBlock.getBlockData() is Orientated)
						{
							Orientated data = (Orientated)thisBlock.getBlockData();
							data.setOrientation(rotationOrientation);
							thisBlock.setBlockData(data);
						}

						if ( hitData.getFace() == Direction.BELOW )
						{
							if (thisBlock.getBlockData() is Flippable)
							{
								Flippable data = (Flippable)thisBlock.getBlockData();
								data.setFlipped(true);
								thisBlock.setBlockData(data);
							}
						}
					}
				}
			}

			UpdateDisplayModel();
			DrawDisplayModel(hitData.localFacePosition);
		}
		else if (mainHand is CUTTER)
		{
			Block clickedBlock = hitData.getAt();

			if (Input.GetMouseButtonDown(0) == true)
			{
				if (clickedBlock.getType() != Voxel.VOID && clickedBlock.getType() != Voxel.SHIP_CORE)
				{
					Position droppedPosition = Position.fromVector3(hit.Position);
					GameMaster.Instance.DropItemNaturally(droppedPosition, clickedBlock.getType(), 1);

					Block thisBlock = hitData.voxelObject.setBlockAt(hitData.atHit, Voxel.VOID);

					if (thisBlock.getChunk() != null)
					{
						GameMaster.Instance.PlaySound(placeSound, hit.Position);
						GetComponent<MotionAnimator>().PlayMotion("mainHandAttack");
					}
				}
			}
			else if (Input.GetMouseButtonDown(1) == true)
			{
				if (clickedBlock.getType() != Voxel.VOID && clickedBlock.getType() != Voxel.SHIP_CORE)
				{
					Block thisBlock = hitData.voxelObject.setBlockAt(hitData.atHit, Voxel.FRAME);

					if (thisBlock.getChunk() != null)
					{
						GameMaster.Instance.PlaySound(placeSound, hit.Position);
						GetComponent<MotionAnimator>().PlayMotion("mainHandAttack");
					}
				}
			}

			currentVoxel = clickedBlock.getType();

			if (clickedBlock.getBlockData() is Rotatable)
			{
				Rotatable data = (Rotatable)clickedBlock.getBlockData();
				rotationDirection = data.getDirection();
			}

			if (clickedBlock.getBlockData() is Orientated)
			{
				Orientated data = (Orientated)clickedBlock.getBlockData();
				rotationOrientation = data.getOrientation();
			}

			UpdateDisplayModel();
			DrawDisplayModel(hitData.localPosition);
		}
		else if (mainHand is DRILL)
		{
			if (hitData.isValid())
			{
				rotationOrientation = hitData.getFace().getOpposite();

				if (Input.GetAxis("Mouse ScrollWheel") > 0)
				{
					rotationIndex = Util.WrapInt(rotationIndex += 1, 0, 3);
					rotationDirection = (Direction)rotationIndex;
				}
				else if (Input.GetAxis("Mouse ScrollWheel") < 0)
				{
					rotationIndex = Util.WrapInt(rotationIndex -= 1, 0, 3);
					rotationDirection = (Direction)rotationIndex;
				}

				if (Input.GetMouseButtonDown(2) == true)
                {
                    Block clickedBlock = hitData.getAt();
                    if (clickedBlock.getType() != Voxel.VOID && clickedBlock.getType() != Voxel.SHIP_CORE)
                    {
                        currentVoxel = clickedBlock.getType();
                        currentPaletteIndex = (int)currentVoxel;
                    }
                }

				if (Input.GetMouseButtonDown(1) == true)
                {
                    if (hitData.getAt().getType() == Voxel.FRAME)
                    {
                        Block thisBlock = hitData.voxelObject.setBlockAt(hitData.atHit, currentVoxel);

                        if (thisBlock.getChunk() != null)
                        {
							GameMaster.Instance.PlaySound(placeSound, hit.Position);
							GetComponent<MotionAnimator>().PlayMotion("mainHandAttack");

                            if (thisBlock.getBlockData() is Rotatable)
                            {
                                Rotatable data = (Rotatable)thisBlock.getBlockData();
                                data.setDirection(rotationDirection);
                                thisBlock.setBlockData(data);
                            }

                            if (thisBlock.getBlockData() is Orientated)
                            {
                                Orientated data = (Orientated)thisBlock.getBlockData();
                                data.setOrientation(rotationOrientation);
                                thisBlock.setBlockData(data);
                            }

                            if ( hitData.getFace() == Direction.BELOW )
                            {
                                if (thisBlock.getBlockData() is Flippable)
                                {
                                    Flippable data = (Flippable)thisBlock.getBlockData();
                                    data.setFlipped(true);
                                    thisBlock.setBlockData(data);
                                }
                            }
                        }
                    }
                }

				UpdateDisplayModel();
				DrawDisplayModel(hitData.localPosition);
			}
		}
	}

	public void DrawDisplayModel(Vector3 _displayPosition)
	{
		Vector3 position = hitData.hitTransform.rotation * (_displayPosition) + hitData.hitTransform.position;
		Quaternion rotation = hitData.hitTransform.rotation;
		Vector3 scale = Vector3.one * (1.01f + (Mathf.PingPong(Time.time * 3, 1) * 0.05f ));
		Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);
		Graphics.DrawMesh(displayModel, matrix, GameMaster.Instance.selectionMaterials[0], 0);
	}

	public void UpdateDisplayModel()
    {
        List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<Color> colors = new List<Color>();

        Block thisBlock = currentVoxel.toBlock();

        if (thisBlock.getBlockData() is Rotatable)
        {
            Rotatable data = (Rotatable)thisBlock.getBlockData();
            data.setDirection(rotationDirection);
            thisBlock.setBlockData(data);
        }

        if (thisBlock.getBlockData() is Orientated && hitData != null)
        {
            Orientated data = (Orientated)thisBlock.getBlockData();
            data.setOrientation(rotationOrientation);
            thisBlock.setBlockData(data);
        }

        if ( hitData != null && hitData.getFace() == Direction.BELOW )
        {
            if (thisBlock.getBlockData() is Flippable)
            {
                Flippable data = (Flippable)thisBlock.getBlockData();
                data.setFlipped(true);
                thisBlock.setBlockData(data);
            }
        }

        switch (thisBlock.getModelType())
        {
            case ModelType.CUBE:
                ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock);
                break;

            case ModelType.SLOPE:
                ModelBuilder.Slope.Face(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Slope.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Slope.North(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Slope.East(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Slope.West(vertices, triangles, normals, uvs, colors, thisBlock);
                break;

            case ModelType.CUSTOM:
                ModelBuilder.Custom.Build(vertices, triangles, normals, uvs, colors, thisBlock.getModelData(), thisBlock);
                break;

            case ModelType.CUSTOM_CUBE:
                ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock);
                ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock);
                break;

            case ModelType.CROSS_SECTION_SMALL:
                ModelBuilder.CrossSection.Small.Build(vertices, triangles, normals, uvs, colors, thisBlock);
                break;

            case ModelType.CUBE_HALF:
                Vector3 scale = new Vector3(1.0f, 1.0f, 0.5f);
                ModelBuilder.Cube.Top(vertices, triangles, normals, uvs, colors, thisBlock, scale);
                ModelBuilder.Cube.Bottom(vertices, triangles, normals, uvs, colors, thisBlock, scale);
                ModelBuilder.Cube.North(vertices, triangles, normals, uvs, colors, thisBlock, scale);
                ModelBuilder.Cube.South(vertices, triangles, normals, uvs, colors, thisBlock, scale);
                ModelBuilder.Cube.East(vertices, triangles, normals, uvs, colors, thisBlock, scale);
                ModelBuilder.Cube.West(vertices, triangles, normals, uvs, colors, thisBlock, scale);
                break;
        }

		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i] -= Vector3.one * 0.5f;
		}

        displayModel.Clear();

        displayModel.vertices = vertices.ToArray();
        displayModel.triangles = triangles.ToArray();
        displayModel.normals = normals.ToArray();
        displayModel.uv = uvs.ToArray();
        displayModel.colors = colors.ToArray();
    }
}