Easy Insight
===========

This is a .NET portable class library for the Easy Insight product from Easy Insight LLC, see http://easy-insight.com.

There is a NuGet package available here: https://www.nuget.org/packages/EasyInsight

Getting started
===============

After you have installed the NuGet package, you can get an instance of the service using the static EasyInsightFactory class:

```C#
using EasyInsight;

var service = var service = EasyInsightFactory.Create("<apikey or username>","<apisecret or password>");
```
