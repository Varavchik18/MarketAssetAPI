# MAGNISE MARKET ASSET API DESCRIPTION
    Welcome to the MAGNISE MARKET ASSET API documentation. This API provides endpoints for retrieving asset information and historical price data, including real-time market prices. The API supports filtering, pagination, and various query parameters to customize the data retrieval according to user requirements.
## GETTING STARTED

### 1. Running via Docker

To run the project using Docker, follow these steps:

1. **Install Docker**:
   Ensure you have Docker installed on your machine. You can download and install Docker from [here](https://www.docker.com/get-started).

2. **Clone the repository**:
   ```sh
   git clone https://github.com/Varavchik18/MarketAssetAPI.git

3. **Ensure that compose.yaml is present in the root directory**
4. **Build and Run Containers:**
    Run the following command in the root directory where compose.yaml is located
    docker-compose up --build
5. **Access the API:**
Once the containers are up and running, you can access the API at:
    http://localhost:8080/

### 2. Running locally
    To run the project locally, follow these steps:

1. **Install .NET 8 SDK:**
    Ensure you have the .NET 8 SDK installed on your machine. You can download and install it from here:
    https://dotnet.microsoft.com/en-us/download

2. **Install SQL Server:**
    Ensure you have SQL Server installed and running on your machine. You can download and install SQL Server from here:
    https://www.microsoft.com/en-us/sql-server/sql-server-downloads

3. **Clone the repository:**
    git clone https://github.com/Varavchik18/MarketAssetAPI.git
    cd magnise-market-asset-api

4. **Update Connection String:**
    Update the connection string in appsettings.json to point to your local SQL Server instance:
    {
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=MarketDB;User Id=sa;Password=YourStrongPassword123!"
    }
    }
5. **Apply Migrations:**
    Open a terminal or command prompt and navigate to the project directory. Run the following commands to apply migrations and update the database:
    dotnet ef database update
6. **Run the application**
dotnet run
7. **Access the API**
    Once the application is running, you can access the API documentation via Swagger at:
    http://localhost:49149/swagger
    http://localhost:49149

## API Documentation

## 1. Get Assets List

### Endpoint
    `GET /api/assets/list`

### Description
    Interacts with the FintaChartsClientService to fetch asset data from the FintaCharts API based on the query parameters provided by the user. It then maps the API response to the internal asset model and updates the database with the retrieved assets. The response includes paging information and a list of assets matching the query criteria.

### Request Parameters
    | Parameter | Type     | Description                                      |
    |-----------|----------|--------------------------------------------------|
    | Provider  | string   | (Optional) Filter assets by provider             |
    | Kind      | string   | (Optional) Filter assets by kind                 |
    | Symbol    | string   | (Optional) Filter assets by symbol               |
    | Page      | int      | (Optional) Page number for pagination (default: 1) |
    | Size      | int      | (Optional) Number of items per page (default: 10) |

### Request Example
        http
        GET /api/assets/list?provider=exampleProvider&kind=exampleKind&symbol=exampleSymbol&page=1&size=10

### Response Body
    {
    "paging": {
        "page": "int",
        "pages": "int",
        "items": "int"
    },
    "data": [
        {
        "id": "string",
        "symbol": "string",
        "kind": "string",
        "description": "string",
        "tickSize": "decimal",
        "currency": "string",
        "baseCurrency": "string",
        "mappings": {
            "mappingKey": {
            "symbol": "string",
            "exchange": "string",
            "defaultOrderSize": "int"
            }
        }
        }
    ]
    }

### Response Example:
    {
    "paging": {
        "page": 1,
        "pages": 10,
        "items": 100
    },
    "data": [
        {
        "id": "123",
        "symbol": "AAPL",
        "kind": "stock",
        "description": "Apple Inc.",
        "tickSize": 0.01,
        "currency": "USD",
        "baseCurrency": "USD",
        "mappings": {
            "NASDAQ": {
            "symbol": "AAPL",
            "exchange": "NASDAQ",
            "defaultOrderSize": 100
            }
        }
        }
    ]
    }


## 2. Get Historical Prices Count Back

### Endpoint
    `GET /api/historicalprices/count-back`

### Description
    Endpoint interacts with the FintaChartsClientService to fetch historical price data within a specified date range. It also subscribes to real-time price data using the FintaChartsClientService_WS to provide the most recent market prices. The response includes both the historical price data and the current real-time data for the specified instrument.

### Request Parameters
    | Parameter     | Type     | Description                            |
    |---------------|----------|----------------------------------------|
    | InstrumentId  | string   | The ID of the instrument               |
    | Provider      | string   | The provider of the price data         |
    | Interval      | int      | The interval for the price data        |
    | Periodicity   | string   | The periodicity of the price data      |
    | BarsCount     | int      | The number of bars to retrieve         |

### Request Example
    http
    GET /api/historicalprices/count-back?instrumentId=123&provider=exampleProvider&interval=1&periodicity=daily&barsCount=10

### Response Body
    {
    "assetId": "string",
    "realTimeData": {
        "last": {
        "timestamp": "DateTime",
        "price": "decimal",
        "volume": "long"
        },
        "ask": {
        "timestamp": "DateTime",
        "price": "decimal",
        "volume": "long"
        },
        "bid": {
        "timestamp": "DateTime",
        "price": "decimal",
        "volume": "long"
        }
    },
    "historicalData": [
        {
        "time": "DateTime",
        "open": "decimal",
        "high": "decimal",
        "low": "decimal",
        "close": "decimal",
        "volume": "long"
        }
    ]
    }


### Response Example:
    {
    "assetId": "123",
    "realTimeData": {
        "last": {
        "timestamp": "2024-06-29T12:34:56Z",
        "price": 150.25,
        "volume": 1000
        },
        "ask": {
        "timestamp": "2024-06-29T12:34:56Z",
        "price": 150.30,
        "volume": 500
        },
        "bid": {
        "timestamp": "2024-06-29T12:34:56Z",
        "price": 150.20,
        "volume": 500
        }
    },
    "historicalData": [
        {
        "time": "2024-06-28T00:00:00Z",
        "open": 148.50,
        "high": 150.50,
        "low": 148.00,
        "close": 150.00,
        "volume": 2000
        }
    ]
    }


## 3. Get Historical Prices Date Range

### Endpoint
    `GET /api/historicalprices/date-range`

### Description
    Endpoint interacts with the FintaChartsClientService to fetch historical price data within a specified date range. It also subscribes to real-time price data using the FintaChartsClientService_WS to provide the most recent market prices. The response includes both the historical price data and the current real-time data for the specified instrument.
    
### Request Parameters
    | Parameter     | Type     | Description                                 |
    |---------------|----------|---------------------------------------------|
    | InstrumentId  | string   | The ID of the instrument                    |
    | Provider      | string   | The provider of the price data              |
    | Interval      | int      | The interval for the price data             |
    | Periodicity   | string   | The periodicity of the price data           |
    | StartDate     | DateTime | The number of bars to retrieve  (YYYY-MM-DD)|
    | EndDate       | DateTime | The number of bars to retrieve  (YYYY-MM-DD)|

### Request Example
    http
    GET /api/historicalprices/date-range?instrumentId=123&provider=exampleProvider&interval=1&periodicity=daily&startDate=2024-01-01&endDate=2024-06-29

### Response Body
    {
    "assetId": "string",
    "realTimeData": {
        "last": {
        "timestamp": "DateTime",
        "price": "decimal",
        "volume": "long"
        },
        "ask": {
        "timestamp": "DateTime",
        "price": "decimal",
        "volume": "long"
        },
        "bid": {
        "timestamp": "DateTime",
        "price": "decimal",
        "volume": "long"
        }
    },
    "historicalData": [
        {
        "time": "DateTime",
        "open": "decimal",
        "high": "decimal",
        "low": "decimal",
        "close": "decimal",
        "volume": "long"
        }
    ]
    }



### Response Example:
    {
    "assetId": "123",
    "realTimeData": {
        "last": {
        "timestamp": "2024-06-29T12:34:56Z",
        "price": 150.25,
        "volume": 1000
        },
        "ask": {
        "timestamp": "2024-06-29T12:34:56Z",
        "price": 150.30,
        "volume": 500
        },
        "bid": {
        "timestamp": "2024-06-29T12:34:56Z",
        "price": 150.20,
        "volume": 500
        }
    },
    "historicalData": [
        {
        "time": "2024-06-28T00:00:00Z",
        "open": 148.50,
        "high": 150.50,
        "low": 148.00,
        "close": 150.00,
        "volume": 2000
        }
    ]
    }

