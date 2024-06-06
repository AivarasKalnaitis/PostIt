# PostIt Project

## Overview

The PostIt project is a C# application designed to update client post codes by fetching data from an external API. The project reads client data from a JSON file, makes API calls to retrieve post codes based on client addresses, and updates the client records accordingly.

## Features

- **Import Client Data**: Reads client data from a JSON file.
- **Fetch Post Codes**: Uses an external API to fetch post codes for client addresses.
- **Update Client Records**: Updates the client records with the new post codes and logs the updates.

## Getting Started

The application includes Swagger UI for easy API exploration and testing. After running the application, navigate to https://localhost:<port>/swagger in your browser to access the Swagger UI

First run command "update-database" to add migrations with EF ORM

To get the DB script run "dotnet ef migrations script" --output <output-file.sql>

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A valid API key for the Postit API.

### Configuration

 **AppSettings**: Ensure your `appsettings.json` includes the API key:
   ```json
   {
       "AppSettings": {
           "PostitApiKey": "your_api_key_here"
       }
   }