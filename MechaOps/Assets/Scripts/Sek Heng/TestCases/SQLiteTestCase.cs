#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEditor;
using NUnit.Framework;


namespace SQLiteTestArea
{
    /// <summary>
    /// Meant to just do some testing for MySQLiteHandler.cs.
    /// But failed horribly for now.
    /// </summary>
    public class SQLiteTestMB : MonoBehaviour, IMonoBehaviourTest {
        public float m_TimeOutTimer = 5.0f;
        MySQLiteHandler m_SQLIteHandler;

        public bool IsTestFinished
        {
            get
            {
                if (m_TimeOutTimer < 0)
                    return true;
                return false;
            }
        }
        private void Start()
        {
            GameObject zeCloneGO = GameObject.Instantiate(gameObject);
            m_SQLIteHandler = zeCloneGO.AddComponent<MySQLiteHandler>();
        }

        private void Update()
        {
            m_TimeOutTimer -= Time.deltaTime;
        }

    }

    class SQLiteTestCase
    {
        [UnityTest]
        public IEnumerator SQLiteDB_CoroutineTest()
        {
            var testGO = new GameObject();
            MySQLiteHandler zeSQLStuff = testGO.AddComponent<MySQLiteHandler>();
            // Making sure it is not null!
            Assert.IsNotNull(zeSQLStuff);
            // Access the editor database so that nothing will be interfered with the streaming asset database!
            string zeEditorDatabasePath = Application.dataPath + "/Editor/TestCaseDatabase";
            Debug.Log("The editor database filepath: " + zeEditorDatabasePath);
            zeSQLStuff.DataBaseName = zeEditorDatabasePath;

            yield break;
        }
    }
}
#endif