Feature: Integration tests for UserController class

Scenario: Anonymous user try create an account with invalid input (empty DTO scenario)
	An anonymous user try to create an account by providing to API an empty CreateUserDto
	Given an empty CreateUserDto object
	And create HttpContent for create request
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
	And create HttpContent for create request
	When try create an account
	Then an HttpStatusCode.BadRequest is returned
	And the following details of errors for created account with invalid fields DTO are
		| ErrorCount | EmailError             | ConfirmPasswordError                                 |
		| 2          | Invalid Email address. | The Password and Confirmation password do not match. |

Scenario: Anonymous user try create an account where username already exists
	An anonymous user try create an account, but username in DTO already exists in database
	Given an valid user in database instantiate an CreateUserDto with the same username
	And create HttpContent for create request
	When try create an account
	Then an HttpStatusCode.OK is returned
	And the following details of error for created account with same fields are
		| Status | Message                        |
		| False  | This user name already exists. |

Scenario: Anonymous user try create an account where email already exists
	An anonymous user try create an account, but email in DTO already exists in database
	Given an valid user in database instantiate an CreateUserDto with the same email
	And create HttpContent for create request
	When try create an account
	Then an HttpStatusCode.OK is returned
	And the following details of error for created account with same fields are
		| Status | Message                    |
		| False  | This email already exists. |

Scenario: Anonymous user try create an account where username and email already exists
	An anonymous user try create an account, but username and email in DTO already exists in database
	Given an valid user in database instantiate an CreateUserDto with the same username and email
	And create HttpContent for create request
	When try create an account
	Then an HttpStatusCode.OK is returned
	And the following details of error for created account with same fields are
		| Status | Message                                 |
		| False  | This user name and emai already exists. |

Scenario: Anonymous user create an account
	An anonymous user create an account by providing to API a valid CreateUserDto object
	Given a valid CreateUserDto object
		| UserName  | Email             | Password | ConfirmPassword |
		| test user | testUser@test.com | password | password        |
	And create HttpContent for create request
	When try create an account
	Then an HttpStatusCode.OK is returned
	And the following details for created account are returned
		| Status | UserName  | Email             |
		| True   | test user | testuser@test.com |





Scenario: Anonymous user try login with empty DTO
	An anonymous user try to ligin with empty DTO
	Given an empty UserLoginDto
	And create HttpContent for login request
	When try login
	Then an HttpStatusCode.BadRequest is returned
	And the following details of errors for login are
		| ErrorCount | EmailError                   | PasswordError                   |
		| 2          | The Email field is required. | The Password field is required. |

Scenario: Anonymous user login
	An anonymous user login with valid credentials, user must be in database
	Given a valid user in database login with its credentials
	And create HttpContent for login request
	When try login
	Then an HttpStatusCode.OK is returned
	And a non-null and non-empty token is returned

Scenario: Anonymous user try login with inexistend credentials
	An anonymous user try login with inexistent credentials
	Given invalid credentials in UserLoginDto
	And create HttpContent for login request
	When try login
	Then an HttpStatusCode.OK is returned
	And the following details of errors for login with inexistent credentials are
		| Status | Message                                      |
		| False  | User do not exists or password is incorrect. |





Scenario: Is token valid when an invalid TokenDto is provided
	Validate token when TokenDto parameter is generated based on an UserDto object with empty fields
	Given a token generated based on an UserDto object with empty fields
	And create HttpContent for isTokenValid
	When try validate token
	Then an HttpStatusCode.OK is returned
	And following details of error for isTokenValid are
		| Status | Message                                  |
		| False  | Token validation failed, user not found. |

Scenario: Is token valid when an valid TokenDto is provided
	Validate token when TokenDto parameter is generated based on an valid UserDto, user exists in database
	Given a token generated based on a valid UserDto that exists in database
	And create HttpContent for isTokenValid
	When try validate token
	Then an HttpStatusCode.OK is returned
	And returned token is the same, not-null and not-empty

Scenario: Is token valid when an valid TokenTdo is provided, token expiration less than a day
	Validate token when TokenDto parameter is generated based on an valid UserDto, user exists in database, but expiration is less than a day
	Given a token generated based on a valid UserDto that exists in database but with expiration less than a day
	And create HttpContent for isTokenValid
	When try validate token
	Then an HttpStatusCode.OK is returned
	And return a new token with updated expiration date, not-null and not-empty

