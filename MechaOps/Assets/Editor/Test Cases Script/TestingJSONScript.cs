using UnityEngine;
using NUnit.Framework;


public class TestingJSONScript {
    [Test]
    public void testJSONScriptOfUnitStat()
    {
        UnitStatsJSON unitStuff = new UnitStatsJSON();
        string unitJSON = JsonUtility.ToJson(unitStuff);
        Debug.Log("Unit JSON: " + unitJSON);
        if (unitJSON.Length <= 2)
        {
            Assert.Fail("This test failed because UnitStatsJSON is failed to converted to JSON: " + unitJSON);
        }
    }
}
