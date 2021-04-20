using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class InputEntry
    {
        public string category = "";
        public string name = "";

        public int mouseButton = 0;
        public KeyCode code = KeyCode.A;
        public string axis = "";

        public bool useAxis = false;
        public bool useCode = false;
        public bool useMouseButton = false;

        public InputEntry(string _name, KeyCode _code, string _category = "")
        {
            name = _name;
            code = _code;
            category = _category;

            useAxis = false;
            useMouseButton = false;
            useCode = true;
        }

        public InputEntry(string _name, string _axis, string _category = "")
        {
            name = _name;
            axis = _axis;
            category = _category;

            useAxis = true;
            useMouseButton = false;
            useCode = false;
        }

        public InputEntry(string _name, int _mouseButton, string _category = "")
        {
            name = _name;
            mouseButton = _mouseButton;
            category = _category;

            useAxis = false;
            useMouseButton = true;
            useCode = false;
        }

        public bool Get()
        {
            if (useAxis == false)
            {
                if (useCode == true)
                {
                    return Input.GetKey(code);
                }

                if (useMouseButton == true)
                {
                    return Input.GetMouseButton(mouseButton);
                }
            }

            return false;
        }

        public bool GetDown()
        {
            if (useAxis == false)
            {
                if (useCode == true)
                {
                    return Input.GetKeyDown(code);
                }

                if (useMouseButton == true)
                {
                    return Input.GetMouseButtonDown(mouseButton);
                }
            }

            return false;
        }

        public bool GetUp()
        {
            if (useAxis == false)
            {
                if (useCode == true)
                {
                    return Input.GetKeyUp(code);
                }

                if (useMouseButton == true)
                {
                    return Input.GetMouseButtonUp(mouseButton);
                }
            }

            return false;
        }

        public float GetAxis()
        {
            if (useAxis == true)
            {
                return Input.GetAxis(axis);
            }

            return 0.0f;
        }
    }

    public class InputManager : ScriptableObject
    {
		//  Keep this object alive
		private static InputManager _instance;
		public static InputManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ScriptableObject.CreateInstance<InputManager>();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}

        public InputEntry[] entries;

        public InputManager()
        {
            entries = new InputEntry[0];
        }

		public void Initialize()
		{
			//  Axes
            Add(new InputEntry("MouseX", "Mouse X", "Axes"));
            Add(new InputEntry("MouseY", "Mouse Y", "Axes"));

			//	Interaction
			Add(new InputEntry("Primary", 0, "Interaction"));
            Add(new InputEntry("Secondary", 1, "Interaction"));
            Add(new InputEntry("Use", KeyCode.F, "Interaction"));

            //  Character movement
            Add(new InputEntry("Forward", KeyCode.W, "Character Movement"));
            Add(new InputEntry("Backward", KeyCode.S, "Character Movement"));
            Add(new InputEntry("Left", KeyCode.A, "Character Movement"));
            Add(new InputEntry("Right", KeyCode.D, "Character Movement"));
            Add(new InputEntry("Up", KeyCode.Space, "Character Movement"));
            Add(new InputEntry("Down", KeyCode.C, "Character Movement"));
            Add(new InputEntry("Roll Left", KeyCode.Q, "Character Movement"));
            Add(new InputEntry("Roll Right", KeyCode.E, "Character Movement"));
		}

		//	Static methods

        public static InputEntry Find(int _index)
        {
            return InputManager.instance.entries[_index];
        }

        public static InputEntry Find(string _name)
        {
            for (int i = 0; i < InputManager.instance.entries.Length; i++)
            {
                if (InputManager.instance.entries[i].name == _name)
                {
                    return InputManager.instance.entries[i];
                }
            }

            return null;
        }

        public static void Add(InputEntry _entry)
        {
            InputEntry[] temp = new InputEntry[InputManager.instance.entries.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = InputManager.instance.entries[i];
            }

            InputManager.instance.entries = new InputEntry[temp.Length + 1];

            for (int i = 0; i < temp.Length; i++)
            {
                InputManager.instance.entries[i] = temp[i];
            }

            InputManager.instance.entries[temp.Length] = _entry;
        }

        //  Quick access

        public static bool Get(string _name)
        {
            for (int i = 0; i < InputManager.instance.entries.Length; i++)
            {
                if (InputManager.instance.entries[i].name == _name)
                {
                    return InputManager.instance.entries[i].Get();
                }
            }

            return false;
        }

        public static bool GetKeyDown(string _name)
        {
            for (int i = 0; i < InputManager.instance.entries.Length; i++)
            {
                if (InputManager.instance.entries[i].name == _name)
                {
                    return InputManager.instance.entries[i].GetDown();
                }
            }

            return false;
        }

        public static bool GetKeyUp(string _name)
        {
            for (int i = 0; i < InputManager.instance.entries.Length; i++)
            {
                if (InputManager.instance.entries[i].name == _name)
                {
                    return InputManager.instance.entries[i].GetUp();
                }
            }

            return false;
        }

        public static float GetAxis(string _name)
        {
            for (int i = 0; i < InputManager.instance.entries.Length; i++)
            {
                if (InputManager.instance.entries[i].name == _name)
                {
                    return InputManager.instance.entries[i].GetAxis();
                }
            }

            return 0.0f;
        }
    }
}