# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["Net10SudokuApp.Web/Net10SudokuApp.Web.csproj", "Net10SudokuApp.Web/"]
COPY ["Net10SudokuApp.ServiceDefaults/Net10SudokuApp.ServiceDefaults.csproj", "Net10SudokuApp.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "Net10SudokuApp.Web/Net10SudokuApp.Web.csproj"

# Copy all source files
COPY . .

# Build and publish
WORKDIR "/src/Net10SudokuApp.Web"
RUN dotnet publish "Net10SudokuApp.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
# NOTE: The application is configured to listen on HTTP only (port 8080).
# It is assumed that HTTPS termination is handled by an external reverse proxy (e.g., in Container Apps).
ENV ASPNETCORE_URLS=http://+:8080

# Copy published app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Net10SudokuApp.Web.dll"]
