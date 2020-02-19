using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWash : MonoBehaviour
{
    public AIDroneController.DroneMode carWashType = AIDroneController.DroneMode.MINER;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AIDroneController>() != null)
        {
            other.GetComponent<AIDroneController>().droneMode = carWashType;
        }
    }

}
