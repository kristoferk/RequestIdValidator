# RequestIdValidator
Small library that checks if id in url and body are the same for http put requests.

## Installation
To install, run the following command in the Package Manager Console:
````csharp

PM> Install-Package RequestIdValidator

````

## Usage
Use IdentityValidator to compare id in url and body

````csharp

[HttpPut("{id}")]
public async Task<Customer> UpdateCustomer(int id, [FromBody] Customer customer)
{
    IIdentityValidator validator = new IdentityValidator();
    validator.VerifyIds(id, customer, c => c.Id).ThrowExceptionOnError();

	//...
            
    return customer;
}

````


## Scenarios
IF: Id in query string is the same as in the body => Everything is find and dandy

IF: Id is different compared to id in body => Throw exception

IF: Id in body is missing or empty => Copy id from querystring to body.
