# JWT Based Authentication Using ASP.Net Core 2.0 - Part 4

## Create a *JWT Token Server* and consume the *Resource APIs* using *Role, Claim and Policy based Authorization*

### Introduction

When you're developing a **RESTful API** for mobile applications or SPA, you have to make sure that you follow the industry standards while implementing the *Security, Authentication and Authorization* etc. ASP<i></i>.Net Core 2.0 provides all these features. In this article I'll talk about how to setup token based authentication using JWT(Without creating a seperate Identity Server). Then we'll walkthrough you to how to validate that JWT in Resource API using **Role, Claim and Policy based Authorization**.

### Requirements

- Visual Studio 2017 (Any Edition)
- SQL Server 2017 (Any Edition)
- .Net Core 2.0 Libraries
- [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql "Pomelo.EntityFrameworkCore.MySql") 

### Article

This sample application is based on the below tutorials. The original code was written using SQL Server, but with some minor changes in the `Startup.cs` and `KookifyDbContext.cs` you can change the database from SQL Server to MySQL. Detailed article coming soon.

[JWT Based Authentication Using ASP.Net Core 2.0 - Part 1](http://ffive.io/jwt-based-authentication-using-asp-net-core-2-0-part-1/ "JWT Based Authentication Using ASP.Net Core 2.0 - Part 1")

[JWT Based Authentication Using ASP.Net Core 2.0 - Part 2](http://ffive.io/jwt-based-authentication-using-asp-net-core-2-0-part-2/ "JWT Based Authentication Using ASP.Net Core 2.0 - Part 2")

[JWT Based Authentication Using ASP.Net Core 2.0 - Part 3](http://ffive.io/jwt-based-authentication-using-asp-net-core-2-0-part-3/ "JWT Based Authentication Using ASP.Net Core 2.0 - Part 3")
