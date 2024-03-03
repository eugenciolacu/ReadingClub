Feature: An anonymous user can create an account for itself
	An anonymous user create an account by providing to API a valid CreateUserDto object

Scenario: Anonymous user create an account
	Given a valid CreateUserDto object
		| UserName  | Email             | Password | ConfirmPassword |
		| test user | testUser@test.com | password | password        |
	And create HttpContent for request
	When create an account
	Then an HttpStatusCode.OK is returned
	And the following details are returned:
		| Status | UserName  | Email             |
		| True   | test user | testuser@test.com |