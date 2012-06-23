// ///////////////////////////////////////////////////////////////////
// The class which creates mock-objects.  To craete a mock-object of 
// a class, call (within a unit-test)
//    Mock::createMock(ClassName);
// where ClassName is, obviously, the name of the class you want to
// mock.  You can then set expectations on those mocks - see
// Expectation.cs and MockTests.cs for more information.
//
// Once you are finished setting up expectations, simply call
//    Mock::replayAll();
// to put all mocked objects into a replay state.  Then call the
// method you wish to test - all expectations are automatically verified
// after the test finishes.
//
// For more information on mock-objects, google "mock-objects tutorial"

///The global variable that stores the "properties" of this class
$Mock = new SimObject();

///Called automatically when the program starts
function Mock::_onStart()
{
   //Setup globals
   $Mock.maxParameters = 16; //Max number of parameters that can be passed to
                             // a mocked method.  If changed, both MockMethod.cs
                             // and Mock::_dontCallThis() must be edited.
   $Mock.mocksCounter  = 0;  //A counter used in generating unique classnames
   $Mock.mockedObjects = new SimSet(); //The set which holds all objects that have
                                       // been mocked for the current test
   $Mock.methodString  = "";   //The method which overrides all an objects other methods.
                               // See MockMethod.cs
   $Mock.isRecording   = true; //Whether our mock-objects are in recording or
                               // playback state
   
   //Execute files
   exec("./Expectation.cs");

   //Open the file
   %fileName = "./MockMethod.cs";
   %file = new FileObject();
   if(!%file.openForRead(%fileName))
   {
      error("ERROR: Cannot open " @ %fileName);
      return;
   }
   
   //Read the entire file into a string
   %nextLine = %file.readLine();
   while(%nextLine !$= "")
   {
      $Mock.methodString = $Mock.methodString @ %nextLine @ "\n";
      %nextLine = %file.readLine();
   }
}

///Called automatically at the beginning of every test
function Mock::_setup()
{
   $Mock.mockedObjects.clear();
   $Mock.isRecording = true;
}

///Called automatically at the end of every test (replaces the more
/// traditional "verifyAll")
function Mock::_teardown()
{
   //Check that all expectations for all objects are satisfied
   %mockedObjectsCount = $Mock.mockedObjects.getCount();
   for(%objNum = 0; %objNum < %mockedObjectsCount; %objNum++)
   {
      %mockedObject = $Mock.mockedObjects.getObject(%objNum);
      %expectationsCount = %mockedObject.expectations.getCount();
      for(%expNum = 0; %expNum < %expectationsCount; %expNum++)
      {
         %expectation = %mockedObject.expectations.getObject(%expNum);
         if(!%expectation._isSatisfied())
         {
            Mock::_failExpectation("Expectation failed", %expectation);
         }
      }
   }
}

///Returns a mock of the passed object.
function Mock::createMock(%mockClass)
{
   //Can't create object of name 'SimObject'; see 
   // http://www.garagegames.com/mg/forums/result.thread.php?qt=77551
   // for more details
   if(%mockClass $= "simobject")
      %mockClass = "";
   
   //%mockClass could be the name of an object, or just a namespace
   // of methods
   if(isObject(%mockClass))
      %mockMe = %mockClass;
   else
      %mockMe = new SimObject(%mockClass);
   
   //Generate a unique classname by concatenating our object's ID with a counter
   %uniqueClassname = "MOCK_" @ %mockMe @ "_" @ $Mock.mocksCounter++;
   
   //Mock each method in turn
   %methodCount = %mockMe.getMethodCount();
   for(%i = 0; %i < %methodCount; %i++)
   {
      Mock::_mockMethod(%uniqueClassname, %mockMe.getMethod(%i));
   }
   
   //Create the mocked-object itself
   %mockedObject = new SimObject(%uniqueClassname);
   %mockedObject.expectations = new SimSet();
   %mockedObject.uniqueClass  = %uniqueClassname; //Can't call getName() - it's been mocked!
   %mockedObject.mockedClass  = %mockClass;
   $Mock.mockedObjects.add(%mockedObject);
   
   //Return an instance of our new class
   return %mockedObject;
}

///Called when recording of expectations is finished,
///and testing of expectations should begin.
function Mock::replayAll()
{
   $Mock.isRecording = false;
}

///Called to mock an individual method
function Mock::_mockMethod(%className, %methodName)
{
   %methodStr = strreplace($Mock.methodString, "__CLASS_NAME__", %className);
   %methodStr = strreplace(%methodStr, "__METHOD_NAME__", %methodName);
   eval(%methodStr);
}

///Called by every mocked method; checks or adds the expectation as necessary
function Mock::_methodCalled(%this, %expectation)
{
   if($Mock.isRecording)
   {
      //Recording state, add the expectation
      %this.expectations.add(%expectation);
      return %expectation;
   } else {
      //Replay state, check the expectation against those recorded
      %expectationNum = %this.expectations.getCount();
      for(%i = 0; %i < %expectationNum; %i++)
      {
         %checkMe = %this.expectations.getObject(%i);

         //Mark the expectation as called and return its return-value
         if(%checkMe._matches(%expectation))
         {
            return %checkMe._onMethodCalled();
         }
      }

      //Method was called unexpectedly in a non-record state, fail.
      Mock::_failExpectation("Unexpected call", %expectation);
   }
}

///Called when a mocked object has an unexpected call, or an
///expectation is never met
function Mock::_failExpectation(%message, %expectation)
{
   //Fail test
   $Assert.lastTestPassed = false;
   
   //Format the error message
   %errorMessage = "[" @ %message @ "] " @ TUnit::_getCurrentTestName() @ " - " @
                   %expectation._getFullMethodName() @ "(" @ %expectation.argv[0];

   //Add all parameters
   for(%i = 1; %i < $Mock.maxParameters; %i++)
   {
      if(%expectation.argv[%i] $= "")
         break;
      %errorMessage = %errorMessage @ ", " @ %expectation.argv[%i];
   }
   %errorMessage = %errorMessage @ ")";
   
   //Display error
   error(%errorMessage);
}

///Necessary for MockMethod.cs to work correctly.  See
/// http://www.garagegames.com/mg/forums/result.thread.php?qt=77534
/// for more details
function Mock::_dontCallMe(%this, %argv0, %argv1, %argv2,
         %argv3, %argv4, %argv5, %argv6, %argv7, %argv8,
         %argv9, %argv10, %argv11, %argv12, %argv13, 
         %argv14, %argv15)
{
   echo(%this @ %argv0 @ %argv1 @ %argv2 @
        %argv3 @ %argv4 @ %argv5 @ %argv6 @ %argv7 @ %argv8 @
        %argv9 @ %argv10 @ %argv11 @ %argv12 @ %argv13 @ 
        %argv14 @ %argv15);
}

//Finally, call _onStart()
Mock::_onStart();