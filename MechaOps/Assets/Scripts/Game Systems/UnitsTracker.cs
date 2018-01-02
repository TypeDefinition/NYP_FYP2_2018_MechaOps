using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A quick class to store an array of gameobject
/// </summary>
public class UnitsTracker : MonoBehaviour
{
    private Dictionary<FactionType, List<UnitStats>> m_Units = new Dictionary<FactionType, List<UnitStats>>();
    private GameEventNames m_GameEventNames = null;

    public void SetUnits(FactionType _factionType, UnitStats[] _units)
    {
        // Clear the current alive list.
        if (m_Units.ContainsKey(_factionType))
        {
            List<UnitStats> currentUnits;
            m_Units.TryGetValue(_factionType, out currentUnits);
            currentUnits.Clear();
        }
        else
        {
            m_Units.Add(_factionType, new List<UnitStats>());
        }

        List<UnitStats> factionUnits;
        m_Units.TryGetValue(_factionType, out factionUnits);
        for (int i = 0; i < _units.Length; ++i)
        {
            factionUnits.Add(_units[i]);
        }
    }

    public bool HasAliveUnits(FactionType _factionType)
    {
        return GetAliveUnits(_factionType).Length > 0;
    }

    /// <summary>
    /// This is a simple unoptimised implementation which is only suitable for a small number of units.
    /// It is done for simplicity of implementation over processing speed.
    /// </summary>
    /// <param name="_factionType"></param>
    /// <returns></returns>
    public UnitStats[] GetAliveUnits(FactionType _factionType)
    {
        if (!m_Units.ContainsKey(_factionType))
        {
            return new UnitStats[0];
        }

        List<UnitStats> result = new List<UnitStats>();
        List<UnitStats> factionUnits;
        m_Units.TryGetValue(_factionType, out factionUnits);
        for (int i = 0; i < factionUnits.Count; ++i)
        {
            if (factionUnits[i].IsAlive())
            {
                result.Add(factionUnits[i]);
            }
        }

        return result.ToArray();
    }

    public bool HasDeadUnits(FactionType _factionType)
    {
        return GetDeadUnits(_factionType).Length > 0;
    }

    /// <summary>
    /// This is a simple unoptimised implementation which is only suitable for a small number of units.
    /// It is done for simplicity of implementation over processing speed.
    /// </summary>
    /// <param name="_factionType"></param>
    /// <returns></returns>
    public UnitStats[] GetDeadUnits(FactionType _factionType)
    {
        if (!m_Units.ContainsKey(_factionType))
        {
            return new UnitStats[0];
        }

        List<UnitStats> result = new List<UnitStats>();
        List<UnitStats> factionUnits;
        m_Units.TryGetValue(_factionType, out factionUnits);
        for (int i = 0; i < factionUnits.Count; ++i)
        {
            if (!factionUnits[i].IsAlive())
            {
                result.Add(factionUnits[i]);
            }
        }

        return result.ToArray();
    }

    public bool HasFactionUnits(FactionType _factionType)
    {
        if (!m_Units.ContainsKey(_factionType)) { return false; }

        List<UnitStats> factionUnits;
        m_Units.TryGetValue(_factionType, out factionUnits);
        return factionUnits.Count > 0;
    }

    public FactionType[] GetAliveFactions()
    {
        List<FactionType> aliveFactions = new List<FactionType>();
        foreach (KeyValuePair<FactionType, List<UnitStats>> unitList in m_Units)
        {
            if (HasAliveUnits(unitList.Key))
            {
                aliveFactions.Add(unitList.Key);
            }
        }

        return aliveFactions.ToArray();
    }

    private void InitEvents()
    {
        GameEventSystem.GetInstance().SubscribeToEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    private void DeinitEvents()
    {
        GameEventSystem.GetInstance().UnsubscribeFromEvent<UnitStats, bool>(m_GameEventNames.GetEventName(GameEventNames.GameplayNames.UnitDead), OnUnitDead);
    }

    private void Awake()
    {
        m_GameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
        InitEvents();
    }

    private void OnDestroy()
    {
        DeinitEvents();
    }

    // Callbacks
    void OnUnitDead(UnitStats _deadUnit, bool _isVisible)
    {
        FactionType factionType = _deadUnit.UnitFaction;
        FactionNames factionNames = GameSystemsDirectory.GetSceneInstance().GetFactionNames();
        Assert.IsTrue(HasFactionUnits(factionType), MethodBase.GetCurrentMethod().Name + " - No units found for faction " + factionNames.GetFactionName(factionType) + "!");

        GameEventNames gameEventNames = GameSystemsDirectory.GetSceneInstance().GetGameEventNames();
        if (!HasAliveUnits(factionType))
        {
            GameEventSystem.GetInstance().TriggerEvent<FactionType>(gameEventNames.GetEventName(GameEventNames.GameplayNames.FactionDead), factionType);
        }
    }
}