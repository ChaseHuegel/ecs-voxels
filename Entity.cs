using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swordfish;

//	return 1 if facing, -1 if facing opposite Vector3.Dot(transform.forward, (other.position - transform.position).normalized);

public class Entity : MonoBehaviour
{
	//	Objects
	[Header("Objects")]

	[SerializeField]
	protected CharacterController controller;
	[SerializeField]
	protected GameObject renderer;

	//	Data
	[Header("Data")]

	[SerializeField]
	protected Inventory inventory = new Inventory();

	[SerializeField]
	protected Equipment equipment = new Equipment();

	//	Variables
	[Header("Variables")]

	[SerializeField]
	protected 	EntityType 	type 		= EntityType.GENERIC;
	[SerializeField]
	protected 	string 		name 		= "";
	[SerializeField]
	protected 	Guid 		uid 		= Guid.NewGuid();
	[SerializeField]
	protected 	Entity 		passenger	= null;
	[SerializeField]
	protected 	Entity 		vehicle 	= null;
	[SerializeField]
	protected	bool		spawned		= false;
	[SerializeField]
	protected	bool		invuln		= false;
	[SerializeField]
	protected 	int 		ticksLived 	= 0;
	[SerializeField]
	protected 	int 		health 		= 1;
	[SerializeField]
	protected 	int 		maxHealth 	= 1;

	//	Getters
	public Inventory 	getInventory() 	{ return inventory; }
	public Equipment 	getEquipment() 	{ return equipment; }
	public EntityType 	getType() 		{ return type; }
	public string 		getName() 		{ return name; }
	public Guid 		getUID() 		{ return uid; }
	public int 			getTicksLived()	{ return ticksLived; }
	public int 			getHealth() 	{ return health; }
	public int 			getMaxHealth() 	{ return maxHealth; }

	//	Setters
	public void 	setInventory(Inventory c)	{ inventory = c; }
	public void 	setEquipment(Equipment c)	{ equipment = c; }
	public void 	setName(string c)			{ name = c; }
	public void 	setHealth(int c) 			{ health = c; }
	public void 	setMaxHealth(int c) 		{ maxHealth = c; }

	//	Checks
	public bool 	isValid() 			{ return (health > 0 && spawned == true); }
	public bool 	isInvulnerable() 	{ return invuln; }

	//	Overridable methods
	protected virtual void OnInitialize() {}
	protected virtual void OnUpdate() {}
	protected virtual void OnLateUpdate() {}
	protected virtual void OnTick() {}
	protected virtual void OnSpawn() {}
	protected virtual void OnDespawn() {}
	protected virtual void OnDeath() {}

	//	Builtin methods
	private void Start()
	{
		try
		{
			renderer = this.transform.FindChildByRecursion("renderer").gameObject;
			controller = this.transform.gameObject.GetComponent<CharacterController>();
			renderer.SetActive(false);
		} catch {};

		equipment.setInventory(inventory);

		OnInitialize();
	}

	private void Update()
	{
		if (health <= 0)
		{
			Despawn();
			OnDeath();
		}
		else if (health > maxHealth)
		{
			health = maxHealth;
		}

		OnUpdate();
	}

	private void LateUpdate()
	{
		OnLateUpdate();
	}

	//	Public methods
	public void Tick()
	{
		ticksLived++;
		OnTick();
	}

	public void Spawn()
    {
		ticksLived = 0;
		health = maxHealth;

		spawned = true;
		if (renderer != null) { renderer.SetActive(true); }

		OnSpawn();
    }

	public void Despawn()
    {
		spawned = false;
		if (renderer != null) { renderer.SetActive(false); }

		EjectPassenger();
		LeaveVehicle();

		OnDespawn();
    }

	public void Remove()
	{
		Destroy(this.gameObject);
	}

	public void Kill()
	{
		health = 0;
	}

	public void Damage(int _amount)
	{
		if (invuln == false) { health -= Mathf.Abs(_amount); }
	}

	public void Heal(int _amount)
	{
		health += Mathf.Abs(_amount);
	}

	public void EjectPassenger()
	{
		passenger = null;	//	TODO: eject passenger
	}

	public void LeaveVehicle()
	{
		vehicle = null;	//	TODO: leave vehicle
	}

	public void MountPassenger(Entity _entity)
	{
		passenger = _entity;	//	TODO: mount passenger
	}

	public void MountVehicle(Entity _entity)
	{
		vehicle = _entity;	//	TODO: mount vehicle
	}
}