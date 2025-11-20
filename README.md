DublinBikes API – Full Stack Development 

This project is part of Continuous Assessment 1. The API was built in .NET 8 and uses the dublinbike.json file as the main data source.
The goal was to load the data, expose versioned endpoints (V1 and V2), and support searching, filtering, sorting, paging, caching, and background updates.

How to run

Open the solution in Visual Studio 2022 and run the project.
Swagger will open automatically.

Example:
https://localhost:7124/swagger/index.html

Main endpoints

GET /api/v1/stations
GET /api/v1/stations/{number}
GET /api/v1/stations/summary
POST /api/v1/stations
PUT /api/v1/stations/{number}

The same endpoints are available in /api/v2, using data loaded through a CosmosDB-style service.

Features included

Load JSON on startup

Search by name and address (q parameter)

Filter by status (OPEN, CLOSED)

Filter by minBikes

Sort by name, availableBikes, or occupancy

Paging with page and pageSize

Convert last_update to Europe/Dublin time

5-minute in-memory caching

BackgroundService updating bike values at intervals

Postman collection

Full Postman collection with all endpoints:

https://bjcg2022-3128241.postman.co/workspace/Barbara-Campos's-Workspace~9e5ec028-8399-4baf-86a1-0ffc8713a578/collection/50225583-1f6d6d63-c3bc-4732-b328-9ad6f5e56b2e?action=share&creator=50225583

It includes example requests and basic Postman tests.

Tests

There is a separate test project using xUnit.
It includes one unit test for filtering and one endpoint test using WebApplicationFactory.
To run tests, open the Test Explorer and choose Run All.

Project structure

Models
Services
Versioned endpoints (V1 and V2)
BackgroundUpdateService
dublinbike.json

Notes

The API was designed to follow the assignment requirements.
No UI was created, only the API endpoints requested.

Barbara Campos 
74270
