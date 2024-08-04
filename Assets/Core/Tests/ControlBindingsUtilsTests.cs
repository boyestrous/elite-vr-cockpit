using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using EVRC.Core;
using NUnit.Framework;
using UnityEngine;


public class ControlBindingsUtilsTests
{
    private string tempBindingsPath;


    [SetUp]
    public void Setup()
    {
        tempBindingsPath = Path.Combine(Application.temporaryCachePath, "temp.4.0.binds");
    }

    #region ------------- ParseControlBinding Tests ------------------

    [Test]
    public void ParseControlBinding_Returns_Controls()
    {
        //Arrange
        string xml = @"<LeftThrustButton>
		                <Primary Device=""Keyboard"" Key=""Key_Q"" />
		                <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Left"" />
	                </LeftThrustButton>";
        XElement control = XElement.Parse(xml);

        //Act
        var controlBinding = new ControlButtonBinding
        {
            Primary = EDControlBindingsUtils.ParseControlBinding(control, "Primary"),
            Secondary = EDControlBindingsUtils.ParseControlBinding(control, "Secondary")
        };

        //Assert
        Assert.NotNull(controlBinding.Primary);
        Assert.NotNull(controlBinding.Primary.Key); //literal keyboard key
        Assert.NotNull(controlBinding.Primary.Device); 
        Assert.NotNull(controlBinding.Secondary);
        Assert.NotNull(controlBinding.Secondary.Key); //literal keyboard key
        Assert.NotNull(controlBinding.Secondary.Device);

        Assert.AreEqual("Key_Q",controlBinding.Primary.Key);
        Assert.AreEqual("Joy_POV3Left", controlBinding.Secondary.Key);
    }
    
    [Test]
    public void ParseControlBinding_Returns_Modifiers()
    {
        //Arrange
        string xml = @"<LeftThrustButton>
		                <Primary Device=""Keyboard"" Key=""Key_N"">
			                <Modifier Device=""Keyboard"" Key=""Key_RightAlt"" />
			                <Modifier Device=""Keyboard"" Key=""Key_LeftControl"" />
		                </Primary>
		                <Secondary Device=""Keyboard"" Key=""Key_S"">
			                <Modifier Device=""Keyboard"" Key=""Key_LeftAlt"" />
		                </Secondary>
	                </LeftThrustButton>";
        XElement control = XElement.Parse(xml);

        var expectedModifierPrimaryOne = new ControlButtonBinding.KeyModifier()
        {
            Key = "Key_RightAlt",
            Device = "Keyboard"
        };
        var expectedModifierPrimaryTwo = new ControlButtonBinding.KeyModifier()
        {
            Key = "Key_LeftControl",
            Device = "Keyboard"
        };
        var expectedModifierSecondaryOne = new ControlButtonBinding.KeyModifier()
        {
            Key = "Key_LeftAlt",
            Device = "Keyboard"
        };


        //Act
        var controlBinding = new ControlButtonBinding
        {
            Primary = EDControlBindingsUtils.ParseControlBinding(control, "Primary"),
            Secondary = EDControlBindingsUtils.ParseControlBinding(control, "Secondary")
        };

        //Assert
        Assert.NotNull(controlBinding.Primary);
        Assert.AreEqual(2, controlBinding.Primary.Modifiers.Count);
        Assert.That(controlBinding.Primary.Modifiers.Contains(expectedModifierPrimaryOne));
        Assert.That(controlBinding.Primary.Modifiers.Contains(expectedModifierPrimaryTwo));

        Assert.NotNull(controlBinding.Secondary);
        Assert.AreEqual(1, controlBinding.Secondary.Modifiers.Count);
        Assert.That(controlBinding.Secondary.Modifiers.Contains(expectedModifierSecondaryOne));

    }

    #endregion


    #region ------------- ParseFile Tests ------------------

    [Test]
    public void Invalid_Path_Throws_Exception()
    {
        string invalidPath = Path.Combine(Paths.OverlayStatePath,"docthatdoesntexist.4.0.binds");

        void TestDelegate()
        {
            EDControlBindingsUtils.ParseFile(invalidPath);
        }
        Assert.Throws<FileNotFoundException>(TestDelegate);
    }

