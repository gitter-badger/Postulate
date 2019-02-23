[![Build status](https://ci.appveyor.com/api/projects/status/i8uoaftti334xuth/branch/master?svg=true)](https://ci.appveyor.com/project/adamosoftware/postulate/branch/master)

Postulate is a library of `IDbConnection` [extension methods](https://github.com/adamosoftware/Postulate/wiki/Crud-method-reference) for SQL Server and MySQL made with [Dapper](https://github.com/StackExchange/Dapper). Here are examples with `FindAsync` and `SaveAsync`.

```
using (var cn = GetConnection())
{
  // find a record
  var order = await cn.FindAsync<Order>(id);
  
  // create and save
  var employee = new Employee()
  {
    FirstName = "Whoever",
    LastName = "Nobody",
    HireDate = new DateTime(2002, 3, 13)
  };
  await cn.SaveAsync<Employee>(employee);
}
```
## Why another ORM framework?
- Entity Framework is too big and complicated. Let's keep ORM lightweight, but capable. See the list of CRUD methods [here](https://github.com/adamosoftware/Postulate/wiki/Crud-method-reference). POCO is fine, but do more with [Record](https://github.com/adamosoftware/Postulate/wiki/Use-Base.Record-and-IUser-for-audit-tracking-and-more) and [navigation properties](https://github.com/adamosoftware/Postulate/wiki/Using-IFindRelated-to-implement-navigation-properties).

- Code-first is a neat idea, but I don't want to write migrations. The [SchemaSync](https://github.com/adamosoftware/SchemaSync) project powers my database diff/merge commercial app [SQL Model Merge](https://aosoftware.net/Project/SqlModelMerge). This isn't required, but it gives you a way to merge model classes to a physical database interactively.

- In the end, inline SQL is more productive than Linq, but it needs to be isolated and testable with the [Query](https://github.com/adamosoftware/Postulate/wiki/Using-the-Query-class) class.

## Getting Started
Install the Nuget package for your platform:
- [Postulate.SqlServer](https://www.nuget.org/packages/Postulate.SqlServer)
- [Postulate.MySql](https://www.nuget.org/packages/Postulate.MySql)

In the SQL Server package, there are three primary key types supported: `int`, `long`, and `Guid`. Import the appropriate namespace for the key type you want to use: `Postulate.SqlServer.IntKey`, `LongKey`, and `GuidKey` respectively. See [SQL Server CRUD Methods](https://github.com/adamosoftware/Postulate/wiki/SQL-Server-CRUD-Methods)

The MySql package supports only `int` key types via namespace `Postulate.MySql.IntKey`. See [MySQL CRUD Methods](https://github.com/adamosoftware/Postulate/wiki/MySQL-CRUD-Methods)

Both MySQL and SQL Server providers, as well as any new back-ends implemented should reference these [base CRUD methods](https://github.com/adamosoftware/Postulate/wiki/Crud-method-reference) and surface them as extension methods.

Learn more at the [Wiki](https://github.com/adamosoftware/Postulate/wiki).
