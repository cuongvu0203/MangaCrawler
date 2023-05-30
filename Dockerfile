#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SayHenTai-WebApp.csproj", "."]
RUN dotnet restore "./SayHenTai-WebApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SayHenTai-WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SayHenTai-WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SayHenTai-WebApp.dll"]