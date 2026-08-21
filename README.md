## SmartOms -- Smart Order Management System

## Mentoined Cases : About project - Key features - Archtecture - Technologies - Testing

______________________________________________________________

## About project
 __ SmartOms is a system for managing orders that include cases like Shopping cart , Order , Checkout proccess
   , Payment , Wallet ,...
   I designed this system with 'Domain Driven Design' aproach that separating main business logics from infrastructure.
   I tried to making code maintainable , testable and sacleable

   --------------------------------------------------------------

## Key features
__ Cart : 
	_ Add and remove items (services) to cart with quantity validation
	_ Real-time price updates with change detection
	_ Automatic removing inactive services from cart

__ Checkout process :
	- Wallet balence validation and withdraw , deposite
	- Invoice genaration for all finantial transactions
	- Managing database cocurrency exeption with ServiceHelper (max 2 retries on concurrency conflicts)

__ Security :
	- Password hashing with BCrypt
	- JWT authentication with Refresh Token rotation
	- Secure storage of refresh tokens 

----------------------------------------------------------------

## Architecture
__ I used Clean Architecture priciples in this project that has 4 layer : 

1. Domain layer : Include Entities , Value Objects and core buisiness logics
2. Application layer : use cases , Model DTOs and service orchestration
3. Infrastructure : Handel the data access and communicate with external services
4. Presentation :  API endpoints

---------------------------------------------------------------

## Technologies : 
- **.NET 8** – Version of dotnet
- **ASP.NET Core** – Web framework
- **Entity Framework Core** – ORM 
- **BCrypt.Net-Next** – Password hashing
- **JWT Bearer Authentication** – Token-based auth
- **xUnit** & **Moq** – Testing
- **FluentValidation** – Request validation
- **Result Pattern** – Explicit error handling

------------------------------------------------------------------

## Tests
- **150+** unit & integration tests
- **Moq** for mocking dependencies
- **xUnit** as the testing framework
- Exception testing for concurrency scenarios


----------------------------------------------------------

## About me : 
I'm a backend developer with c# programming language and asp.net core framework. please follow me in linkedin and give star in github if you intersted from this project.


## Github address : https://github.com/ErfanProgrammer335133

## Linkedin address : https://www.linkedin.com/in/erfan-ghorbani-45897842b/