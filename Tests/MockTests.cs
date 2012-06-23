// A class which demonstrates some abilities of mocks

TUnit::registerClass(MockTests);

//Need a class to mock:
function MockTestsMockClass::foo(%this, %arg1, %arg2, %arg3)
{
   //Could do anything at all here, it doesn't matter
}
function MockTestsMockClass::bar(%this)
{
   %this.someVariable = 10;
}

//Need a class to test:
function MockTestsTestClass::callsFoo(%this, %mockClassInstance)
{
   return %mockClassInstance.foo(1, 2, 3);
}

function MockTestsTestClass::callsBar(%this, %mockClassInstance)
{
   return %mockClassInstance.bar();
}

//Create mocks of the previous class and set up expectations
function MockTests::callsBarTest()
{
   //Create a mock of MockTestsMockClass.  Now we can pass it to
   // any class expecting a real MockTestsMockClass, but it will
   // behave how we want it to, not how it originally behaved.
   %mockedClassInstance = Mock::createMock(MockTestsMockClass);
   
   //Our "target" is the class we're testing.  Typically, this will
   // be the only real class in your test
   %target = new SimObject(MockTestsTestClass);
   
   //Now we set up expectations for our mock-objects.  We're going to
   // be calling %target.callsBar(), and so we only have one expectation:
   // that %mockedClassInstance.bar() gets called.  We do that simply by
   // "calling" it.  This action is essentially recorded, to later be
   // verified
   %mockedClassInstance.bar();
   
   //To finish recording, we call Mock::replayAll().  This puts all
   // mock-objects in a "replay" state, where they act out the behaviours
   // previously given to them (we haven't set up any behaviours yet, other
   // then expecting to be called)
   Mock::replayAll();
   
   //Now, all that's left is to call the method we're testing
   %target.callsBar(%mockedClassInstance);
   
   //After the method ends, all expectations are verified - %mockedClassInstance
   // is called, as expected, so the test passes!  Try commenting out the call
   // in MockTestsTestClass::callsBar() and see what happens!
}

function MockTests::returnsTest()
{
   %mockedClassInstance = Mock::createMock(MockTestsMockClass);
   %target = new SimObject(MockTestsTestClass);
   
   //We can do more than simply check that methods are called, however -
   // we can also have them return values when they're called!  This can
   // be helpful when the target class calls methods in other classes and
   // expects values returned - you can test what would happen with all
   // sorts of different return values right here in the code!
   //To create a "returns" expectation, simply follow the expected method
   // call with a call to "returns" and give it a return-value.
   //Here, we expect %mockedClassInstance.bar() to return the value "5"
   %mockedClassInstance.bar().returns(5);
   
   Mock::replayAll();
   
   //Since MockTestsTestClass::callsBar() immediately returns the value
   // of %mockedClassInstance.bar(), we can easily test that the value
   // "5" really was returned as so:
   Assert::areEqual(%target.callsBar(%mockedClassInstance), 5);
   
   //Notice that the ability to specify return values means that we can
   // test classes that rely on other classes, even if nothing in the other
   // classes has been implemented yet!
}

function MockTests::sumArguments(%args)
{
   %sum = 0;
   for(%i = 0; %args.argv[%i] !$= ""; %i++)
      %sum += %args.argv[%i];
   return %sum;
}

function MockTests::callsTest()
{
   %mockedClassInstance = Mock::createMock(MockTestsMockClass);
   %target = new SimObject(MockTestsTestClass);
   
   //We can also have our mock-method call other method when it's called
   // inside the test-method.  To do this, we use the "calls" method, with
   // the parameter as the name of the method we want to call
   %mockedClassInstance.foo(1,2,3).calls("MockTests::sumArguments");
   
   Mock::replayAll();
   
   //The value returned in the method specified by "calls" is the value
   // returned by the mocked-method
   Assert::areEqual(%target.callsFoo(%mockedClassInstance), 6);
}

function MockTests::callOriginalMethodTest()
{
   %mockedClassInstance = Mock::createMock(MockTestsMockClass);
   %target = new SimObject(MockTestsTestClass);

   //We can have the original method called using callOriginalMethod.
   // We use .ignore() to state that we want the original bar() method
   // to be called every time, no matter the arguments
   %mockedClassInstance.bar().callOriginalMethod().ignore();
   
   Mock::replayAll();
   
   //The original bar() method set %this.someVariable to 10.  So, we assert
   // that this actually happened.  Notice that variables are not mocked.
   %mockedClassInstance.someVariable = 0;
   %target.callsBar(%mockedClassInstance);
   Assert::areEqual(%mockedClassInstance.someVariable, 10);
}

function MockTests::setup(%this)
{
   //The function "setup" is called automatically at the beginning of every test,
   // with the optional variable "this" which is persisted across tests of the
   // same class.  This means that objects we create in every test - such as
   // %target and %mockedClassInstance, can simply be moved to being added
   // as dynamic fields to %this during the setup.
   %this.mockedClassInstance = Mock::createMock(MockTestsMockClass);
   %this.target = new SimObject(MockTestsTestClass);
}

function MockTests::setupAndCallCountTest(%this)
{
   %this.mockedClassInstance.bar().callAny();
   %this.mockedClassInstance.foo().ignoreArguments();
   Mock::replayAll();
   
   %this.target.callsFoo(%this.mockedClassInstance);
   
   //This test passes.  Why?  The callAny() expectation means that the method
   // could be called any number of times, including 0 (which it was).  This is
   // often useful for accessor/mutator methods which you don't care how many
   // times are called.
   //In addition, the call to "ignoreArguments()" means that any call to foo() will
   // match that expectation, including the call foo(1,2,3) in 
   // MockTestsTestClass::callsFoo
}

//That's it!  For more expectations available, see the "public" methods
// (methods without underscores) given in Expectation.cs.