FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
RUN mkdir -p /app/Dictionaries
COPY --from=build-env /app/Dictionaries/*.aff .
COPY --from=build-env /app/Dictionaries/*.dic .
COPY --from=build-env /app/Dictionaries/*.txt .
ENTRYPOINT ["dotnet", "jobs.dll"]