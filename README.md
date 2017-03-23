# U33A2SqlTool

generic testing proceedures defined in Panda tester, actual tests defined in U33A2SqlTools\Testing

U33A2SqlTool Directory explaination:
Logger: used for generic logging output.
DataManager: centralized data manager to organize operation.
Models/Tables: holds the table definitions for the SQL tables on the database.
SQL/BaseTypes: definitions for generic SQL objects/types
SQL/Statements: definition for Statement class which is used to support SQL statement generation via intellisense.
SQLTools: series of helper methods/functions
    SqlFormatter: helper methods to format collections to a single string.
    SqlHelper: helper methods for Random outputs and pluralizing strings.
    SqlManager: Used to expose access methods for the database. contains generic methods for commonly used SQL commands.
    SqlQuery: used to expose direct access to the database using pure SQL commands and handling communication with the database.
              also parses Database queries into a SQLCollection (defined in BaseTypes).
    SqlStatementBuilder: Helper class used to generate very frequently used SQL statements.
Testing: where the tests are defined
    SqlSeeder: used to generate and insert dummy information used for testing.
    SqlTest: used to define generic Itest behavior
    SqlTestFactory: Used to define the actual tests.
 
