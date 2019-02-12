# Postulate ORM

[![Build status](https://ci.appveyor.com/api/projects/status/i8uoaftti334xuth/branch/master?svg=true)](https://ci.appveyor.com/project/adamosoftware/postulate/branch/master)

Postulate is a CRUD library of extension methods for SQL Server and MySQL based on [Dapper](https://github.com/StackExchange/Dapper).

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
- Entity Framework is too big and complicated. Let's keep ORM lightweight.
- Code-first is a neat idea, but I don't want to write migrations.
- In the end, inline SQL is more productive than Linq, but it needs to be isolated and testable.

## Learn more

See the list of CRUD methods [here](https://github.com/adamosoftware/Postulate/wiki/Crud-method-reference).

Postulate is POCO-friendly, but you can do more by basing your model types on [Record](https://github.com/adamosoftware/Postulate/wiki/Use-Base.Record-and-IUser-for-audit-tracking-and-more).

Postulate also supports code-first development with [SchemaSync](https://github.com/adamosoftware/SchemaSync) and [SQL Model Merge](https://aosoftware.net/Project/SqlModelMerge).

Postulate has nifty features like a [Query](https://github.com/adamosoftware/Postulate/wiki/Using-the-Query-class) class for working with inline SQL, change tracking, and [navigation properties](https://github.com/adamosoftware/Postulate/wiki/Using-IFindRelated-to-implement-navigation-properties).

