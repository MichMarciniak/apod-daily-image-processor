# Apod Daily Image Processor
Async system for synchronizing and processing images from NASA APOD APi.

## Key Features
- **Auto-Sync**: Once a day, the app check NASA for a new photo.
- **Processing**: Creates smaller thumbnails.
- **Background Work**: Uses 2 separate workers - one for fetching data, other for handling images.

## Tech
- **Framework**: .NET 10 (C#)
- **Database**: SQLite

## How to run
1. Clone this repository
2. Modify `appsettings.json` - example is in `appsettings.Development.json`
3. Add you NASA API key
```json
{
  "Secrets": {
    "ApiKey": "key_goes_here"
  }
}

```
4. Run project using IDE or `dotnet run`
