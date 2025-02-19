# C# Accepted Assessment app

An application for C# (.net) knowledge assessment

## Description

This is a web application that interacts with a 3nd party service (<https://fakeapi.platzi.com>/<https://api.escuelajs.co>) and serves data

We have to do some code refactoring and implement some new features

## Code refactoring

Seems that the use of http client is not so much efficient

Let's make a different, more solid, approach/implementation

## New features

**#1**

Right now only the **getAll** method supported for **products**

We have to implement **getOne** and **create** methods also

**#2**

Add implementation for **categories**

**#3**

3nd party service supports JWT auth. We have to implement and support it. Use the credentials provided to appsettings.json file.

**#4**

We must measure and log the performance of the requests. Create a middleware to achieve this.

## Implementation

* Try to understand and keep the architectural approach.
* Add unit testing.
* Add docker support.
* Using CQRS pattern will be considered as a strong plus.
* The attached collections (postman/insomnia) will help you with the requests.

## Code refactoring  
* Changed the way the httpClient is created by Configurind and injecting inside the Services the IHttpClientFactory.
* Changed the registration of the services to be Transient instead of Singletons, since they do not maintain any state. 
* Fixed a bug on ProductDto where the Images array was missing the setter, causing it to not return the list of url strings.
* Changed the default configuration to validate the Settings classes as initialized by the project, to prevent bugs in the case of configuration missing from the appsettings.json
* Implemented a JsonConverter to properly return the array of images inside the product Dto, as the external api returns a single string every time
  

## Implementation Design And Comments

**#1**  
All CRUD operation for Products have been implemented (Although only the GetOne and Create methods were requested I felt it would be interesting to add the update and delete as well since this is an assignment project).  
Implemented validation for Dtos on my layer instead of letting a request fail and return error code as a better performance practice.

**#2**
All CRUD operations for Categories have been implemented.  
Same patern with validation has been implemented for Dtos.

**#3**
I implemented JWT auth using JWTBearer library.  
For this issue I was not sure how I should handle the implementation as the external API does not require any authentication apart from the auth/profile endpoint which was not requested.  
Usually in a senario where we need to keep the access/refresh tokens of the exteranl API for further requests I would create a handler, registered as a singleton, where I would map the generated token from my appplication to an encryption of the external API's token for storage (Or some cloud based keystore in a real life scenario), then add a backround job to handle (remove) expired tokens.  
So the implementation I followed uses the auth/login to validate that the user and password are correct and stores as a claim the email of the user. Then on refresh I challenge the existance of the user by calling the users/is-available to validate that the user still exists.  
All endpoints for CRUD operations require a Bearer token. The access token can be used to authenticate while with the refresh token you can generate a new pair.  

**#4**  
A Request Performance middleware has been implemented inside the infrastructure project which will log the method the path of the endpoint and the time it took to execute.  

**#Comments**  
As another best practice I implemented an ExceptionHandler Middleware and created some custom exceptions to better describe the errors/problems that occured to the client. The middleware also logs the full exception for better understanding of the problem.  


