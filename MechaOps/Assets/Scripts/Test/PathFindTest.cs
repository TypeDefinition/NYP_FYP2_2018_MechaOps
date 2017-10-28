using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class PathFindTest : MonoBehaviour
{

    public TileSystem m_TileSystem;
    public TileId m_StartTile;
    public TileId m_EndTile;

    [Range(0, 10000)] public int m_Speed;
    [SerializeField] public TileAttributeOverride[] m_Overrides;

    public GameObject m_ReachableTileMarker;
    public Color m_PathColor = Color.red;
    public Color m_ReachableColor = Color.white;

    [SerializeField] TileId[] m_Path;
    [SerializeField] TileId[] m_ReachableArea;

    private LineRenderer m_LineRenderer;

    private void Start()
    {
        m_LineRenderer = gameObject.GetComponent<LineRenderer>();
        if (m_LineRenderer == null)
        {
            Debug.Log(MethodBase.GetCurrentMethod().Name + " - No LineRenderer found! PathFindTest needs a LineRenderer to work!");
        }

        m_Path = m_TileSystem.GetPath(m_Speed, m_StartTile, m_EndTile, m_Overrides);
        if (m_Path != null)
        {
            Vector3 offset = new Vector3(0.0f, 0.0f, 0.2f);
            List<Vector3> linePositions = new List<Vector3>();
            for (int i = 0; i < m_Path.Length; ++i)
            {
                linePositions.Add(m_TileSystem.GetTile(m_Path[i]).transform.position + offset);
            }

            m_LineRenderer.positionCount = linePositions.Count;
            m_LineRenderer.SetPositions(linePositions.ToArray());
        }

        m_ReachableArea = m_TileSystem.GetReachableTiles(m_Speed, m_StartTile, m_Overrides);
        if (m_ReachableArea != null)
        {
            Vector3 offset = new Vector3(0.0f, 0.0f, 0.2f);
            List<Vector3> markerPositions = new List<Vector3>();
            for (int i = 0; i < m_ReachableArea.Length; ++i)
            {
                GameObject marker = GameObject.Instantiate(m_ReachableTileMarker);
                marker.transform.position = m_TileSystem.GetTile(m_ReachableArea[i]).transform.position + offset;
                marker.transform.SetParent(gameObject.transform);
                marker.GetComponent<MeshRenderer>().material.color = m_ReachableColor;

                for (int j = 0; j < m_Path.Length; ++j)
                {
                    if (m_ReachableArea[i].Equals(m_Path[j]))
                    {
                        marker.GetComponent<MeshRenderer>().material.color = m_PathColor;
                        break;
                    }
                }
            }
        }
    }

}