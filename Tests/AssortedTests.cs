// Various other tests demonstrating the usage of TUnit.

TUnit::registerClass(AssortedTests);

///Variable which holds class properties
$AssortedTests = new SimObject();

function AssortedTests::setup()
{
   $AssortedTests.setup = true;
}

function AssortedTests::setupTest()
{
   Assert::isTrue($AssortedTests.setup);
}

function AssortedTests::catchingAMistakeTest()
{
   Assert::areEqual(Math::multiply(2, 3), 6, "The method was implemented incorrectly!");
}

function Math::multiply(%num1, %num2)
{
   return %num1 + %num2;
}

function AssortedTests::privateMethod()
{
   Assert::fail("Methods not ending in 'test' should not be called from TUnit!");
}

function AssortedTests::booleanTest()
{
   Assert::areEqual(true, true);
   Assert::areEqual(false, false);
   Assert::areNotEqual(true, false);
   Assert::isTrue(false $= false, "String compare, same type");
}

function AssortedTests::numberTest()
{
   Assert::areEqual(10, 10);
   Assert::areNotEqual(10, 11);
}

function AssortedTests::stringTest()
{
   Assert::areEqual("", "");
   Assert::areEqual("a", a);
   Assert::areEqual("1", 1);
   Assert::areNotEqual("batman", "robin");
}

function AssortedTests::differentTypesTest()
{
   //areEqual and areNotEqual use string-compare
   Assert::areNotEqual("", false);
   Assert::areNotEqual("", NULL);
   Assert::areNotEqual(false, NULL);
   Assert::areNotEqual(NULL, 0, "0 is not the same as NULL");
   Assert::areEqual(false,   0, "0 is the same as false");
   
   //isNull/isNotNull and isFalse/isTrue use boolean compare
   Assert::isNull(false);
   Assert::isNull("");
   Assert::isNull(0);
   Assert::isFalse(NULL);
   Assert::isFalse("");
   Assert::isFalse(0);
   
   //Examples of string compare
   Assert::isTrue(false $= false, "String compare works on boolean-types...");
   Assert::isFalse(false $= "", "But allows to make comparison between different types!");
   Assert::isFalse(false $= NULL);
   Assert::isFalse("" $= NULL);
   Assert::isFalse("" $= 0);
   Assert::isTrue("1" $= 1);
}