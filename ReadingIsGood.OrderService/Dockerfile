# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore "./ReadingIsGood.OrderService.csproj"

# copy and publish app and libraries
COPY . .
WORKDIR "/src/."
RUN dotnet build "ReadingIsGood.OrderService.csproj" -c Release -o /app/build
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ReadingIsGood.OrderService.dll"]