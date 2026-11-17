# Auth

Currently the application has no real auth only user name. This document will detial the auth system that you will implement. 

You will leveraege .NETs Idenitiy API Endpoints, using Npgsql.EntityFrameworkCore.PostgreSQL, you can auto migrate auth tables and passowrd hashing code. use https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-10.0&tabs=visual-studio for documentation. 

Use a Auth Engine in the util layer to handle auth. Every endpoint should have a simple general user auth check to prevent attacks.

For the frontend use a cookie based auth token. Add a auth service layer in the FE for managing and checking for this cookie. For auth, have cookie expiration after 24 hours of no activitiy triggering a reloggin event. 

For the actual auth schema a simple username, email, password approach will be used, email and password as the primary keys for auth. Every workout template, set and session all belong to a user and should only be visible to the users. This is with the exception of a generatl template "Power Upper Body Workout" template that will be avaiable to everyone upon login. In addition to this, remove the ability to log into others accounts from login screen and enable a login via email and password. 
