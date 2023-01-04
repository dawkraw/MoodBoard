# MoodBoard

MoodBoard is a web application where users can collaborate to create [mood boards](https://en.wikipedia.org/wiki/Mood_board) in real-time. It is created to help in the process of illustrating the artistic style of a graphic design project.

## Features
- Create and delete moodboards
- Invite other users to your moodboards to collaborate
- Upload and delete images
- Move, rotate and resize an image on canvas in real-time
- Comment on an image

## Built With
- .NET 6
- ASP.NET Core 6 
- ASP.NET Identity
- SignalR
- Entity Framework Core 6
- AutoMapper
- FluentValidation
- MediatR
- CQRS
- ImageKit
- SQL Server
- Angular 14
- TailwindCSS
- KonvaJs

## Setting up the project
- Create an [ImageKit](https://imagekit.io/) account, retrieve the needed public key, private key and URL endpoint.
- Create SQL Server database.
- Clone the repo.
- Fill in the needed API keys, JWT settings, and database connection string in *Web/appsettings.json*
- Run ```dotnet restore``` in order to get and resolve any problems with NuGet packages.
- Run ```dotnet ef database update --startup-project Web --project Infrastructure``` in order to create a database with the help of EF.
