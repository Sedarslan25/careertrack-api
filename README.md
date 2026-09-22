# CareerTrack API

A minimal ASP.NET Core REST API for tracking job applications. It exposes a small, testable CRUD surface and OpenAPI metadata without requiring a database or external package.

**Kısa Türkçe özet:** İş başvurularını kaydetmek, durumlarını güncellemek ve listelemek için hazırlanmış ASP.NET Core REST API örneği.

## Features

- Create, list, retrieve, update status, and delete job applications
- Optional status filtering (`Saved`, `Applied`, `Interview`, `Offer`, `Rejected`)
- Request validation for company and role fields
- In-memory data store to keep setup friction low
- Clear endpoint examples in the README

## Run locally

```bash
dotnet run
```

Try it in another terminal:

```bash
curl http://localhost:5000/api/applications
```

Create an entry:

```bash
curl -X POST http://localhost:5000/api/applications -H "Content-Type: application/json" -d "{\"company\":\"Example Studio\",\"role\":\"Backend Intern\",\"status\":\"Applied\"}"
```

## Design note

The API deliberately uses in-memory data, so records reset when the process stops. A database, user authentication, and pagination would be sensible next production steps.
