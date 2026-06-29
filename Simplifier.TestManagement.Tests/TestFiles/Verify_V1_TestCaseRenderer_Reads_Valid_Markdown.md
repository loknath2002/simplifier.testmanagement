# TC-1: Test Case 1

> Author: ##Description
>
> Date: 01-Jan-2020
>
> Tags: Tag1,Tag2
>
> Status: Draft

## Description

>Test case description goes here.
>SecondLine of description.
>* User is logged out.
>* System is in a known state.

## Preconditions

> Test case preconditions go here.
> * User is logged out.
> * System is in a known state.

## Dataset

> Test case data goes here.
> * Username:something
> * Password:something

## Steps & Expected Results

| Step | Procedure                                             | Expected Result                                                           |
| ---- | ----------------------------------------------------- | ------------------------------------------------------------------------- |
| 1    | Navigate to the application login screen.             | Login form is displayed with Username and Password fields.                |
| 2    | Navigate to the application login screen.             | Login form is displayed with Username and Password fields.                |
| 3    | Enter valid `username` and `password` from Test Data. | Characters are masked in the password field. Login button becomes active. |
| 4    | Click the "Login" button.                             | User is redirected to the dashboard. Session token is stored locally.     |

## Run History

### Run-2: 01-Jan-2021

| Step | Status  | Notes           |
| ---- | ------- | --------------- |
| 1    | Pass    |                 |
| 2    | Fail    | 2nd login fails |
| 3    | Started |                 |
| 4    | Pending |                 |

### Run-1: 01-Jan-2020

| Step | Status | Notes |
| ---- | ------ | ----- |
| 1    | Pass   |       |
| 2    | Pass   |       |
| 3    | Pass   |       |
| 4    | Pass   |       |

#### Internal Program Data [DO NOT CHANGE OR REMOVE]

> Program Used: Super Simple Test Management (SSTM)
>
> SchemaVersion: SSTM1
