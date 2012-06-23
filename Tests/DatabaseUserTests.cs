//A rather contrived example of mock-classes in action.

TUnit::registerClass(DatabaseUserTests);

//Expensive Database class.  This would normally be in a
// separate file
function ExpensiveDatabase::getValue(%this, %valueName)
{
   error("This method would take forever to test!");
}

function ExpensiveDatabase::setValue(%this, %valueName, %value)
{
   error("This method would take forever to test!");
}

//DatabaseUser class.  This would normally be in a
// separate file
function DatabaseUser::setPlayer(%this, %player)
{
   %this.database.setValue("Player", %player);
}

function DatabaseUser::setLevel(%this, %level)
{
   %this.database.setValue("Level", %level);
}

//DatabaseUserTests.  The actual tests for the database class.
function DatabaseUserTests::setup(%this)
{
   %this.databaseUser = new SimObject(DatabaseUser);
   %this.databaseUser.database = Mock::createMock(ExpensiveDatabase);
}

function DatabaseUserTests::setPlayerTest(%this)
{
   %user = %this.databaseUser;
   %database = %user.database;
   %database.setValue("Player", 5);
   Mock::replayAll();
   
   %user.setPlayer(5);
}

function DatabaseUserTests::setLevelTest(%this)
{
   %user = %this.databaseUser;
   %database = %user.database;
   %database.setValue("Level", 5);
   Mock::replayAll();
   
   %user.setLevel(5);
}