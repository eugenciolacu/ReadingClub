Feature: Integration tests for UserController class


Scenario: Anonymous user try create an account with invalid input (empty DTO scenario)
	An anonymous user try to create an account by providing to API an empty CreateUserDto
	Given an empty CreateUserDto object
	And create HttpContent for request
	When try create an account
	Then an HttpStatusCode.BadRequest is returned
	And the following details of errors for created account with empty DTO are
		| ErrorCount | UserNameError                    | EmailError0                    | EmailError1            | PasswordError                   | ConfirmPasswordError                    |
		| 4          | The User name field is required. | The Email address is required. | Invalid Email address. | The Password field is required. | The Confirm password field is required. |



Scenario: Anonymous user try create an account with invalid input (wrong DTO fields scenario)
	An anonymous user try to create an account by providing to API a CreateUserDto object with invalid Email field and Password and ConfirmPassword fields that do not match
	Given an invalid CreateUserDto object
		| UserName | Email | Password | ConfirmPassword     |
		| Test     | Test  | Test     | Something different |
	And create HttpContent for request
	When try create an account
	Then an HttpStatusCode.BadRequest is returned
	And the following details of errors for created account with invalid fields DTO are
		| ErrorCount | EmailError             | ConfirmPasswordError                                 |
		| 2          | Invalid Email address. | The Password and Confirmation password do not match. |



Scenario: Anonymous user create an account
	An anonymous user create an account by providing to API a valid CreateUserDto object
	Given a valid CreateUserDto object
		| UserName  | Email             | Password | ConfirmPassword |
		| test user | testUser@test.com | password | password        |
	And create HttpContent for request
	When try create an account
	Then an HttpStatusCode.OK is returned
	And the following details for created account are returned
		| Status | UserName  | Email             |
		| True   | test user | testuser@test.com |