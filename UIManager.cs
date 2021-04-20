using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Swordfish;
using Swordfish.items;

public class UIManager : MonoBehaviour
{
	public Image[] slots;

	public Image helperSlot;
	public Image helperSlotThumbnail;
	public Text helperSlotText;

	public MeshRenderer mainHand;
	public MeshRenderer offHand;

	public Sprite slotBase;
	public Sprite slotSelected;

	public Vector3 mainHandAnchor;
	public Vector3 offHandAnchor;

	public Image[] thumbnails;
    public Text[] counts;

	public RectTransform inventoryPanel;
	public RectTransform hotbarPanel;

	private bool inventoryUpdated = false;

	//  Keep this object alive
    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("UIManager").GetComponent<UIManager>();
            }

            return _instance;
        }
    }

	public void Start()
	{
		UIManager.instance.Initialize();
	}

	public void LateUpdate()
	{
		inventoryUpdated = false;	//	This flag limits inventory updates to 1/frame
	}

	public void Initialize()
	{
		GameMaster.Instance.player.getEquipment().EquipmentChange += OnEquipmentChange;
		GameMaster.Instance.player.getInventory().InventoryChange += OnInventoryChange;

		mainHandAnchor = mainHand.transform.localPosition;
		offHandAnchor = offHand.transform.localPosition;

		inventoryPanel = this.transform.FindChildByRecursion("panel_playerInventory").GetComponent<RectTransform>();
		hotbarPanel = this.transform.FindChildByRecursion("panel_hotbar").GetComponent<RectTransform>();

		helperSlot = hotbarPanel.Find("slot_helper").GetComponent<Image>();
		helperSlotThumbnail = helperSlot.transform.GetChild(0).GetComponent<Image>();
		helperSlotText = helperSlot.transform.GetChild(1).GetComponent<Text>();

		List<Image> slotList = new List<Image>();

		Transform rootToSearch = hotbarPanel.Find("SLOTS");
		foreach (Transform child in rootToSearch)
		{
			slotList.Add( child.GetComponent<Image>() );
		}

		rootToSearch = inventoryPanel.Find("SLOTS");
		foreach (Transform child in rootToSearch)
		{
			slotList.Add( child.GetComponent<Image>() );
		}

		slots = slotList.ToArray();
		thumbnails = new Image[slots.Length];
		counts = new Text[slots.Length];

		for (int i = 0; i < slots.Length; i++)
		{
			thumbnails[i] = slots[i].transform.GetChild(0).GetComponent<Image>();
			counts[i] = slots[i].transform.GetChild(1).GetComponent<Text>();
		}

		UpdateHotbar();
		UpdateViewModels();
	}

	public static void CloseInventory()
	{
		Screen.lockCursor = true;
		UIManager.instance.getInventoryPanel().gameObject.SetActive(false);
	}

	public static void ShowInventory(Inventory _inventory)
	{
		Screen.lockCursor = false;
		UIManager.instance.getInventoryPanel().gameObject.SetActive(true);
	}

	public static bool hasInventoryOpen()
	{
		return UIManager.instance.getInventoryPanel().gameObject.activeSelf;
	}

	public RectTransform getInventoryPanel()
	{
		return inventoryPanel;
	}

	public static bool isMouseOverUI()
	{
		return EventSystem.current.IsPointerOverGameObject();
	}

	public static bool isMouseOverUIWithIgnore()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = Input.mousePosition;

		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, raycastResults);

		for (int i = 0; i < raycastResults.Count; i++)
		{
			if (raycastResults[i].gameObject.tag == "IgnoreRaycast")
			{
				raycastResults.RemoveAt(i);
				i--;
			}
		}

		return (raycastResults.Count > 0);
	}

	private void OnEquipmentChange(object sender, EquipmentChangeEvent e)
	{
		UpdateViewModels();

		if (inventoryUpdated == false)
		{
			UpdateInventory();
			UpdateHotbar();
			inventoryUpdated = true;
		}
	}

	private void OnInventoryChange(object sender, InventoryChangeEvent e)
	{
		UpdateViewModels();

		if (inventoryUpdated == false)
		{
			UpdateInventory();
			UpdateHotbar();
			inventoryUpdated = true;
		}
	}

	public static void ForceUpdate()
	{
		UIManager.instance.UpdateInventory();
		UIManager.instance.UpdateHotbar();
		UIManager.instance.UpdateViewModels();
	}

	private void UpdateInventory()
	{

	}

	public void UpdateHotbar()
	{
		bool usingHelper = false;

		for (int i = 0; i < thumbnails.Length; i++)
		{
			Inventory inventory = GameMaster.Instance.player.getInventory();

			if (GameMaster.Instance.player.getEquipment().getMainHandSlot() == i)
			{
				slots[i].sprite = slotSelected;

				if (GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.MAINHAND) is DRILL)
				{
					helperSlot.transform.SetParent(slots[i].transform, false);

					Voxel currentVoxel = GameMaster.Instance.player.GetComponent<PlayerInteractControl>().currentVoxel;
					// Texture2D texture = GameMaster.Instance.getCachedImage(currentVoxel.ToString()).texture;
        			helperSlotThumbnail.sprite = GameMaster.Instance.getCachedImage(currentVoxel.ToString()).sprite;//Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

					helperSlot.gameObject.SetActive(true);
					helperSlotThumbnail.gameObject.SetActive(true);
					helperSlotText.gameObject.SetActive(false);
					usingHelper = true;
				}
			}
			else
			{
				slots[i].sprite = slotBase;
			}

			if (inventory.at(i) != null && inventory.at(i).isValid() == true)
			{
				CachedImage cachedImage = inventory.at(i).getImageData();

				Texture2D texture = null;
				thumbnails[i].gameObject.SetActive(false);
				if (cachedImage != null) { texture = cachedImage.texture; thumbnails[i].gameObject.SetActive(true); }
				thumbnails[i].transform.localPosition = inventory.at(i).getDisplayOffset().toVector3();
				thumbnails[i].transform.localRotation = Quaternion.Euler(inventory.at(i).getDisplayRotation().toVector3());
				thumbnails[i].transform.localScale = inventory.at(i).getDisplayScale().toVector3();

				// thumbnails[i].material.SetTexture("_MainTex", texture);
				thumbnails[i].sprite = cachedImage.sprite;//Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

				counts[i].gameObject.SetActive(false);
				if (inventory.at(i).getAmount() > 1)
				{
					counts[i].gameObject.SetActive(true);
					counts[i].text = inventory.at(i).getAmount().ToString();
				}
			}
			else
			{
				thumbnails[i].gameObject.SetActive(false);
				counts[i].gameObject.SetActive(false);
			}
		}

		if (usingHelper == false)
		{
			helperSlot.gameObject.SetActive(false);
		}
	}

	private void UpdateViewModels()
	{
		if (mainHand != null)
		{
			CachedImage cachedImage = GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.MAINHAND).getImageData();

			Texture2D texture = null;
			mainHand.gameObject.SetActive(false);
			if (cachedImage != null) { texture = cachedImage.texture; mainHand.gameObject.SetActive(true); }

			// mainHand.material.mainTexture = texture;
			mainHand.material.SetTexture("_BaseColorMap", texture);
			mainHand.transform.localPosition = mainHandAnchor + GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.MAINHAND).getViewOffset().toVector3();
			mainHand.transform.localRotation = Quaternion.Euler(GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.MAINHAND).getViewRotation().toVector3());
			mainHand.transform.localScale = GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.MAINHAND).getViewScale().toVector3();

			mainHand.SendMessage("GenerateMesh", null, SendMessageOptions.DontRequireReceiver);
		}

		if (offHand != null)
		{
			CachedImage cachedImage = GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.OFFHAND).getImageData();

			Texture2D texture = null;
			offHand.gameObject.SetActive(false);
			if (cachedImage != null) { texture = cachedImage.texture; offHand.gameObject.SetActive(false); }

			// offHand.material.mainTexture = texture;
			offHand.material.SetTexture("_BaseColorMap", texture);
			offHand.transform.localPosition = offHandAnchor + GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.OFFHAND).getViewOffset().toVector3();
			offHand.transform.localRotation = Quaternion.Euler(GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.OFFHAND).getViewRotation().toVector3());
			offHand.transform.localScale = GameMaster.Instance.player.getEquipment().getSlot(EquipmentSlot.OFFHAND).getViewScale().toVector3();

			offHand.transform.localPosition = new Vector3(-offHand.transform.localPosition.x, offHand.transform.localPosition.y, offHand.transform.localPosition.z);

			offHand.SendMessage("GenerateMesh", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(0); }

		else if (Input.GetKeyDown(KeyCode.Alpha2) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(1); }

		else if (Input.GetKeyDown(KeyCode.Alpha3) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(2); }

		else if (Input.GetKeyDown(KeyCode.Alpha4) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(3); }

		else if (Input.GetKeyDown(KeyCode.Alpha5) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(4); }

		else if (Input.GetKeyDown(KeyCode.Alpha6) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(5); }

		else if (Input.GetKeyDown(KeyCode.Alpha7) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(6); }

		else if (Input.GetKeyDown(KeyCode.Alpha8) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(7); }

		else if (Input.GetKeyDown(KeyCode.Alpha9) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(8); }

		else if (Input.GetKeyDown(KeyCode.Alpha0) == true)
			{ GameMaster.Instance.player.getEquipment().setMainHandSlot(9); }
	}
}