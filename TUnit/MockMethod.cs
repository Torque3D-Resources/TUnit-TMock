//A skeleton for mocking any method
function __CLASS_NAME__::__METHOD_NAME__(%this, %argv0, %argv1, %argv2,
         %argv3, %argv4, %argv5, %argv6, %argv7, %argv8,
         %argv9, %argv10, %argv11, %argv12, %argv13, 
         %argv14, %argv15)
{
   %expectation = new SimObject(Expectation);
   %expectation._onCreate(%this, __METHOD_NAME__);
   for(%i = 0; %i < $Mock.maxParameters; %i++)
   {
      %expectation.argv[%i] = %argv[%i];
   }
   
   return Mock::_methodCalled(%this, %expectation);
}