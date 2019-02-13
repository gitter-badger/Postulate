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

- Code-first is a neat idea, but I don't want to write migrations. The [SchemaSync](https://github.com/adamosoftware/SchemaSync) project powers my database diff/merge app [SQL Model Merge](https://aosoftware.net/Project/SqlModelMerge).

- Inline SQL is more productive than Linq, but it needs to be isolated and testable with the [Query](https://github.com/adamosoftware/Postulate/wiki/Using-the-Query-class) class.

## Getting Started
Install the Nuget package for your platform:
- [Postulate.SqlServer](https://www.nuget.org/packages/Postulate.SqlServer)
- [Postulate.MySql](https://www.nuget.org/packages/Postulate.MySql)

SQL Server offers a choice of key types on your model classes: `int`, `long` and `Guid`. MySQL currently supports `int` only. In your code where you want to use Postulate [extension methods](https://github.com/adamosoftware/Postulate/wiki/Crud-method-reference), add the namespace for your platform and key type:

`using Postulate.SqlServer.IntKey` 

or

`using Postulate.SqlServer.LongKey` 

or

`using Postulate.SqlServer.GuidKey` 

or

`using Postulate.MySql.IntKey`

On each of your model classes, add an `Id` property using the type you decided on. If it's inconvenient to have an `Id` property, you can designate the key property explictly by adding the [Identity](https://github.com/adamosoftware/Postulate/blob/master/Postulate.Base/Attributes/IdentityAttribute.cs) attribute to the class. For example:
```
public class OrderHeader
{
  public int Id { get; set; } // implicit key property
  ...
}
```
or with `Identity` attribute:
```
[Identity(nameof(OrderHeaderId))]
public class OrderHeader
{
  public int OrderHeaderId { get; set; } // explicit key property
}
```
Learn more at the [Wiki](https://github.com/adamosoftware/Postulate/wiki).
