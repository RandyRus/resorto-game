﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager
{
    private static Dictionary<RegionInformation, ArrayHashSet<RegionInstance>> regionsInMap = new Dictionary<RegionInformation, ArrayHashSet<RegionInstance>>();

    public delegate void RegionDelegate(RegionInstance instance);
    public static event RegionDelegate OnRegionCreated;
    public static event RegionDelegate OnRegionRemoved;

    public static CustomEventManager<RegionInformation> OnRegionCreatedEventManager { get; private set; } = new CustomEventManager<RegionInformation>();
    public static CustomEventManager<RegionInformation> OnRegionRemovedEventManager { get; private set; } = new CustomEventManager<RegionInformation>();

    public static bool RegionPlaceable(RegionInformation info, Vector2Int position)
    {
        if (!TileInformationManager.Instance.TryGetTileInformation(position, out TileInformation tileInfo))
            return false;

        if (tileInfo.Region != null)
            return false;

        return true;
    }

    public static bool TryCreateRegion(RegionInformation info, HashSet<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions) {
            if (!RegionPlaceable(info, pos))
            {
                return false;
            }
        }

        RegionInstance newInstance = info.CreateInstance(info.RegionName, positions);

        HashSet<Vector2Int> allInstancePositions = new HashSet<Vector2Int>();
        allInstancePositions.UnionWith(positions);

        //If any neighbours have same region type (info), combine all
        //TODO: Could be done more efficiently if only check edges O(sqrt(n)) instead of O(n)
        HashSet<RegionInstance> neighbouringRegionsOfSameType = new HashSet<RegionInstance>();
        foreach (Vector2Int pos in positions)
        {
            TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);
            foreach (Vector2Int neighbour in tileInfo.neighbours)
            {
                if (TileInformationManager.Instance.TryGetTileInformation(neighbour, out TileInformation nTileInfo) && nTileInfo.Region?.regionInformation == info)
                {
                    //Add neighbouring region to new instance and remove old instance
                    HashSet<Vector2Int> oldInstancePositions = nTileInfo.Region.GetRegionPositions();
                    newInstance.AddPositions(oldInstancePositions);
                    allInstancePositions.UnionWith(oldInstancePositions);
                    RemoveRegion(nTileInfo.Region);
                }

            }
        }

        //Add region to dictionary
        if (regionsInMap.TryGetValue(info, out ArrayHashSet<RegionInstance> set))
        {
            set.Add(newInstance);
        }
        else
        {
            ArrayHashSet<RegionInstance> newSet = new ArrayHashSet<RegionInstance>();
            newSet.Add(newInstance);
            regionsInMap.Add(info, newSet);
        }

        //Set regions on tilemap
        foreach (Vector2Int pos in allInstancePositions)
        {
            TileInformationManager.Instance.TryGetTileInformation(pos, out TileInformation tileInfo);
            tileInfo.SetRegion(newInstance);
        }

        OnRegionCreated?.Invoke(newInstance);
        OnRegionCreatedEventManager.TryInvokeEventGroup(info, new object[] { newInstance });

        return true;
    }

    public static void RemoveRegion(RegionInstance instance)
    {
        regionsInMap[instance.regionInformation].Remove(instance);

        instance.InvokeRegionRemoved();

        OnRegionRemoved?.Invoke(instance);
        OnRegionRemovedEventManager.TryInvokeEventGroup(instance.regionInformation, new object[] { instance });
    }

    public static RegionInstance GetRandomRegionInstanceOfType(RegionInformation type)
    {
        if (regionsInMap.TryGetValue(type, out ArrayHashSet<RegionInstance> set))
        {
            return (set.Count > 0) ? set.GetRandom() : null;
        }
        else
        {
            return null;
        }
    }
}