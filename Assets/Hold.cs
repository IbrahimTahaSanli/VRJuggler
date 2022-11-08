using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Hold : MonoBehaviour
{
    [SerializeField] private float TriggerActiveDistance;
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private Collider Collider;

    [SerializeField] private Vector3 startPosition;

    [HideInInspector] private InputController.Controller holdBy = InputController.Controller.None;

    [HideInInspector] public static GameObject WhatLeftHandHolds;
    [HideInInspector] public static GameObject WhatRightHandHolds;

    public void OnTriggerStay(Collider other)
    {
        float val;

        switch (other.tag)
        {
            case "Right":
                InputController.Instance.GetInput(
                    InputController.Controller.Right,
                    InputController.Inputs.Grip,
                    out val
                    );
                if (val > TriggerActiveDistance)
                    Holded(InputController.Controller.Right, other.transform);

                break;
            case "Left":
                InputController.Instance.GetInput(
                    InputController.Controller.Left,
                    InputController.Inputs.Grip,
                    out val
                    );
                if (val > TriggerActiveDistance)
                    Holded(InputController.Controller.Left, other.transform);
                break;
        }
    }

    public void Holded(InputController.Controller cont, Transform other)
    {
        holdBy = cont;

        Rigidbody.isKinematic = true;
        Collider.enabled = false;

        this.transform.parent = other;

        switch (cont)
        {
            case InputController.Controller.Left:
                WhatLeftHandHolds = this.gameObject;
                break;
            case InputController.Controller.Right:
                WhatRightHandHolds = this.gameObject;
                break;
        }
    }

    public void Update()
    {
        float val;

        switch (holdBy) {
            case InputController.Controller.Left:
                InputController.Instance.GetInput(
                    InputController.Controller.Left,
                    InputController.Inputs.Grip,
                    out val
                    );
                if (val < TriggerActiveDistance)
                    Relase();
                break;

            case InputController.Controller.Right:
                InputController.Instance.GetInput(
                    InputController.Controller.Right,
                    InputController.Inputs.Grip,
                    out val
                    );
                if (val < TriggerActiveDistance)
                    Relase();
                break;


            case InputController.Controller.None:
                break;
            }
    }

    public void Relase()
    {
        Vector3 vel;
        switch (holdBy)
        {
            case InputController.Controller.Left:
                InputController.Instance.leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out vel);
                WhatLeftHandHolds = null;
                break;
            case InputController.Controller.Right:
                InputController.Instance.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out vel);
                WhatRightHandHolds = null;
                break;
            default:
                vel = Vector3.zero;
                break;
        }

        holdBy = InputController.Controller.None;

        this.transform.parent = null;

        Rigidbody.isKinematic = false;
        Rigidbody.velocity = vel;
        Rigidbody.useGravity = true;
        Collider.enabled = true;

    }
    
    public void ResetWorldPosition()
    {
        holdBy = InputController.Controller.None;

        this.transform.parent = null;

        Rigidbody.isKinematic = false;
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        Collider.enabled = true;

        this.transform.position = startPosition;

    }
}
