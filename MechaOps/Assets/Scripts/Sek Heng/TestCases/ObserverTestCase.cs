using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace ObserverSystemTestUnit
{
    public class ObserverTestCase
    {
        [UnityTest]
        public IEnumerator TestingLamdaFunctionRemoving()
        {
            var testGO = new GameObject();
            ObserverSystemScript testObserver = testGO.AddComponent<ObserverSystemScript>();
            var LOLWhat = true;
            testObserver.SubscribeEvent("TestLamda", () => LOLWhat = false);
            testObserver.UnsubscribeEvent("TestLamda", () => LOLWhat = false);
            //if (testObserver == null)
            //{
            Assert.Fail();
            //}
            // for some reason this is not working
            testObserver.TriggerEvent("TestLamda");
            yield break;
        }
    }
}
