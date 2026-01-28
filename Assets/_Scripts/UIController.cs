using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> vehicles = new();

    private int _activeIndex = -1;
    
    private void Awake()
    {
        // Disable everything first
        for (int i = 0; i < vehicles.Count; i++)
        {
            if (vehicles[i] != null)
                vehicles[i].SetActive(false);
        }

        // Auto-activate first vehicle if it exists
        if (vehicles.Count > 0)
        {
            ActivateVehicle(0);
        }
    }

    public void ActivateVehicle(int index)
    {
        if ((uint)index >= (uint)vehicles.Count)
            return;

        if (_activeIndex == index)
            return;

        if (_activeIndex >= 0)
        {
            var current = vehicles[_activeIndex];
            if (current != null)
                current.SetActive(false);
        }

        var next = vehicles[index];
        if (next != null)
            next.SetActive(true);

        _activeIndex = index;

        AssignCamera();
    }
    
    void AssignCamera()
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.AutoAssignTarget();
    }

    public int AddVehicle(GameObject vehicle)
    {
        if (vehicle == null)
            return -1;

        vehicle.SetActive(false);
        vehicles.Add(vehicle);

        // If this is the first vehicle ever added, activate it
        if (_activeIndex == -1)
        {
            ActivateVehicle(0);
        }

        return vehicles.Count - 1;
    }
}