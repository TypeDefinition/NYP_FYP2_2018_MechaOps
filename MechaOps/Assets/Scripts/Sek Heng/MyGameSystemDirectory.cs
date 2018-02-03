using UnityEngine;
using UnityEngine.Assertions;
using System.Reflection;

/// <summary>
/// A literal copy and paste from GameSystems Directory with minor changes.
/// Only for Sek Heng's use!
/// </summary>
public class MyGameSystemDirectory : GameSystemsDirectory {
    static private MyGameSystemDirectory m_SceneInstance = null;
    private bool m_Destroyed = false;

    static public new MyGameSystemDirectory GetSceneInstance()
    {
        if (m_SceneInstance == null)
        {
            MyGameSystemDirectory[] gameSystemDirectories = FindObjectsOfType<MyGameSystemDirectory>();
            if (gameSystemDirectories == null || gameSystemDirectories.Length == 0) { return null; }

            for (int i = 0; i < gameSystemDirectories.Length; ++i)
            {
                if (gameSystemDirectories[i].m_Destroyed) { continue; }
                if (m_SceneInstance == null)
                {
                    m_SceneInstance = gameSystemDirectories[i];
                }
                else
                {
                    Assert.IsTrue(false, MethodBase.GetCurrentMethod().Name + " - There can be no more than 1 GameSystemDirectory per scene!");
                }
            }
        }
        return m_SceneInstance;
    }

    // Destroy this if it another instance already exists in the scene.
    private void Awake()
    {
        if (null == m_SceneInstance)
        {
            m_SceneInstance = this;
        }
        else if (this != m_SceneInstance)
        {
            Assert.IsTrue(false, MethodBase.GetCurrentMethod().Name + " - There can only be 1 MyGameSystemDirectory per scene!");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        m_Destroyed = true;
        if (this == m_SceneInstance) { m_SceneInstance = null; }
    }

    [Header("MyGameSystemDirectory that are NOT a child of this GameObject.")]
    [SerializeField] private Camera m_UnitInfoCamera;

    public Camera GetUnitInfoCamera() { return m_UnitInfoCamera; }
}
