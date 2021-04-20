using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class Interactable : MonoBehaviour
    {
        [Header("Interaction")]
        public bool CanInteract = true;
        public float InteractionRange = 10.0f;
        public bool ScaleInteractionRange = false;
        public float InteractCooldown = 1.0f;

        private float baseInteractionRange = 10.0f;
        private Timer cooldownTimer;

        public void Awake()
        {
            baseInteractionRange = InteractionRange;
            cooldownTimer = new Timer(InteractCooldown);
            PreInitialize();
        }

        public void Start()
        {
            Initialize();
        }

        public virtual void PreInitialize()
        {

        }

        public virtual void Initialize()
        {

        }

        public void TryHover(GameObject _interactor)
        {
            if (CanInteract == true)
            {
                if (ScaleInteractionRange == true)
                {
                    InteractionRange = baseInteractionRange * transform.localScale.x;
                }

                if (Vector3.Distance(transform.position, _interactor.transform.position) <= InteractionRange)
                {
                    Hover(_interactor);
                }
            }
        }

        public void TryInteract(GameObject _interactor)
        {
            if (CanInteract == true)
            {
                if (InteractCooldown > 0.0f)
                {
                    if (cooldownTimer.Tick() == false)
                    {
                        return;
                    }
                }

                if (ScaleInteractionRange == true)
                {
                    InteractionRange = baseInteractionRange * transform.localScale.x;
                }

                if (Vector3.Distance(transform.position, _interactor.transform.position) <= InteractionRange)
                {
                    if (InteractCooldown > 0.0f) { cooldownTimer.Reset(); }
                    Interact(_interactor);
                }
            }
        }

        public virtual void Hover(GameObject _interactor = null)
        {

        }

        public virtual void Interact(GameObject _interactor = null)
        {

        }
    }
}