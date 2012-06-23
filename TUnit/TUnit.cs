// ///////////////////////////////////////////////////////////////////
// The main class which does all the dirty-work.  To use, simply add 
// the line:
//    exec("./TUnit/TUnit.cs");
// to the startGame() function in game.cs.  Then, add the following
// line to the top of all test-classes:
//    TUnit::registerClass(className);
// (replacing className with the name of your class.  The function
//  does not actually need to be called at the top of your class -
//  this is only a convention).  Additionally, all test-files must be
//  in the "Tests" directory
//
// To run the test-cases, call TUnit::runAllTests() or
// TUnit::runTest(myClass) (replacing myClass with the name of your
// class) from the console.  All test-methods must end in the word
// "test"
//
// Throughout this document, the term "class" is, obviously, meant to
// refer to the TorqueScript concept of a namespace.

///The global variable that stores the "properties" of this class
$TUnit = new SimObject();

///(Should be) called when the program starts
function TUnit::_onStart()
{
   //Call other TSUnit classes
   exec("./Assert.cs");
   exec("./Mock.cs");

   //The dataset to store all test-classes in
   $TUnit.testClasses = new SimSet();
   
   //Execute all files in Tests directory
   %searchPattern = "*/Tests/*.cs";
   %file = findFirstFile(%searchPattern);
   while(%file !$= "")
   {
      exec(%file);
      %file = findNextFile(%searchPattern);
   }
}

///Registers a class with TUnit.  A class must be registered before
/// it will be tested.  Typically, this method is called within the
/// test-class itself, but that's only a convention.
function TUnit::registerClass(%testClass)
{
   %object = new SimObject(%testClass);
   $TUnit.testClasses.add(%object);
}

///Runs all the tests.  All test-classes must be registered via TUnit::registerClass()
/// first, and all test-methods must end in the word "Test".  The method setup() is run,
/// if present, before each test, and the method teardown() is run, if present, after
/// each test.
function TUnit::runAllTests()
{
   %numTestClasses = $TUnit.testClasses.getCount();
   
   //Keep track of number of tests run
   %testCount = new SimObject();
   %testCount.testsFailed = 0;
   %testCount.testsTotal = 0;
   
   for(%i = 0; %i < %numTestClasses; %i++)
   {
      %testMe = $TUnit.testClasses.getObject(%i);
      %returnedCount = TUnit::_runTestsOnObject(%testMe);
      %testCount.testsFailed += %returnedCount.testsFailed;
      %testCount.testsTotal  += %returnedCount.testsTotal;
   }
   TUnit::_displayResults(%testCount);
}

///Run all tests for a certain class.  All test-classes must be registered via
/// TUnit::registerClass() first, and all test-methods must end in the word "Test"
/// The method setup() is run, if present, before each test, and the method teardown()
/// is run, if present, after each test.
function TUnit::runTests(%testClass)
{
   %numTestClasses = $TUnit.testClasses.getCount();
   
   //Need to interate through classes anyways
   for(%i = 0; %i < %numTestClasses; %i++)
   {
      %testMe = $TUnit.testClasses.getObject(%i);
      if(%testMe.getName() $= %testClass)
      {
         %testCount = TUnit::_runTestsOnObject(%testMe);
         TUnit::_displayResults(%testCount);
         return;
      }
   }
   error("Class not found: " @ %testClass);
}

///Method which takes an instance of a test-class and runs all tests associated with
/// it.  This method should never be called directly (outside of TUnit.cs).
/// Returns an object that contains counts of passed/failed tests.
function TUnit::_runTestsOnObject(%object)
{
   $TUnit.currentClass = %object.getName();
   %methodCount = %object.getMethodCount();
   
   //Keep track of number of tests run
   %testCount = new SimObject();
   %testCount.testsFailed = 0;
   %testCount.testsTotal  = 0;
   
   for(%i = 0; %i < %methodCount; %i++)
   {
      //Test all methods that end in the word "Test"
      %methodName = %object.getMethod(%i);
      if(strlwr(getSubStr(%methodName, strlen(%methodName)-4, 4)) $= "test")
      {
         //Setup test
         $TUnit.currentMethod = %methodName;
         Assert::_setup();
         Mock::_setup();
         if(%object.isMethod("setup"))
            %object.setup();
         
         //Run test
         %object.call(%methodName);
         
         //Teardoown test
         if(%object.isMethod("teardown"))
            %object.teardown();
         Mock::_teardown();

         //Increment the test-counts
         if(!$Assert.lastTestPassed)
            %testCount.testsFailed++;
         %testCount.testsTotal++;
      }
   }
   
   //Display warning if class contains no tests
   if(%testCount.testsTotal == 0)
   {
      warn("Warning: " @ $TUnit.currentClass @ " has no tests");
   }
   return %testCount;
}

///Displays the results at the end of the test-run.  Shouldn't be called normally.
function TUnit::_displayResults(%testCount)
{
   if(%testCount.testsFailed == 0)
      %result = "\cp\c9All tests passed!\co";
   else if(%testCount.testsFailed == 1)
      %result = "\cp\c21 test failed\co";
   else
      %result = "\cp\c2" @ %testCount.testsFailed @ " tests failed\co";
   %result = %result SPC "(" @ %testCount.testsTotal @ " total)";
   echo(%result);
}

///Returns the name of the test currently being run
function TUnit::_getCurrentTestName()
{
   return $TUnit.currentClass @ "::" @ $TUnit.currentMethod;
}

//Finally, call _onStart()
TUnit::_onStart();