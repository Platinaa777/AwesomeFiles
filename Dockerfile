FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app

COPY src/AwesomeFiles.Api/AwesomeFiles.Api.csproj                          ./src/AwesomeFiles.Api/AwesomeFiles.Api.csproj 
COPY src/AwesomeFiles.Application/AwesomeFiles.Application.csproj          ./src/AwesomeFiles.Application/AwesomeFiles.Application.csproj
COPY src/AwesomeFiles.Domain/AwesomeFiles.Domain.csproj                    ./src/AwesomeFiles.Domain/AwesomeFiles.Domain.csproj
COPY src/AwesomeFiles.HttpModels/AwesomeFiles.HttpModels.csproj            ./src/AwesomeFiles.HttpModels/AwesomeFiles.HttpModels.csproj
COPY src/AwesomeFiles.Infrastructure/AwesomeFiles.Infrastructure.csproj    ./src/AwesomeFiles.Infrastructure/AwesomeFiles.Infrastructure.csproj

RUN dotnet restore ./src/AwesomeFiles.Api/AwesomeFiles.Api.csproj

COPY . ./
RUN dotnet publish ./src/AwesomeFiles.Api/AwesomeFiles.Api.csproj  -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "AwesomeFiles.Api.dll"]