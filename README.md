# ReactGamesListAPI

A ASP.NET Core Web API for managing users and games with cookie-based authentication.

## Overview

ReactGamesListAPI is a backend service that allows users to create accounts, authenticate, and manage a personal list of games. The API uses cookie-based authentication to protect user-specific operations.

Related project [Games Library](https://github.com/irakli-ka/React_App-Games_Library/tree/additional-backend-implementation) uses this API.

## Features

- User registration and authentication
- Cookie-based session management
- View user profiles and game collections
- Add and remove games from a personal list
- Retrieve all users and games
- Delete user accounts

## Technologies

- **Language**: C#
- **Framework**: ASP.NET Core Web API
- **Database**: Microsoft SQL Server 2019 Express
- **ORM**: Entity Framework Core
- **Authentication**: Cookie Authentication
- **Data Mapping**: AutoMapper
- **Password Hashing**: ASP.NET Identity PasswordHasher
- **Containerization**: Docker & Docker Compose
- **API Documentation**: Swagger

## Authentication

Protected endpoints require a valid authentication cookie obtained from the login endpoint. The cookie is automatically sent with subsequent requests.

## API Endpoints

### Users

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/users/signup` | Create a new user account | ❌ |
| POST | `/users/login` | Authenticate and receive auth cookie | ❌ |
| POST | `/users/logout` | Log out and invalidate session | ✅ |
| GET | `/users/me` | Get current authenticated user | ✅ |
| DELETE | `/users` | Delete current user account | ✅ |
| GET | `/users` | Get all users | ❌ |
| GET | `/users/id/{id}` | Get user by ID | ❌ |
| GET | `/users/username/{username}` | Get user by username | ❌ |
| POST | `/users/games` | Add a game to current user's list | ✅ |
| DELETE | `/users/games?gameId={id}` | Remove a game from current user's list | ✅ |

### Games

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/games` | Get all games | ❌ |
| GET | `/games/{id}` | Get a game by ID | ❌ |
| GET | `/games/username/{username}` | Get games for a specific user | ❌ |







