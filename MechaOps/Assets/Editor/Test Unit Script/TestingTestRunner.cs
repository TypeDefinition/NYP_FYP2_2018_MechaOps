using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class TestingTestRunner {
	[Test, Timeout(1000)]
	// Use this for initialization
	public void TestFunctionThatWillAlwaysFail () {
		Assert.Fail("Testing the failure of a unit test at Test Runner Window!");
	}
}