# Samples

This directory contains sample projects demonstrating how to use Mistruna.Core.

## Available Samples

### BasicApi

A minimal API sample showing basic usage of:
- Exception handling middleware
- Custom exceptions (NotFoundException, ConflictException, ForbiddenAccessException)
- Swagger/OpenAPI integration

#### Running the Sample

```bash
cd samples/Mistruna.Core.Samples.BasicApi
dotnet run
```

Then navigate to `https://localhost:5001/swagger` to see the API documentation.

## Adding New Samples

When adding new samples, please:
1. Create a new folder under `samples/`
2. Add a descriptive README.md
3. Update this file with the new sample description
4. Ensure the sample project references the main library via ProjectReference
