# Develop an ASP.Net Core Application using MySQL with Entity Framework and Identity Framework

## Convert the backend of an existing ASP.Net Core Application from Microsoft SQL Server to MySQL

### Introduction

[Read the previous posts - JWT Based Authentication Using ASP.Net Core 2.0 - Part 1](http://ffive.io/jwt-based-authentication-using-asp-net-core-2-0-part-1/ "JWT Based Authentication Using ASP.Net Core 2.0 - Part 1")

[JWT Based Authentication Using ASP.Net Core 2.0 - Part 2](http://ffive.io/jwt-based-authentication-using-asp-net-core-2-0-part-2/ "JWT Based Authentication Using ASP.Net Core 2.0 - Part 2")

[JWT Based Authentication Using ASP.Net Core 2.0 - Part 3](http://ffive.io/jwt-based-authentication-using-asp-net-core-2-0-part-3/ "JWT Based Authentication Using ASP.Net Core 2.0 - Part 3")

I'm assuming that you went through the above articles. In these examples we've used Microsoft SQL Server as our backend. In this article we will try to convert the backend to MySQL.

This article can be found [here](http://ffive.io/develop-an-asp-net-core-application-using-mysql-with-entity-framework-and-identity-framework/ "Develop an ASP.Net Core Application using MySQL with Entity Framework and Identity Framework")

### Requirements

- Visual Studio 2017 (Any Edition)
- SQL Server 2017 (Any Edition)
- .Net Core 2.0 Libraries
- [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql "Pomelo.EntityFrameworkCore.MySql") 


### Add Dependancies

Microsoft currently does not officially support MySQL data provider for EF Core. There is an open source package [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql "Pomelo.EntityFrameworkCore.MySql") maintained by Pomelo Foundation.

Run the below command in the Nuget Package Manager Console

```
Install-Package Pomelo.EntityFrameworkCore.MySql
```

### Change the DBContext configuration

In the `Startup.cs' file, modify the below line

```csharp=
services.AddDbContext<KookifyDbContext>(options => options.UseSqlServer(Configuration["DefaultConnection"]));
```

To

```csharp=
services.AddDbContextPool<KookifyDbContext>( // replace "YourDbContext" with the class name of your DbContext
                options => options.UseMySql(Configuration["DefaultConnection"],
                    mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(5, 5, 45), ServerType.MySql); // replace with your Server Version and Type
                    }
            ));
```

In MySQL according to the version, Max Key length will be differ. So we need to make sure our total key length(In case of composite key) should not increase the max limit of MySQL. We can specify the column lengths (mainly for the Ids) in the `OnModelCreating` method off your DbContext class as per below:

```csharp=
protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
            
            modelBuilder.Entity<RefreshToken>()
                 .HasAlternateKey(c => c.UserId)
                 .HasName("refreshToken_UserId");
            modelBuilder.Entity<RefreshToken>()
                .HasAlternateKey(c => c.Token)
                .HasName("refreshToken_Token");
        }
```

Now change the DefaultConnection in `appsettings.json` as per below. Make sure you enter the correct MySQL credentials.

```
"DefaultConnection": "Server=localhost;Database=eff127;User=root;Password=xxxx;",
```

### Clean-up

Delete the migration entries (all the files) under the `Migrations` folder and run the below commands in the Package Manager Console to create migration and update the database.

```
Add-Migration Initial
```

```
Update-Database
```

Now all the tables must be created in the MySQL database as below:

```shell
mysql> show tables;
+-----------------------+
| Tables_in_eff127      |
+-----------------------+
| __efmigrationshistory |
| aspnetroleclaims      |
| aspnetroles           |
| aspnetuserclaims      |
| aspnetuserlogins      |
| aspnetuserroles       |
| aspnetusers           |
| aspnetusertokens      |
| refreshtokens         |
+-----------------------+
9 rows in set (0.01 sec)
```

### Summary

Now you've successfully changed the database from SQL Server to MySQL. All the APIs we created in the previous articles will work.

### Source Code

Source code used in this article can be downloaded from [FFive Github Repository](https://github.com/ffive-io/JWTAuthenticationASPNetCore20MySql "FFive Github Repository")