# -----------------------------
# Stage 1: Build
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY . ./
RUN dotnet restore DotNetWbapi.sln
RUN dotnet publish DotNetWbapi.sln -c Release -o /app/publish


# -----------------------------
# Stage 2: Runtime
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Set port for Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Start the API
ENTRYPOINT ["dotnet", "DotNetWbapi.dll"]
