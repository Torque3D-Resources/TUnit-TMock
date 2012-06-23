// A test class in which all tests pass.

TUnit::registerClass(AllTestsPass);

function AllTestsPass::passTest()
{
   Assert::isTrue(1);
   Assert::isFalse(0);
   Assert::areEqual(1,1);
   Assert::areNotEqual(0,1);
   Assert::isNull(NULL);
   Assert::isNotNull(new SimObject());
}

function AllTestsPass::emptyTest()
{
}