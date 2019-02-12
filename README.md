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
- Entity Framework is too big and complicated. Let's keep ORM lightweight, but capable. See the list of CRUD methods [here](https://github.com/adamosoftware/Postulate/wiki/Crud-method-reference). POCO is fine, but do more with [Record](https://github.com/adamosoftware/Postulate/wiki/Use-Base.Record-and-IUser-for-audit-tracking-and-more) and [navigation properties](https://github.com/adamosoftware/Postulate/wiki/Using-IFindRelated-to-implement-navigation-properties).

- Code-first is a neat idea, but I don't want to write migrations. The [SchemaSync](https://github.com/adamosoftware/SchemaSync) project powers my database diff/merge app [SQL Model Merge](https://aosoftware.net/Project/SqlModelMerge).

- Inline SQL is more productive than Linq, but it needs to be isolated and testable with the [Query](https://github.com/adamosoftware/Postulate/wiki/Using-the-Query-class) class.