Scenario: Is token valid when an valid TokenDto is provided, token expiration more than a day
	Validate token when TokenDto parameter is generated based on an valid UserDto, user exists in database, but expiration is more than a day
	Given a token generated based on a valid UserDto that exists in database but with expiration more than a day
	And create HttpContent for isTokenValid
	When try validate token
	Then an HttpStatusCode.OK is returned
	And following details of error for isTokenValid are
		| Status | Message                  |
		| False  | Token validation failed. |

Scenario: Is token valid when an altered TokenDto is provided
	Validate token when TokenDto parameter is altered
	Given an altered token
	And create HttpContent for isTokenValid
	When try validate token
	Then an HttpStatusCode.OK is returned
	And following details of error for isTokenValid are
		| Status | Message                  |
		| False  | Token validation failed. |





Scenario: Get user details with a JWT token that has no email claim
	Get user with a JWT token that has no email claim
	Given a UserDto object with empty fields
    And generate JWT token and set it in the Authorization header 
    When try get user
    Then an HttpStatusCode.OK is returned
	And the following details of error for getting user are
		| Status | Message                                              |
		| False  | An error occurred during processing, user not found. |

Scenario: Get user details with a JWT token that has inexistent credentials
	Get user with a JWT token that has inexistent credentials in database
	Given a UserDto object with inexistent credentials
	And generate JWT token and set it in the Authorization header 
    When try get user
	Then an HttpStatusCode.OK is returned
	And the following details of error for getting user are
		| Status | Message                                              |
		| False  | An error occurred during processing, user not found. |

Scenario: Get user details with a JWT token that is valid
	Get user details with a JWT token that is valid
	Given a UserDto object with valid fields, user is in database
	And generate JWT token and set it in the Authorization header 
    When try get user
	Then an HttpStatusCode.OK is returned
	And the same user details is returned





Scenario: Update user with invalid UpdateUserDto object, empty fields
	Update user with invalid UpdateUserDto object, empty fields
	Given a UserDto object with valid fields, user is in database
	And generate JWT token and set it in the Authorization header
	And create an UpdateUserDto object with empty fields
	And create HttpContent for update
	When try update user
	Then an HttpStatusCode.BadRequest is returned
	And the following details of error for update user with an invalid UpdateUserDto object with empty fields are
		| ErrorCount | UserNameError                    | EmailError0                    | EmailError1            | PasswordError                   | ConfirmPasswordError                    | OldEmailError                   |
		| 5          | The User name field is required. | The Email address is required. | Invalid Email address. | The Password field is required. | The Confirm password field is required. | The OldEmail field is required. |

Scenario: Update user with invalid UpdateUserDto object, wrong fields
	Update user with invalid UpdateUserDto, wrong fields
	Given a UserDto object with valid fields, user is in database
	And generate JWT token and set it in the Authorization header
	And create an invalid UpdateUserDto object
	And create HttpContent for update
	When try update user
	Then an HttpStatusCode.BadRequest is returned
	And the following details of error for update user with invalid UpdateUserDto object are
		| ErrorCount | EmailError             | ConfirmPasswordError                                 |
		| 2          | Invalid Email address. | The Password and Confirmation password do not match. |

Scenario: Update user with invalid UpdateUserDto object, wrong OldEmail, user to be updated cannot be found in database
	Update user with invalid UpdateUserDto object, wrong OldEmail, user to be updated cannot be found in database
	Given a UserDto object with valid fields, user is in database
	And generate JWT token and set it in the Authorization header
	And create an invalid UpdateUserDto object with wrong OldEmail field
	And create HttpContent for update
	When try update user
	Then an HttpStatusCode.OK is returned
	And the following details of error for updated with invalid UpdatedUserDto object, wrong OldEmail field, are
		| Status | Message                                              |
		| False  | An error occurred during processing, user not found. |

Scenario: Update user with valid UpdateUserDto object
	Update user with valid UpdateUserDto object, return updated user details
	Given a UserDto object with valid fields, user is in database
	And generate JWT token and set it in the Authorization header
	And create a valid UpdateUserDto object
	And create HttpContent for update
	When try update user
	Then an HttpStatusCode.OK is returned
	And returned data are the same as in UpdateUserDto object