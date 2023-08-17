using EVRC.Core;
using EVRC.Core.Overlay;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ControlBindingsManagerTests : MonoBehaviour
{
    public ControlBindingsManager manager;
    public ControlBindingsState controlBindingsState; 
    private string bindingsFile;
    private string bindingsPath;

    [SetUp]
    public void SetUp()
    {
        manager = new ControlBindingsManager();
        controlBindingsState = ScriptableObject.CreateInstance<ControlBindingsState>();
        manager.controlBindingsState = controlBindingsState;

        bindingsFile = Paths.ControlBindingsPath;
        bindingsPath = Path.GetDirectoryName(bindingsFile);
    }


    [Test]
    public void File_Loads_Without_Errors()
    {
        Assert.DoesNotThrow(() =>
        {
            manager.LoadControlBindings();
        });
        
    }

}
