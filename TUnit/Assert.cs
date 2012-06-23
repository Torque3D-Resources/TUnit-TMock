// ///////////////////////////////////////////////////////////////////
// The class which takes care of all the test-assertions.  An
// assertion is the condition you're testing for - if an assertion
// fails, the test fails.
// For examples of how to use Assert, see the files in the Tests
// directory.

///The global variables that stores the "properties" of this class
$Assert = new SimObject();

///Fail the test immediately
function Assert::fail(%message)
{
   Assert::_assertException(%message, "Assert::fail()");
}

///Asserts that the two objects are equal
function Assert::areEqual(%one, %two, %message)
{
   if(%one != %two || %one !$= %two)
      Assert::_assertException(%message, "Assert::areEqual(" @ %one @ ", " @ %two @ ")");
}

///Asserts that the two objects are not equal
function Assert::areNotEqual(%one, %two, %message)
{
   //%one == %two is always true if both are strings
   if(%one != %two || %one !$= %two)
      return;
   Assert::_assertException(%message, "Assert::areNotEqual(" @ %one @ ", " @ %two @ ")");
}

///Asserts that the given object evaluates to true
function Assert::isTrue(%testMe, %message)
{
   if(!%testMe)
      Assert::_assertException(%message, "Assert::isTrue(" @ %testMe @ ")");
}

///Asserts that the given object evaluates to false
function Assert::isFalse(%testMe, %message)
{
   if(%testMe)
      Assert::_assertException(%message, "Assert::isFalse(" @ %testMe @ ")");
}

///Asserts that the given object is null
function Assert::isNull(%testMe, %message)
{
   if(%testMe != NULL)
      Assert::_assertException(%message, "Assert::isNull(" @ %testMe @ ")");
}

///Asserts that the given object is not null
function Assert::isNotNull(%testMe, %message)
{
   if(%testMe == NULL)
      Assert::_assertException(%message, "Assert::isNotNull(" @ %testMe @ ")");
}

///Called at the beginning of every test
function Assert::_setup(%testName)
{
   $Assert.lastAssertedTest = %testName;
   $Assert.lastTestPassed = true;
}

///Throws an exception on a failed assertion.  This method should never be called
/// (outside of Assert.cs)
function Assert::_assertException(%message, %assertError)
{
   $Assert.lastTestPassed = false;
   %errorMessage = "[Assertion failed] " @ TUnit::_getCurrentTestName() @
                   ": " @ %assertError;
   if(%message !$= "")
      %errorMessage = %errorMessage @ " - " @ %message;

   error(%errorMessage);
}