    [Test]
    public void ParseFile_Returns_ValidEntries()
    {
        // Arrange - write some valid examples to a temp file
            // one entry has primary value only
            // one has secondary value only
            // one has both
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
                    <Root PresetName=""Custom"" MajorVersion=""4"" MinorVersion=""0"">
	                    <KeyboardLayout>en-US</KeyboardLayout>
	                    <YawLeftButton>
		                    <Primary Device=""Keyboard"" Key=""Key_A"" />
		                    <Secondary Device=""{NoDevice}"" Key="""" />
	                    </YawLeftButton>
	                    <YawRightButton>
		                    <Primary Device=""{NoDevice}"" Key="""" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Right"" />
	                    </YawRightButton>
                        <LeftThrustButton>
		                    <Primary Device=""Keyboard"" Key=""Key_Q"" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Left"" />
	                    </LeftThrustButton>
                    </Root>";
        File.WriteAllText(tempBindingsPath, xml);

        // Act 
        Dictionary<EDControlButton, ControlButtonBinding> readBindings 
            = EDControlBindingsUtils.ParseFile(tempBindingsPath);

        // Assert
        Assert.AreEqual(3,readBindings.Count);
    }

    [Test]
    public void ParseFile_Skips_Undefined_EDControlButton()
    {
        // Arrange - write some valid examples to a temp file
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
                    <Root PresetName=""Custom"" MajorVersion=""4"" MinorVersion=""0"">
	                    <KeyboardLayout>en-US</KeyboardLayout>
	                    <YawLeftButton>
		                    <Primary Device=""Keyboard"" Key=""Key_A"" />
		                    <Secondary Device=""{NoDevice}"" Key="""" />
	                    </YawLeftButton>
	                    <YawRightButton>
		                    <Primary Device=""Keyboard"" Key=""Key_D"" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Right"" />
	                    </YawRightButton>
                        <BUTTONTHATISNTREAL>
		                    <Primary Device=""Keyboard"" Key=""Key_D"" />
		                    <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_POV3Right"" />
	                    </BUTTONTHATISNTREAL>
                    </Root>";
        File.WriteAllText(tempBindingsPath, xml);

        // Act 
        Dictionary<EDControlButton, ControlButtonBinding> readBindings
            = EDControlBindingsUtils.ParseFile(tempBindingsPath);

        // Assert
        Assert.AreEqual(2, readBindings.Count); // only 2 of the 3 controls have matching EDControlButtons
    }



    #endregion

    [Test]
    public void UpdateBindingXml_DoesntOverwrite_VJoyBindings()
    {
        // Copy the template file to the cache folder
        string filepath = Path.Combine(Application.temporaryCachePath, "tempPreserveVjoy.binds");
        File.Copy(Paths.BindingsTemplatePath, filepath, true);

        // Read the file
        Dictionary<EDControlButton, ControlButtonBinding> bindsFile = EDControlBindingsUtils.ParseFile(filepath);

        // The template file (at the time of this writing) defines SelectTarget with a secondary binding that is a vJoy control and a primary control that is a keyboard binding
        EDControlButton selectTarget = EDControlButton.SelectTarget;
        bindsFile.TryGetValue(selectTarget, out var selectTargetControls);
        var secondaryBinding = selectTargetControls.Secondary;
        Assert.IsTrue(secondaryBinding.IsValidVJoyPress); // Double check that the binding is still a vjoy binding

        // Assign a new KeyBinding to replace the original value
        var newBinding = TestUtils.CreateKeyBinding("Key_J", new List<string>());

        // Set a new binding without specifying the one to overwrite
        EDControlBindingsUtils.UpdateBindingXml(filepath, selectTarget.ToString(), newBinding);

        var fileAfterUpdate = EDControlBindingsUtils.ParseFile(filepath);
        fileAfterUpdate.TryGetValue(selectTarget, out var updatedControlButtonBinding);
        var updatedSecondaryBinding = updatedControlButtonBinding.Secondary;
        var updatedPrimaryBinding = updatedControlButtonBinding.Primary;
        Assert.AreEqual(secondaryBinding, updatedSecondaryBinding); // Secondary should remain unchanged
        Assert.AreEqual(newBinding, updatedPrimaryBinding); // Primary is changed to the new binding

        // Set a new binding and specify the secondary one (vjoy) -> should fail
        // Set a new binding on a control with a missing primary bindingv
        // Set a new binding on a conrol with a missing secondary binding
    }


    [Test]
    public void UpdateBindingXml_OverwritesMatching_WhenProvided()
    {
        // Copy the template file to the cache folder
        string filepath = Path.Combine(Application.temporaryCachePath, "temp.binds");
        File.Copy(Paths.BindingsTemplatePath, filepath,true);

        // Read the file
        Dictionary<EDControlButton, ControlButtonBinding> firstRead = EDControlBindingsUtils.ParseFile(filepath);

        // pick a control and get the primary binding
        EDControlButton targetControl = EDControlButton.CamTranslateForward;
        firstRead.TryGetValue(targetControl, out var originalControlButtonBinding);
        var originalPrimaryBinding = originalControlButtonBinding.Primary;

        // Assign a new KeyBinding to replace the original value
        var newBinding = TestUtils.CreateKeyBinding("Key_J", new List<string>());
        Assert.AreNotEqual(originalPrimaryBinding, newBinding); // make sure we don't accidentally make it already the same

        // Act
        EDControlBindingsUtils.UpdateBindingXml(filepath, targetControl.ToString(), newBinding, originalPrimaryBinding);


        // Assert
        var fileAfterUpdate = EDControlBindingsUtils.ParseFile(filepath);
        fileAfterUpdate.TryGetValue(targetControl, out var updatedControlButtonBinding);
        var updatedPrimaryBinding = updatedControlButtonBinding.Primary;
        Assert.AreEqual(newBinding, updatedPrimaryBinding);
    }

    [Test]
    public void UpdateBindingXml_SetsSecondary_WhenBlankOldBinding()
    {
        // Copy the template file to the cache folder
        string filepath = Path.Combine(Application.temporaryCachePath, "temp.binds");
        File.Copy(Paths.BindingsTemplatePath, filepath, true);

        // Read the file
        Dictionary<EDControlButton, ControlButtonBinding> firstRead = EDControlBindingsUtils.ParseFile(filepath);

        // This should be empty in the template file
        EDControlButton targetControl = EDControlButton.YawToRollButton;
        firstRead.TryGetValue(targetControl, out var originalControlButtonBinding);
        var originalPrimaryBinding = originalControlButtonBinding.Primary;
        var originalSecondaryBinding = originalControlButtonBinding.Secondary;
        //Make sure they're both unbound before we start
        Assert.AreEqual(originalPrimaryBinding, TestUtils.UnboundKeyBinding);
        Assert.AreEqual(originalSecondaryBinding, TestUtils.UnboundKeyBinding);

        var newBinding = TestUtils.CreateKeyBinding("Key_M", new List<string>());

        // Act
        EDControlBindingsUtils.UpdateBindingXml(filepath, targetControl.ToString(), newBinding, TestUtils.UnboundKeyBinding);

        // Assert
        var fileAfterUpdate = EDControlBindingsUtils.ParseFile(filepath);
        fileAfterUpdate.TryGetValue(targetControl, out var updatedControlButtonBinding);
        var updatedPrimaryBinding = updatedControlButtonBinding.Primary;
        var updatedSecondaryBinding = updatedControlButtonBinding.Secondary;
        Assert.AreEqual(updatedPrimaryBinding, TestUtils.UnboundKeyBinding); // Primary remains unbound
        Assert.AreEqual(newBinding, updatedSecondaryBinding); // Secondary is updated with the new binding
        Assert.AreNotEqual(originalSecondaryBinding, updatedSecondaryBinding); // Secondary is not the same as the original secondary (make sure we didn't screw up the test itself)
    }

    [TearDown]
    public void TearDown()
    {
        // Get the temporary cache path
        string cachePath = Application.temporaryCachePath;

        // Delete all files and directories within the cache path
        TestUtils.DeleteFilesAndDirectories(cachePath);

        // Optional: Verify that the cache path is empty after cleanup
        Assert.IsFalse(Directory.Exists(cachePath), "Temporary cache path is not empty after cleanup");
    }
}
