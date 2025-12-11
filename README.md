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
### Project Overview

For Assignment 2 I extended my existing DublinBikes API from Assignment 1 and built a Blazor Server client called **fs-2025-assessment-1-74270.BlazorClient**.  
The Blazor app connects to my **v2 API**, which is backed by **Azure Cosmos DB** (Cosmos Emulator running locally). All station data is stored in the `DorsetCosmos` database and the `myContainer` container.

The client implements a **master/detail view** for bike stations. The left side shows a table with all stations, including:

- Station name  
- Address  
- Status (OPEN / CLOSED)  
- Available bikes vs total stands (e.g. `10 / 40`)  

When the user selects a row, a **detail panel** is shown on the right. The detail view includes:

- Full name and address  
- Status badge  
- Total stands, available bikes, available stands  
- A basic capacity/occupancy visual (bikes vs stands)  
- Last update in a friendly format  
- Latitude and longitude (which could be linked to a map)

Filters, Search, Sorting and Paging

At the top of the Stations page I implemented several filters:

- A **text search** box that filters by station name or address  
- A **status filter** (All, Open, Closed)  
- A **minimum available bikes** numeric filter  
- **Sorting controls** (e.g. sort by Name and choose Asc/Desc)

When the user changes any filter or sort option, the list is refreshed by calling my API. If no stations match the current filters, an empty-state message is shown. If the API fails, a red error banner is displayed so the user understands what happened.

API Integration and CRUD

The Blazor client uses an injected `StationsApiClient` service which wraps `HttpClient`.  
The base URL is configured in `appsettings.json` and points to my local API, for example:

- `https://localhost:7124/api/v2/stations`

I implemented the following operations against the v2 API (CosmosDB):

- `GET /api/v2/stations` – paged and filterable list of stations  
- `GET /api/v2/stations/{number}` – load a single station  
- `POST /api/v2/stations` – create a new station from the Blazor form  
- `PUT /api/v2/stations/{number}` – update an existing station  
- `DELETE /api/v2/stations/{number}` – delete a station

On the UI, there is a **form for adding a new station** and the same panel is reused for **editing** an existing station. When the user submits the form, the Blazor client sends the appropriate POST or PUT request to the API and refreshes the list on success.


How to Run

1. Start the **Azure Cosmos DB Emulator** and ensure the `DorsetCosmos` database and `myContainer` container exist with some station data.
2. Run the **API project** (`fs-2025-assessment-1-74270`) so that it listens on `https://localhost:7124`.
3. Run the **Blazor client project** (`fs-2025-assessment-1-74270.BlazorClient`).
4. Open the browser at the Blazor URL (for example `https://localhost:xxxx`) and go to the **Stations** page.


Barbara Campos 
74270
