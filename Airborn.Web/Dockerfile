# NuGet restore
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .

# testing
FROM build AS testing
RUN dotnet build

# publish
FROM build AS publish
RUN dotnet publish -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .

# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet airborn.web.dll