# Guide de Déploiement

Ce guide couvre le déploiement des **2 applications frontend** du projet DotnetNiger :

| Application | Type | Hébergement |
|-------------|------|-------------|
| **DotnetNiger.UI** | Blazor WebAssembly (WASM) → **statique** | Partout : CDN, S3, Pages, etc. |
| **Identity.Web** | ASP.NET Core Razor Pages → **serveur** | Nécessite un runtime .NET ou Docker |

---

## 1. DotnetNiger.UI (Blazor WASM)

Le Blazor WASM compile en **fichiers statiques** (HTML, JS, CSS, DLL WASM). Pas besoin de serveur .NET pour l'exécuter — un simple serveur HTTP/nginx suffit.

### 1.1 Publier

```bash
# Publish les fichiers statiques
dotnet publish -c Release -o publish/ui

# Le dossier publish/ui/wwwroot/ contient tout le nécessaire
```

Avant de publier, modifier `wwwroot/appsettings.json` :
```json
{
  "ApiBaseUrl": "https://TON-BACKEND.com",
  "UseMockServices": false
}
```

### 1.2 Options de déploiement

| Plateforme | Type | Prix | HTTPS | Particularité |
|------------|------|------|-------|---------------|
| **GitHub Pages** | Statique | Gratuit | Oui | Push le dossier `wwwroot/` sur branche `gh-pages` |
| **Netlify** | Statique | Gratuit | Oui | Drag & drop ou connexion GitHub |
| **Vercel** | Statique | Gratuit | Oui | Déploiement auto depuis GitHub |
| **Cloudflare Pages** | Statique | Gratuit | Oui | CDN mondial, connexion GitHub |
| **Hugging Face Static** | Statique | Gratuit | Oui | Idéal si le backend est déjà sur HF |
| **Firebase Hosting** | Statique | Gratuit | Oui | 10 GB bande passante/mois |
| **Oracle Object Storage** | Statique | Gratuit | Oui* | 10 GB, HTTPS via Cloudflare |
| **AWS S3 + CloudFront** | Statique | Gratuit** | Oui | 1ère année seulement |
| **Docker (nginx)** | Conteneur | Variable | Oui | Pour orchestrer avec le backend |

*\*HTTPS nécessite Cloudflare ou un CDN devant.*
*\*\*AWS Free Tier : 1 an.*

### 1.3 Configuration par plateforme

#### GitHub Pages

```bash
# 1. Créer un repo GitHub pour le frontend
# 2. Pusher le code source

# 3. Publier et déployer
dotnet publish -c Release -o publish/ui
cd publish/ui/wwwroot
git init
git checkout -b gh-pages
git add . && git commit -m "Deploy"
git remote add origin https://github.com/TON_COMPTE/DotnetNiger.UI.git
git push origin gh-pages --force
```

Dans Settings → Pages : choisir la branche `gh-pages`.

#### Netlify

```bash
dotnet publish -c Release -o publish/ui
# Glisser-déposer publish/ui/wwwroot dans Netlify
```

Ou connexion GitHub → déploiement automatique.

#### Cloudflare Pages

```bash
# Connecter le repo GitHub
# Build command : dotnet publish -c Release -o publish
# Publish directory : publish/wwwroot
```

### 1.4 Connexion au backend

Où que soit déployé le frontend, il doit pointer vers l'URL du backend :

```json
// wwwroot/appsettings.json
{
  "ApiBaseUrl": "https://ton-backend.hf.space"
}
```

**CORS** : le backend doit accepter les requêtes depuis l'origine du frontend. Le backend DotnetNiger inclut déjà la config CORS dans `deploy/nginx.conf` qui autorise toutes les origines `*.hf.space`. Pour d'autres domaines, ajoute l'origine dans la config nginx du backend.

> **Alternative sans CORS** : utilise le **proxy nginx** du frontend (Dockerfile.hf). Le frontend et le backend sont sur la même origine → pas de CORS.

---

## 2. Identity.Web (ASP.NET Core Razor Pages)

Identity.Web est une application **ASP.NET Core Razor Pages** qui nécessite un runtime .NET. Elle ne peut pas être déployée comme un site statique.

### 2.1 Options de déploiement

| Plateforme | Type | Prix | Détail |
|------------|------|------|--------|
| **Docker (recommandé)** | Conteneur | Variable | Via docker-compose avec les autres services |
| **MonsterASP.NET** | .NET 9 hosting | Gratuit | Déploiement direct des binaires |
| **Somee.com** | .NET 9 hosting | Gratuit | 150 MB, bandeaux pub |
| **Oracle Cloud VM** | VPS | Gratuit | Docker ou .NET runtime direct |
| **Azure App Service** | PaaS | Gratuit 1 an | 60 min compute/jour |
| **Your own VPS** | VPS | 3-5€/mois | Contrôle total |

### 2.2 Via Docker

Identity.Web est inclus dans le `docker-compose.yml` du backend :

```bash
git clone https://github.com/akaletekoffilevis/DotnetNiger.git
cd DotnetNiger
docker compose up --build -d identity-web
```

Port : `5100`. L'application se connecte à la Gateway (`http://gateway:5000/identity-api`) pour l'authentification OIDC.

### 2.3 Déploiement direct (MonsterASP)

```bash
dotnet publish DotnetNiger.Identity.Web/DotnetNiger.Identity.Web.csproj \
  -c Release -o publish/identity-web

# Uploader le dossier vers MonsterASP
```

Variables d'environnement :
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
Identity__BaseUrl=https://ton-backend.com/identity-api
Identity__ClientId=web-ui
DeveloperPortal__GatewayBaseUrl=https://ton-backend.com
```

> **Important** : Identity.Web a besoin du **backend** (Gateway + Identity) pour fonctionner. Il ne peut pas tourner seul.

---

## 3. Résumé : Quelle option choisir ?

| Ton besoin | Frontend (Blazor WASM) | Identity.Web (Razor Pages) |
|------------|----------------------|---------------------------|
| **Démo rapide** | Netlify / GitHub Pages | N/A (backend requis) |
| **Avec backend HF** | Static HF + CORS | Inclus dans le Docker HF |
| **Tout Docker** | Dockerfile (nginx) | Inclus dans docker-compose |
| **Production** | Cloudflare Pages | Docker sur Oracle/VPS |

> **Règle générale** : le Blazor WASM peut être servi par n'importe quel serveur HTTP. Identity.Web a besoin du runtime .NET et du backend. Les deux applications dépendent du backend DotnetNiger (Identity + Community + Gateway) pour fonctionner.
