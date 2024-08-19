# Checkout - Payment Gateway

This payment gateway at an abstract level exposes two endpoints to either process or fetch payments which would have been processed via a third party banking client.

## Structure

The API code structure has been organised into folders for:

- Controllers
- BLL (Business Logic Layer)
- DAL (Data Access Layer)
- Entities (OO entities which can be persisted in the system)
- Models (Classes required by clients for requests / responses)
- Attributes (Custom Attributes for data validation on entities)
- Database (Granular control on the DB Context being used, for this it is assumed to be an In-Memory DB)

The Test folder structure is designed to follow the structure of the API project.
	

## Pulling and running this repository

- Clone this repository to a local environment, run **npm i** if you wish (assuming npm is already installed) for githooks to be used before further committing to this repo. There is a githook to lint commit messages and which will run all unit tests before any code is committed.
- **dotnet test**: run all unit tests
- **dotnet build**: build the API project

## Assumptions

- Assumptions have been made regarding authentication and considerations around caching and rate limiting of the endpoints. All endpoints are unauthenticated and public for the purposes of this project.
- The above also being true for the banking client communication.
- Error handling has not been handled elegantly or logged with any cloud infrastructure and simply thrown for the calling client in the response.
- Auditing has not been implemented but can easily be extended to do so using EF Core interceptors or integration with cloud infrastructure.
- The calling client has stored a payment Id as a reference already.

## Testing

All testing has been automated with pre-commit githooks and using github actions which will run when code is committed or a pul-request initiated on the main branch.

A code coverage report has also been generated for analysis with this implementation.

Integration tests with Banking API excluded in this solution but can be extended to include them.

## Endpoints

**POST /api/PaymentGateway**
Expected input:
```json
  "cardNumber": "2222405343248877",
  "expiryMonth": 4,
  "expiryYear": 2025,
  "currency": "GBP",
  "amount": 100,
  "cvv": "123"
  ```

Expected output:
```json
{
  "status": 1,
  "lastFourCardDigits": "8877",
  "expiryMonth": 4,
  "expiryYear": 2025,
  "currency": "GBP",
  "amount": 100,
  "id": "4c94fd33-0afb-4854-adba-132a5e14b454",
  "createdDateTime": "2024-08-19T12:34:36.131362Z",
  "updatedDateTime": null
}
```

**GET /api/PaymentGateway**

Expected input:

Existing Guid for a previous payment
```4c94fd33-0afb-4854-adba-132a5e14b454 ```

Expected output:
```json
{
  "status": 1,
  "lastFourCardDigits": "8877",
  "expiryMonth": 4,
  "expiryYear": 2025,
  "currency": "GBP",
  "amount": 100,
  "id": "4c94fd33-0afb-4854-adba-132a5e14b454",
  "createdDateTime": "2024-08-19T12:34:36.131362Z",
  "updatedDateTime": null
}
```
