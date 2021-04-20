using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish.items;

namespace Swordfish
{
    public class DroppedItem : Interactable
    {
		public Billboard billboard = null;
		public MeshRenderer renderer = null;
		public Mesh mesh = null;

		public TextMesh displayName = null;
		public ItemType presetItem = ItemType.VOID;
		public Voxel presetVoxel = Voxel.VOID;
		public int presetCount = 1;

		public bool isVoxel = false;
		public Material itemMaterial;

		[SerializeField]
		protected Item item = null;

		public void setItem(Item _item)
		{
			item = _item;

			UpdateDisplay();
		}

		public Item getItem()
		{
			return item;
		}

		public override void PreInitialize()
		{
			itemMaterial = renderer.material;
        }

        public override void Initialize()
        {
			if (presetItem != ItemType.VOID)
			{
				if (presetItem == ItemType.BLOCKITEM)
				{
					item = presetVoxel.toItem();
					item.setAmount(presetCount);
				}
				else
				{
					item = presetItem.toItem();
					item.setAmount(presetCount);
				}

				UpdateDisplay();
			}
		}

		public void LateUpdate()
		{
			if (mesh != null)
			{
				Vector3 position = new Vector3(transform.position.x, transform.position.y + (Mathf.Sin(Time.time * 3) * 0.1f), transform.position.z);
				Quaternion rotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 0.5f) * 180, 0);
				Vector3 scale = Vector3.one * 0.5f;

				if (isVoxel == false) { scale = Vector3.one; }

				Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

				if (item.getAmount() > 1)
				{
					int drawCount = 2 + (int)( item.getAmount() * 0.05f );

					for (int i = 0; i < drawCount; i++)
					{
						UnityEngine.Random.seed = i;

						Vector3 position2 = new Vector3( position.x + ((UnityEngine.Random.value - 0.5f) * 0.5f), position.y + ((UnityEngine.Random.value - 0.5f) * 0.5f), position.z + ((UnityEngine.Random.value - 0.5f) * 0.5f) );

						matrix = Matrix4x4.TRS(position2, rotation, scale * UnityEngine.Random.value );

						Material mat = GameMaster.Instance.voxelMaterials[0];
						if (isVoxel == false) { mat = itemMaterial; }

						Graphics.DrawMesh(mesh, matrix, mat, 0);
					}

					//Graphics.DrawMeshInstanced(mesh, 0, GameMaster.Instance.voxelMaterials[0], matrices);
				}
				else
				{
					Material mat = GameMaster.Instance.voxelMaterials[0];
					if (isVoxel == false) { mat = itemMaterial; }

					Graphics.DrawMesh(mesh, matrix, mat, 0);
				}
			}

			if (Util.DistanceUnsquared(transform.position, GameMaster.Instance.player.transform.position) <= 4.0f)
			{
				Interact(GameMaster.Instance.player.gameObject);
			}

