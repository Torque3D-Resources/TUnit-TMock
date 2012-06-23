// ///////////////////////////////////////////////////////////////////
// An expectation is a way of defining behaviour or (excuse me for using 
// the word in the definition) creating expectations of a certain method.
//
// For instance, you would use an expectation to declare that you want
// this particular method to be called exactly two times in the method
// you're testing.  You would also use an expectation to declare that
// you want that same method to return the number "10" when called,
// or you want to delegate the logic (read: make a call) to another
// function.

///Called automatically when the expectation is created.  Should not
/// normally be called.
function Expectation::_onCreate(%this, %parent, %methodName)
{
   //TODO: Does anyone know how to accomplish this with datablocks?
   %this.returns   = NULL;           //The value this method will return
   %this.callCount = 0;              //Number of times this method has been called
   %this.callCountExpected = 1;      //Number of times this method should be called
   %this.callType  = "exact";        //What to do with callCount in relation to callCountExpected
                                     // valid types are: "exact", "at least", "at most", "any"
                                     // ("any" meaning ignore the callCount altogether)
   %this.ignoresArguments = false;   //Whether the call to this method should ignore its arguments
   %this.parent       = %parent;     //The object that calls this method
   %this.methodName   = %methodName; //The name of the method that this expectation is for
   %this.callback     = "";          //Function to call when this method is called
   %this.callOriginal = false;       //Whether or not to call the original function
}

///The method should be called exactly once
function Expectation::callOnce(%this)
{
   %this.callCountExpected = 1;
   %this.callType = "exact";
   return %this;
}

///The method should never be called
function Expectation::callNone(%this)
{
   %this.callCountExpected = 0;
   %this.callType = "exact";
   return %this;
}

///The method can be called any amount of times
function Expectation::callAny(%this)
{
   %this.callType = "any";
   return %this;
}

///The method should be called exactly the given number of times
function Expectation::callExactly(%this, %times)
{
   %this.callCountExpected = %times;
   %this.callType = "exact";
   return %this;
}

///The method should be called at least the given number of times
function Expectation::callAtLeast(%this, %times)
{
   %this.callCountExpected = %times;
   %this.callType = "at least";
   return %this;
}

///The method should be called at most the given number of times
function Expectation::callAtMost(%this, %times)
{
   %this.callCountExpected = %times;
   %this.callType = "at most";
   return %this;
}

///Arguments will be ignored when calling the method -
/// all calls to the method will fulfill this expectation
function Expectation::ignoreArguments(%this)
{
   %this.ignoresArguments = true;
   return %this;
}

///Does the same thing as callAny().ignoreArguments()
function Expectation::ignore(%this)
{
   %this.callAny().ignoreArguments();
   return %this;
}

///When the method is called, it will call another function whose name is given as a parameter.
/// The function name should not contain any parentheses or a semicolon.
/// The function will optionally be passed a single argument containing an array of the
/// arguments passed to the method, named argv.  For an example of this, see
/// MockTests::sumArguments() and MockTests::callsTest()
/// Cannot be used with returns() or callOriginalMethod()
function Expectation::calls(%this, %functionName)
{
   %this.callback = %functionName;
   return %this;
}

///When this expectation is set, the mocked method will delegate to the original method that
/// it mocked.
function Expectation::callOriginalMethod(%this)
{
   %this.callOriginal = true;
   return %this;
}

///Have the method return the given value.  Cannot be used with
/// calls() or callOriginalMethod()
function Expectation::returns(%this, %returnMe)
{
   %this.returns = %returnMe;
   return %this;
}

///Called everytime the method is called.  Returns the result of the callback, or
/// (if none exists), the return-value specified by returns().  Should not be
/// called outside of Mock.cs.
function Expectation::_onMethodCalled(%this)
{
   %this.callCount++;
   
   //"callOriginalMethod" expectation
   if(%this.callOriginal)
   {
      //Need to pass arguments in the usual fashion
      %callStr = %this._getFullMethodName() @ "(" @ %this.parent;
      for(%i = 0; %i < $Mock.maxParameters; %i++)
      {
         if(%this.argv[%i] $= "")
            break;
         %callStr = %callStr @ "," @ %this.argv[%i];
      }
      %callStr = %callStr @ ");";
      return eval(%callStr);
   }
   
   //"calls" expectation
   if(%this.callback !$= "")
      return eval(%this.callback @ "(" @ %this @ ");");

   //"returns" expectation
   return %this.returns;
}

///Check if the given expectation matches the current one.  The expectations "match" if
/// they have the same class and method name, and (assuming ignoreArguments is false) have
/// the same parameter-list.  This method should not normally be called.
function Expectation::_matches(%this, %checkMe)
{
   //Check class-name and method-name
   if(%checkMe.parent.uniqueClass !$= %this.parent.uniqueClass || %checkMe.methodName !$= %this.methodName)
      return false;
   
   //Check if we even need to check parameters
   if(%this.ignoresArguments)
      return true;

   //Check parameters
   for(%paramNum = 0; %paramNum < $Mock.maxParameters; %paramNum++)
   {
      if(%checkMe.argv[%paramNum] !$= %this.argv[%paramNum])
      {
         return false;
      }
   }
   return true;
}

///Checks if an expectation has been satisfied.  Currently, the only reason
/// a mock wouldn't be satisfied is call-count; so this just checks call-counts.
/// Should not normally be called.
function Expectation::_isSatisfied(%this)
{
   switch$(%this.callType)
   {
      case "exact":
         return %this.callCount == %this.callCountExpected;
      case "at most":
         return %this.callCount <= %this.callCountExpected;
      case "at least":
         return %this.callCount >= %this.callCountExpected;
      case "any":
         return true;
   }
   error("Unknown callType \"" @ %this.callType @ "\"!");
   return false;
}

///Accessor for Expectation.returns.  Should not normally be called.
function Expectation::_getReturnValue(%this)
{
   return %this.returns;
}

///Simply formats the full method name as a string.  Should not normally
/// be called.
function Expectation::_getFullMethodName(%this)
{
   return %this.parent.mockedClass @ "::" @ %this.methodName;
}