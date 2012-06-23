//A test class with a single assertion, which always fails.

TUnit::registerClass(SingleFailingTest);

function SingleFailingTest::failTest()
{
   Assert::fail("This is an expected failure, demonstrating the display message for failed tests.");
}