			if (item == null || item.isValid() == false)
			{
				Destroy(this.gameObject);
			}
		}

		public void UpdateDisplay()
		{
			if (item.getType() == ItemType.BLOCKITEM)
			{
				renderer.enabled = false;

				mesh = ModelBuilder.GetVoxelMesh( ((BLOCKITEM)item).getVoxel() );
				isVoxel = true;
			}
			else
			{
				renderer.enabled = false;

				CachedImage cachedImage = item.getImageData();

				Texture2D texture = null;
				if (cachedImage != null) { texture = cachedImage.texture; }

				itemMaterial.SetTexture("_BaseColorMap", texture);
				mesh = GenerateMesh(texture);
				isVoxel = false;
			}

			displayName.text = item.getName();
		}

        public override void Hover(GameObject _interactor = null)
        {
			UpdateDisplay();
        }

        public override void Interact(GameObject _interactor = null)
        {
			Entity entity = _interactor.GetComponent<Entity>();

			if (entity != null && item != null)
			{
				Inventory inventory = entity.getInventory();
				item = inventory.Add(item);

				if (item == null || item.isValid() == false)
				{
					GameMaster.Instance.PlaySound(GameMaster.Instance.pickupSound, transform.position, 2.0f);
					Destroy(this.gameObject);
				}
			}
        }

	private enum Edge {top, left, bottom, right};
    public int alphaTheshold = 0;
    public float depth = 0.03125f;
    private Color32[] m_Colors;
    private int m_Width;
    private int m_Height;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector3> m_Normals = new List<Vector3>();
    private List<Vector2> m_TexCoords = new List<Vector2>();

    private bool HasPixel(int aX, int aY)
    {
        return m_Colors[aX + aY*m_Width].a > alphaTheshold;
    }
    void AddQuad(Vector3 aFirstEdgeP1, Vector3 aFirstEdgeP2,Vector3 aSecondRelative, Vector3 aNormal, Vector2 aUV1, Vector2 aUV2, bool aFlipUVs)
    {
        m_Vertices.Add(aFirstEdgeP1);
        m_Vertices.Add(aFirstEdgeP2);
        m_Vertices.Add(aFirstEdgeP2 + aSecondRelative);
        m_Vertices.Add(aFirstEdgeP1 + aSecondRelative);
        m_Normals.Add(aNormal);
        m_Normals.Add(aNormal);
        m_Normals.Add(aNormal);
        m_Normals.Add(aNormal);
        if (aFlipUVs)
        {
            m_TexCoords.Add(new Vector2(aUV1.x,aUV1.y));
            m_TexCoords.Add(new Vector2(aUV2.x,aUV1.y));
            m_TexCoords.Add(new Vector2(aUV2.x,aUV2.y));
            m_TexCoords.Add(new Vector2(aUV1.x,aUV2.y));
        }
        else
        {
            m_TexCoords.Add(new Vector2(aUV1.x,aUV1.y));
            m_TexCoords.Add(new Vector2(aUV1.x,aUV2.y));
            m_TexCoords.Add(new Vector2(aUV2.x,aUV2.y));
            m_TexCoords.Add(new Vector2(aUV2.x,aUV1.y));
        }

    }

    void AddEdge(int aX, int aY, Edge aEdge)
    {
        Vector2 size = new Vector2(1.0f/m_Width, 1.0f/m_Height);
        Vector2 uv = new Vector3(aX*size.x, aY*size.y);
        Vector2 P = uv - Vector2.one*0.5f;
        uv += size*0.5f;
        Vector2 P2 = P;
        Vector3 normal;
        if (aEdge == Edge.top)
        {
            P += size;
            P2.y += size.y;
            normal =  Vector3.up;
        }
        else if(aEdge == Edge.left)
        {
            P.y += size.y;
            normal =  Vector3.left;
        }
        else if(aEdge == Edge.bottom)
        {
            P2.x += size.x;
            normal =  Vector3.down;
        }
        else
        {
            P2 += size;
            P.x += size.x;
            normal =  Vector3.right;
        }
        AddQuad(P, P2, Vector3.forward*depth, normal, uv,uv,false);
    }

    private Mesh GenerateMesh(Texture2D _texture)
    {
        Texture2D tex = _texture;
        m_Colors = tex.GetPixels32();
        m_Width = tex.width;
        m_Height = tex.height;
        //      first point                     , second point                    , relative 3. P, normal,          lower UV,     Upper UV,    flipUV
        AddQuad(new Vector3(-0.5f, -0.5f, 0    ), new Vector3(-0.5f,  0.5f, 0    ), Vector3.right, Vector3.back,    Vector2.zero, Vector2.one, false);
        AddQuad(new Vector3(-0.5f, -0.5f, depth), new Vector3( 0.5f, -0.5f, depth), Vector3.up,    Vector3.forward, Vector2.zero, Vector2.one, true);

        for (int y = 0; y < m_Height; y++) // bottom to top
        {
            for (int x = 0; x < m_Width; x++) // left to right
            {
                if (HasPixel(x,y))
                {
                    if(x==0 || !HasPixel(x-1,y))
                        AddEdge(x,y,Edge.left);

                    if(x==m_Width-1 || !HasPixel(x+1,y))
                        AddEdge(x,y,Edge.right);

                    if(y==0 || !HasPixel(x,y-1))
                        AddEdge(x,y,Edge.bottom);

                    if(y==m_Height-1 || !HasPixel(x,y+1))
                        AddEdge(x,y,Edge.top);
                }
            }
        }
        var thisMesh = new Mesh();
        thisMesh.vertices = m_Vertices.ToArray();
        thisMesh.normals = m_Normals.ToArray();
        thisMesh.uv = m_TexCoords.ToArray();
        int[] quads = new int[m_Vertices.Count];
        for (int i = 0; i < quads.Length; i++)
            quads[i] = i;
        thisMesh.SetIndices(quads,MeshTopology.Quads,0);
        // GetComponent<MeshFilter>().sharedMesh = thisMesh;
		return thisMesh;
    }
    }
}