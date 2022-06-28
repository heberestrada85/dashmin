# Build sdk image
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS builder
LABEL maintainer="Heber Estrada <heber.estrada@pm.me>"

ENV TZ=America/Chihuahua
# Copy csproj and restore as distinct layers
WORKDIR /app

# Copy everything else and build
COPY  . ./app/

RUN ln -sf /usr/share/zoneinfo/America/Chihuahua /etc/localtime
RUN dotnet restore ./app/Dashmin.Client/Dashmin.Client.csproj
RUN dotnet publish ./app/Dashmin.Client/Dashmin.Client.csproj -c Release --output out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:3.1
LABEL maintainer="Heber Estrada <heber.estrada@pm.me>"
WORKDIR /app

COPY --from=builder /app/out .

EXPOSE 5001
ENTRYPOINT ["dotnet", "Dashmin.Client.dll"]