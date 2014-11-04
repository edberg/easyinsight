Easy Insight
===========

This is a .NET portable class library for the Easy Insight product from Easy Insight LLC, see http://easy-insight.com.

There is a NuGet package available here: https://www.nuget.org/packages/EasyInsight

Getting started
===============

After you have installed the NuGet package, you can get an instance of the service using the static EasyInsightFactory class:

```C#
using EasyInsight;

var service = EasyInsightFactory.Create("<apikey or username>","<apisecret or password>");
```

In order to add or replace data you need a class decorated with the DataSource attribute and DataField attibute on the fields

```C#
[DataSource("Sample Order Database")]
public class SampleOrderData
{
	[DataField("customer", "Order Customer", DataType.Grouping)] 
	public string Customer {get;set;}
	[DataField("orderAmount", "Order Amount", DataType.Measure)] 
	public decimal Amount {get; set;}
}
```

Create a collection of data instances and invoke either the Add or Replace method of the service

```C#
var data = new List<SampleOrderData>();
await service.Add(data);
//or
await service.Replace(data);
```

