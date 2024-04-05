using System.Collections.Generic;
using UnityEngine;

public class PortalManager
{
    public static readonly PortalManager instance;
    static PortalManager()
    {
        instance = new PortalManager();
    }

    public readonly List<Portal> portals;

    private PortalManager()
    {
        portals = new List<Portal>();
    }

    public void AddPortal(Portal portal)
    {
        portals.Add(portal);
    }

    public void RemovePortal(Portal portal)
    {
        portals.Remove(portal);
    }
}