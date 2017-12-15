using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameSystemsDirectory : MonoBehaviour
{
    [SerializeField] private TileSystem m_TileSystem = null;
    [SerializeField] private Canvas m_ScreenCanvas = null;
    [SerializeField] private Canvas m_WorldCanvas = null;
    [SerializeField] private UnitsTracker m_UnitsTracker = null;

    public TileSystem GetTileSystem() { return m_TileSystem; }
    public Canvas GetScreenCanvas() { return m_ScreenCanvas; }
    public Canvas GetWorldCanvas() { return m_WorldCanvas; }
    public UnitsTracker GetUnitsTracker() { return m_UnitsTracker; }

}