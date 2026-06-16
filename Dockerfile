FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ARG API_BASE_URL=http://localhost:5000

COPY ["DotnetNiger.UI.csproj", "."]
RUN dotnet restore "DotnetNiger.UI.csproj"

COPY . .

RUN sed -i "s|\"ApiBaseUrl\": \".*\"|\"ApiBaseUrl\": \"${API_BASE_URL}\"|" wwwroot/appsettings.json

RUN dotnet publish "DotnetNiger.UI.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY --from=build /app/publish/wwwroot .

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget -qO- http://localhost:80/ || exit 1

ENTRYPOINT ["nginx", "-g", "daemon off;"]
